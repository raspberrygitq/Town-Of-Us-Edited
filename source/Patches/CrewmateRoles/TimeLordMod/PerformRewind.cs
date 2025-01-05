using System;
using System.Linq;
using HarmonyLib;
using Object = UnityEngine.Object;
using Reactor.Utilities;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using UnityEngine;
using System.Collections.Generic;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformRewind

    {
        public static List<byte> Revived = new List<byte>();
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord);
            if (!flag) return true;
            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.Cooldown > 0 && !role.Rewinding) return false;
            if (!__instance.enabled) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;

            Rewind(role);
            Utils.Rpc(CustomRPC.Rewind, PlayerControl.LocalPlayer.PlayerId);
            return false;
        }

        public static void Rewind(TimeLord role)
        {
            //System.Console.WriteLine("START...");
            role.TimeRemaining = CustomGameOptions.RewindDuration;
            role.Rewind();
            Coroutines.Start(Utils.FlashCoroutine(Colors.TimeLord, CustomGameOptions.RewindDuration));
            foreach (var deadPlayer in Murder.KilledPlayers)
            {
                if ((DateTime.UtcNow - deadPlayer.KillTime).TotalSeconds < CustomGameOptions.RewindDuration)
                {
                    var player = Utils.PlayerById(deadPlayer.PlayerId);
                    var playerRole = Role.GetRole(player);
                    if (!player.Is(RoleEnum.Astral) || Role.GetRole<Astral>(player).Enabled != true && player.Data.IsDead
                    && playerRole.InfectionState != 4)
                    {
                        Revive(player);
                    }
                }
            }
            if (Minigame.Instance)
                try
                {
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            if (PlayerControl.LocalPlayer.inVent)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
            }
            var position = PlayerControl.LocalPlayer.GetTruePosition();

            PlayerControl.LocalPlayer.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void Revive(PlayerControl player)
        {
            foreach (var poisoner in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisonerRole = (Poisoner)poisoner;
                if (poisonerRole.PoisonedPlayer == player) poisonerRole.PoisonedPlayer = poisonerRole.Player;
            }
            player.Revive();
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Revived.Add(player.PlayerId);
            var body = Object.FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == player.PlayerId);
            var position = body.TruePosition;

            if (PlayerControl.LocalPlayer == player) player.myTasks.RemoveAt(1);

            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }

            if (body != null)
                Object.Destroy(body.gameObject);

            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void StopRewind(TimeLord role)
        {
            //System.Console.WriteLine("STOP...");
            role.StopRewind();
            PlayerControl.LocalPlayer.moveable = true;
            //For some reason the coroutine doesn't stop on itself after the rewind duration
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                if (fullscreen.color.Equals(Colors.TimeLord))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                    fullscreen.enabled = false;
                }
            }
            Patches.SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            foreach (var playerid in Revived)
            {
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == playerid));
            }
        }
    }
}