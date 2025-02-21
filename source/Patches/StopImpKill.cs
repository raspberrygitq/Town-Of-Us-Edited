using HarmonyLib;
using TownOfUsEdited.Extensions;
using AmongUs.GameOptions;
using TownOfUsEdited.Modifiers.UnderdogMod;
using TownOfUsEdited.Roles;
using UnityEngine;
using TownOfUsEdited.CrewmateRoles.SheriffMod;
using TownOfUsEdited.Roles.Modifiers;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.isActiveAndEnabled || PlayerControl.LocalPlayer.coolingDown()) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return false;
            if (PlayerControl.LocalPlayer.IsJailed()) return false;
            HUDKill.ImpKillTarget(__instance);
            var target = __instance.currentTarget;
            if (target == null) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && target.Is(Faction.Coven))
            {
                Utils.Interact(target, PlayerControl.LocalPlayer, true);
                return false;
            }
            else if (target.Is(RoleEnum.PotionMaster) && Role.GetRole<PotionMaster>(target).UsingPotion
            && Role.GetRole<PotionMaster>(target).Potion == "Shield")
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.PotionKCDReset;
                return false;
            }
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (!target.inVent) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            if (target.IsGuarded())
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.WerewolfKillCD;
                return false;
            }
            if (target.IsGuarded2())
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.GuardKCReset;
                return false; 
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(PlayerControl.LocalPlayer).UsingManipulation)
            {
                var role = Role.GetRole<Manipulator>(PlayerControl.LocalPlayer);
                Utils.Interact(role.ManipulatedPlayer, target, true);
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
                return false;
            }
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[4] == true) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Warlock))
            {
                var warlock = Role.GetRole<Warlock>(PlayerControl.LocalPlayer);
                if (warlock.Charging)
                {
                    warlock.UsingCharge = true;
                    warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                    if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                }
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = 0.01f;
            }
            else if (interact[0] == true && !PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC);
                }
                else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Lucky))
                {
                    var num = Random.RandomRange(1f, 60f);
                    Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = num;
                }
                return false;
            }
            else if (interact[1] == true)
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.ProtectKCReset + 0.01f;
                return false;
            }
            else if (interact[2] == true)
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = CustomGameOptions.VestKCReset + 0.01f;
                return false;
            }
            else if (interact[3] == true)
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = 0.01f;
                return false;
            }
            return false;
        }
    }
}