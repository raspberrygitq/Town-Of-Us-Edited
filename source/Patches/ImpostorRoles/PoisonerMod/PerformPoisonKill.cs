using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using Reactor;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformPoisonKill
    {
        public static Sprite PoisonSprite => TownOfUs.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUs.PoisonedSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (target == null) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.Cooldown > 0) return false;
            if (role.Enabled == true) return false;
            if (PlayerControl.LocalPlayer.IsJailed()) return false;
            if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
            {
                Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                return false;
            }
            else if (role.ClosestPlayer.Is(RoleEnum.PotionMaster) && Role.GetRole<PotionMaster>(role.ClosestPlayer).UsingPotion
            && Role.GetRole<PotionMaster>(role.ClosestPlayer).Potion == "Shield")
            {
                role.Cooldown = CustomGameOptions.PotionKCDReset;
                return false;
            }
            if (role.ClosestPlayer.IsGuarded2())
            {
                role.Cooldown = CustomGameOptions.GuardKCReset;
                return false; 
            }
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                role.PoisonedPlayer = target;
                role.TimeRemaining = CustomGameOptions.PoisonDuration;
                role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);
                Utils.Rpc(CustomRPC.Poison, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.PoisonCD;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.ProtectKCReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
