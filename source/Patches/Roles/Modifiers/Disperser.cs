using Hazel;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Object = UnityEngine.Object;
using Reactor.Networking.Extensions;
using System;
using TownOfUsEdited.Modifiers.ShyMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.CrewmateRoles.TimeLordMod;

namespace TownOfUsEdited.Roles.Modifiers
{
    public class Disperser : Modifier
    {
        public KillButton DisperseButton;

        public bool ButtonUsed;
        public DateTime StartingCooldown { get; set; }
        public Disperser(PlayerControl player) : base(player)
        {
            Name = "Disperser";
            TaskText = () => "Separate the <color=#00FFFF>Crewmates</color>";
            Color = Patches.Colors.Impostor;
            StartingCooldown = DateTime.UtcNow;
            ModifierType = ModifierEnum.Disperser;
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Disperse()
        {
            Dictionary<byte, Vector2> coordinates = GenerateDisperseCoordinates();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                254, SendOption.Reliable, -1);
            writer.Write((int)CustomRPC.Disperse);
            writer.Write((byte)coordinates.Count);
            foreach ((byte key, Vector2 value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            DispersePlayersToCoordinates(coordinates);
        }

        public static void DispersePlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Coroutines.Start(Utils.FlashCoroutine(Palette.ImpostorRed));
                if (Minigame.Instance)
                {
                    try
                    {
                        Minigame.Instance.Close();
                    }
                    catch
                    {

                    }
                }

                if (PlayerControl.LocalPlayer.inVent && !PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless))
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }

                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless))
                {
                    var position = PlayerControl.LocalPlayer.transform.position;
                    TimeLordPatches.Positions.Add((Vector2.zero, Time.time, "Teleport", position, 0, null));
                }
            }


            foreach ((byte key, Vector2 value) in coordinates)
            {
                PlayerControl player = Utils.PlayerById(key);
                var position = value;
                if (player.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                {
                    position = new Vector2(position.x, position.y - SizePatch.Radius * 0.75f);
                }
                if (!player.Is(ModifierEnum.Motionless))
                {
                    player.transform.position = position;
                    if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
                }
                if (player.Is(ModifierEnum.Shy) && player.GetCustomOutfitType() == CustomPlayerOutfitType.Default)
                {
                    var shy = GetModifier<Shy>(player);
                    shy.Opacity = 1f;
                    HudManagerUpdate.SetVisiblity(player, shy.Opacity);
                    shy.Moving = true;
                }
            }

            if (PlayerControl.LocalPlayer.walkingToVent && !PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless))
            {
                PlayerControl.LocalPlayer.inVent = false;
                Vent.currentVent = null;
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.MyPhysics.StopAllCoroutines();
            }

            if (SubmergedCompatibility.isSubmerged()) SubmergedCompatibility.ChangeFloor(PlayerControl.LocalPlayer.transform.position.y > -7f);
        }

        private Dictionary<byte, Vector2> GenerateDisperseCoordinates()
        {
            List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();

            HashSet<Vent> vents = Object.FindObjectsOfType<Vent>().ToHashSet();

            Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>(targets.Count);
            foreach (PlayerControl target in targets)
            {
                Vent vent = vents.Random();

                Vector3 destination = SendPlayerToVent(vent);
                coordinates.Add(target.PlayerId, destination);
            }
            return coordinates;
        }

        public static Vector3 SendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }
    }
}