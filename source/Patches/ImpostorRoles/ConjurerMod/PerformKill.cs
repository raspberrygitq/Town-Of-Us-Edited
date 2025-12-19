using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Modifiers.UnderdogMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.ConjurerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Conjurer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Conjurer>(PlayerControl.LocalPlayer);

            if (__instance == role.CurseButton)
            {
                if (!__instance.enabled) return false;
                if (role.CurseButton.graphic.sprite == TownOfUsEdited.Curse)
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.ClosestPlayer == null) return false;

                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (interact[4] == true)
                    {
                        role.CursedPlayer = role.ClosestPlayer;
                        __instance.graphic.sprite = role.Kill;
                    }
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    if (__instance.isCoolingDown) return false;
                    if (role.CursedPlayer == null) return false;
                    var notdead = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead).ToList();
                    var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    var playerToDie = Utils.GetClosestPlayer(role.CursedPlayer, notdead);
                    if (playerToDie == null || playerToDie.Is(Faction.Impostors)) return false;
                    if (Vector2.Distance(playerToDie.GetTruePosition(),
                    role.CursedPlayer.GetTruePosition()) > maxDistance) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;

                    Utils.Interact(role.CursedPlayer, playerToDie, true);
                    SoundManager.Instance.PlaySound(role.Player.KillSfx, false, 0.5f);
                    role.Kills += 1;
                    role.CursedPlayer = null;
                    __instance.graphic.sprite = TownOfUsEdited.Curse;
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.DetonateDelay;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                        Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC);
                    }
                    else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Lucky))
                    {
                        var num = Random.RandomRange(1f, 60f);
                        Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                    }
                    else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Bloodlust))
                    {
                        var modifier = Modifier.GetModifier<Bloodlust>(PlayerControl.LocalPlayer);
                        var num = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2;
                        if (modifier.KilledThisRound >= 2) Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                        else Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    }
                    else Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown; 
                    return false;
                }
            }

            return true;
        }
    }
}