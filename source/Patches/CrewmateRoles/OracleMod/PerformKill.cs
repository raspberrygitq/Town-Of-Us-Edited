using System;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUsEdited.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Oracle);
            if (!flag) return true;
            var role = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;

            if (__instance == role.BlessButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestBlessedPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (role.BlessCooldown > 0) return false;

                var interact2 = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestBlessedPlayer);
                if (interact2[4] == true)
                {
                    role.Blessed = role.ClosestBlessedPlayer;
                    Utils.Rpc(CustomRPC.Bless, PlayerControl.LocalPlayer.PlayerId, (byte)1, role.Blessed.PlayerId);
                    role.BlessCooldown = CustomGameOptions.BlessCD;
                }
                return false;
            }

            if (role.ClosestPlayer == null) return false;
            if (__instance != HudManager.Instance.KillButton) return true;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Confessor = role.ClosestPlayer;
                role.Cooldown = CustomGameOptions.ConfessCd;
                bool showsCorrectFaction = true;
                int faction = 1;
                if (role.Accuracy == 0f) showsCorrectFaction = false;
                else
                {
                    var num = UnityEngine.Random.RandomRangeInt(1, 101);
                    showsCorrectFaction = num <= role.Accuracy;
                }
                if (showsCorrectFaction)
                {
                    if (role.Confessor.Is(Faction.Crewmates)) faction = 0;
                    else if (role.Confessor.Is(Faction.Impostors)) faction = 2;
                    else if (role.Confessor.Is(Faction.Coven)) faction = 3;
                }
                else
                {
                    var num = UnityEngine.Random.RandomRangeInt(0, 3);
                    if (role.Confessor.Is(Faction.Impostors)) faction = num;
                    else if (role.Confessor.Is(Faction.Crewmates)) faction = num + 1;
                    else if (role.Confessor.Is(Faction.Coven)) faction = num - 1;
                    else if (num == 1) faction = 2;
                    else faction = 0;
                }
                if (faction == 0) role.RevealedFaction = Faction.Crewmates;
                else if (faction == 1) role.RevealedFaction = Faction.NeutralEvil;
                else if (faction == 3) role.RevealedFaction = Faction.Coven;
                else role.RevealedFaction = Faction.Impostors;
                Utils.Rpc(CustomRPC.Confess, PlayerControl.LocalPlayer.PlayerId, role.Confessor.PlayerId, faction);
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.ConfessCd;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
