using System;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUsEdited.Roles.Modifiers;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;

namespace TownOfUsEdited.CrewmateRoles.JailorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Jailor);
            if (!flag) return true;
            var role = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (PlayerControl.LocalPlayer.IsJailed()) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.JailedPlayer != null && role.JailedAssassin == true)
                {
                    new Assassin(role.JailedPlayer);
                    Utils.Rpc(CustomRPC.SetJailorAssassin, role.JailedPlayer);
                }
                role.JailedPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.SetJail, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                if (role.JailedPlayer.Is(AbilityEnum.Assassin))
                {
                    Ability.AbilityDictionary.Remove(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.RemoveJailorAssassin, role.ClosestPlayer.PlayerId);
                    role.JailedAssassin = true;
                }
                role.Cooldown = CustomGameOptions.JailCD;
                Utils.Rpc(CustomRPC.JailorPopUp, role.JailedPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.JailCD;
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
