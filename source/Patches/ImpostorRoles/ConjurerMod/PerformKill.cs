using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System.Linq;
using TownOfUs.ImpostorRoles.ImpostorMod;

namespace TownOfUs.ImpostorRoles.ConjurerMod
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
                if (role.CurseButton.graphic.sprite == TownOfUs.Curse)
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.ClosestPlayer == null) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;

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
                    var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
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
                    __instance.graphic.sprite = TownOfUs.Curse;
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown; 
                    return false;
                }
            }

            return true;
        }
    }
}