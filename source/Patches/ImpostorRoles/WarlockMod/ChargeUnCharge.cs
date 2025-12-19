using HarmonyLib;
using TownOfUsEdited.Modifiers.UnderdogMod;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.WarlockMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ChargeUnCharge
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warlock)) return;
            foreach (var role in Role.GetRoles(RoleEnum.Warlock))
            {
                var warlock = (Warlock) role;
                if (warlock.Charging)
                    warlock.ChargePercent = warlock.ChargeUpTimer();
                else if (warlock.UsingCharge)
                {
                    warlock.ChargePercent = warlock.ChargeUseTimer();
                    if (warlock.ChargePercent <= 0f)
                    {
                        warlock.UsingCharge = false;
                        if (warlock.Player.Is(ModifierEnum.Underdog))
                        {
                            if (PlayerControl.LocalPlayer == warlock.Player)
                            {
                                var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                                var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                                var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC);
                            }
                        }
                        else if (warlock.Player.Is(ModifierEnum.Lucky) && PlayerControl.LocalPlayer == warlock.Player)
                        {
                            var num = Random.RandomRange(1f, 60f);
                            Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                        }
                        else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Bloodlust) && PlayerControl.LocalPlayer == warlock.Player)
                        {
                            var modifier = Modifier.GetModifier<Bloodlust>(PlayerControl.LocalPlayer);
                            var num = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2;
                            if (modifier.KilledThisRound >= 2) Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                            else Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        }
                        else if (PlayerControl.LocalPlayer == warlock.Player)
                        {
                            Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        }
                    }
                }
            }
        }
    }
}