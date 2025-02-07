using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Witch);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);

            if (__instance == role.SpellButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    role.CursedList.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.Curse, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
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
                }
                if (interact[0] == true)
                {
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    return false;
                }
                else if (interact[1] == true)
                {
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.ProtectKCReset + 0.01f;
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }

            return true;
        }
    }
}