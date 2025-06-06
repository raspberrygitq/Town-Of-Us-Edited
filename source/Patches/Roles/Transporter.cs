using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TMPro;
using Reactor.Utilities;
using System.Collections.Generic;
using TownOfUsEdited.Patches;
using System.Collections;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Patches.NeutralRoles;
using TownOfUsEdited.Roles.Modifiers;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.CrewmateRoles.TimeLordMod;
using TownOfUsEdited.CrewmateRoles.ClericMod;
using HudManagerUpdate = TownOfUsEdited.Modifiers.ShyMod.HudManagerUpdate;

namespace TownOfUsEdited.Roles
{
    public class Transporter : Role
    {
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public PlayerControl TransportPlayer1 { get; set; }
        public PlayerControl TransportPlayer2 { get; set; }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0 && !SwappingMenus;
        public bool SwappingMenus = false;

        public Dictionary<byte, DateTime> UntransportablePlayers = new Dictionary<byte, DateTime>();

        public Transporter(PlayerControl player) : base(player)
        {
            Name = "Transporter";
            ImpostorText = () => "Choose Two Players To Swap Locations";
            TaskText = () => "Choose two players to swap locations";
            Color = Colors.Transporter;
            Cooldown = CustomGameOptions.TransportCooldown;
            RoleType = RoleEnum.Transporter;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
            TransportPlayer1 = null;
            TransportPlayer2 = null;
            UsesLeft = CustomGameOptions.TransportMaxUses;
            Alignment = Alignment.CrewmateSupport;
        }

        public float TransportTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public static IEnumerator TransportPlayers(byte player1, byte player2, bool die)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary))
            {
                var merc = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
                if (merc.Guarded.Contains(player1))
                {
                    merc.Gold += 1;
                }
                if (merc.Guarded.Contains(player2))
                {
                    merc.Gold += 1;
                }
            }
            var TP1 = Utils.PlayerById(player1);
            var TP2 = Utils.PlayerById(player2);
            var deadBodies = Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;
            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP1.PlayerId) Player1Body = body;
                if (Player1Body == null) yield break;
            }
            if (TP2.Data.IsDead)
            {
                foreach (var body in deadBodies) if (body.ParentId == TP2.PlayerId) Player2Body = body;
                if (Player2Body == null) yield break;
            }

            if (TP1.Is(ModifierEnum.Shy) && !TP1.Data.IsDead && !TP1.Data.Disconnected && TP1.GetCustomOutfitType() == CustomPlayerOutfitType.Default)
            {
                var shy = Modifier.GetModifier<Shy>(TP1);
                shy.Opacity = 1f;
                HudManagerUpdate.SetVisiblity(TP1, shy.Opacity);
                shy.Moving = true;
            }
            if (TP2.Is(ModifierEnum.Shy) && !TP2.Data.IsDead && !TP2.Data.Disconnected && TP1.GetCustomOutfitType() == CustomPlayerOutfitType.Default)
            {
                var shy = Modifier.GetModifier<Shy>(TP2);
                shy.Opacity = 1f;
                HudManagerUpdate.SetVisiblity(TP2, shy.Opacity);
                shy.Moving = true;
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP1.MyPhysics.ExitAllVents();
            }
            if (TP2.inVent && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                while (SubmergedCompatibility.getInTransition())
                {
                    yield return null;
                }
                TP2.MyPhysics.ExitAllVents();
            }

            if ((PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId || PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId) && !PlayerControl.LocalPlayer.Is(ModifierEnum.Motionless))
            {
                var position = PlayerControl.LocalPlayer.transform.position;
                TimeLordPatches.Positions.Add((Vector2.zero, Time.time, "Teleport", position, 0, null));
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP2.MyPhysics.ResetMoveState();
                var TP1Position = TP1.GetTruePosition();
                TP1Position = new Vector2(TP1Position.x, TP1Position.y + 0.3636f);
                var TP2Position = TP2.GetTruePosition();
                TP2Position = new Vector2(TP2Position.x, TP2Position.y + 0.3636f);
                if (TP1.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                {
                    TP1Position = new Vector2(TP1Position.x, TP1Position.y + SizePatch.Radius * 0.75f);
                    TP2Position = new Vector2(TP2Position.x, TP2Position.y - SizePatch.Radius * 0.75f);
                }
                if (TP2.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                {
                    TP1Position = new Vector2(TP1Position.x, TP1Position.y - SizePatch.Radius * 0.75f);
                    TP2Position = new Vector2(TP2Position.x, TP2Position.y + SizePatch.Radius * 0.75f);
                }
                if (!TP1.Is(ModifierEnum.Motionless))
                {
                    TP1.transform.position = TP2Position;
                    TP1.NetTransform.SnapTo(TP2Position);
                }
                if (die) Utils.MurderPlayer(TP1, TP2, true);
                else
                {
                    if (!TP2.Is(ModifierEnum.Motionless))
                    {
                        TP2.transform.position = TP1Position;
                        TP2.NetTransform.SnapTo(TP1Position);
                    }
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }

            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                TP2.MyPhysics.ResetMoveState();
                var TP1Position = Player1Body.TruePosition;
                TP1Position = new Vector2(TP1Position.x, TP1Position.y + 0.3636f);
                var TP2Position = TP2.GetTruePosition();
                TP2Position = new Vector2(TP2Position.x, TP2Position.y + 0.3636f);
                if (TP2.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                {
                    TP1Position = new Vector2(TP1Position.x, TP1Position.y - SizePatch.Radius * 0.75f);
                    TP2Position = new Vector2(TP2Position.x, TP2Position.y + SizePatch.Radius * 0.75f);
                }
                Player1Body.transform.position = TP2Position;
                if (!TP2.Is(ModifierEnum.Motionless))
                {
                    TP2.transform.position = TP1Position;
                    TP2.NetTransform.SnapTo(TP1Position);
                }

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                StopDragging(Player2Body.ParentId);
                TP1.MyPhysics.ResetMoveState();
                var TP1Position = TP1.GetTruePosition();
                TP1Position = new Vector2(TP1Position.x, TP1Position.y + 0.3636f);
                var TP2Position = Player2Body.TruePosition;
                TP2Position = new Vector2(TP2Position.x, TP2Position.y + 0.3636f);
                if (TP1.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                {
                    TP1Position = new Vector2(TP1Position.x, TP1Position.y + SizePatch.Radius * 0.75f);
                    TP2Position = new Vector2(TP2Position.x, TP2Position.y - SizePatch.Radius * 0.75f);
                }
                if (!TP2.Is(ModifierEnum.Motionless)) Player2Body.transform.position = TP1Position;
                if (!TP1.Is(ModifierEnum.Motionless))
                {
                    TP1.transform.position = TP2Position;
                    TP1.NetTransform.SnapTo(TP2Position);
                }
                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                StopDragging(Player2Body.ParentId);
                var TempPosition = Player1Body.TruePosition;
                if (!TP1.Is(ModifierEnum.Motionless)) Player1Body.transform.position = Player2Body.TruePosition;
                if (!TP2.Is(ModifierEnum.Motionless)) Player2Body.transform.position = TempPosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId ||
                PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Transporter));
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            TP1.moveable = true;
            TP2.moveable = true;
            TP1.Collider.enabled = true;
            TP2.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.NetTransform.enabled = true;
        }

        public static void StopDragging(byte PlayerId)
        {
            var Undertaker = (Undertaker)Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Undertaker);
            if (Undertaker != null && Undertaker.CurrentlyDragging != null &&
                Undertaker.CurrentlyDragging.ParentId == PlayerId)
                Undertaker.CurrentlyDragging = null;

            var Doctor = (Doctor)Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Doctor);
            if (Doctor != null && Doctor.CurrentlyDragging != null &&
                Doctor.CurrentlyDragging.ParentId == PlayerId)
                Doctor.CurrentlyDragging = null;
        }

        public void HandleMedicPlague(HudManager __instance)
        {
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return;
            if (TransportPlayer1.IsFortified())
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Warden));
                foreach (var warden in TransportPlayer1.GetWarden())
                {
                    Utils.Rpc(CustomRPC.Fortify, (byte)1, warden.Player.PlayerId);
                }
                return;
            }
            else if (TransportPlayer2.IsFortified())
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Warden));
                foreach (var warden in TransportPlayer2.GetWarden())
                {
                    Utils.Rpc(CustomRPC.Fortify, (byte)1, warden.Player.PlayerId);
                }
                return;
            }
            if (!UntransportablePlayers.ContainsKey(TransportPlayer1.PlayerId) && !UntransportablePlayers.ContainsKey(TransportPlayer2.PlayerId))
            {
                if (Player.IsInfected() || TransportPlayer1.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer1);
                }
                if (Player.IsInfected() || TransportPlayer2.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(Player, TransportPlayer2);
                }
                var role = GetRole(Player);
                var transRole = (Transporter)role;
                if (TransportPlayer1.Is(RoleEnum.Pestilence) || TransportPlayer1.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        foreach (var medic in Player.GetMedic())
                        {
                            Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, Player.PlayerId);
                            StopKill.BreakShield(medic.Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }

                        if (CustomGameOptions.ShieldBreaks)
                            Cooldown = CustomGameOptions.TransportCooldown;
                        return;
                    }
                    else if (!Player.IsProtected() && !Player.IsBarriered())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, Player.PlayerId, true));

                        Utils.Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    else if (Player.IsBarriered())
                    {
                        foreach (var cleric in Player.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                        }
                    }
                    Cooldown = CustomGameOptions.TransportCooldown;
                    return;
                }
                else if (TransportPlayer2.Is(RoleEnum.Pestilence) || TransportPlayer2.IsOnAlert())
                {
                    if (Player.IsShielded())
                    {
                        foreach (var medic in Player.GetMedic())
                        {
                            Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, Player.PlayerId);
                            StopKill.BreakShield(medic.Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }

                        if (CustomGameOptions.ShieldBreaks)
                            Cooldown = CustomGameOptions.TransportCooldown;
                        return;
                    }
                    else if (!Player.IsProtected() && !Player.IsBarriered())
                    {
                        Coroutines.Start(TransportPlayers(TransportPlayer2.PlayerId, Player.PlayerId, true));

                        Utils.Rpc(CustomRPC.Transport, TransportPlayer2.PlayerId, Player.PlayerId, true);
                        return;
                    }
                    else if (Player.IsBarriered())
                    {
                        foreach (var cleric in Player.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                        }
                    }
                    Cooldown = CustomGameOptions.TransportCooldown;
                    return;
                }
                Cooldown = CustomGameOptions.TransportCooldown;
                UsesLeft--;

                Coroutines.Start(TransportPlayers(TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false));

                Utils.Rpc(CustomRPC.Transport, TransportPlayer1.PlayerId, TransportPlayer2.PlayerId, false);
            }
            else
            {
                __instance.StartCoroutine(Effects.SwayX(__instance.KillButton.transform));
            }
        }

        public IEnumerator OpenSecondMenu()
        {
            try
            {
                PlayerMenu.singleton.Menu.ForceClose();
            }
            catch
            {

            }
            yield return (object)new WaitForSeconds(0.05f);
            SwappingMenus = false;
            if (MeetingHud.Instance || !PlayerControl.LocalPlayer.Is(RoleEnum.Transporter)) yield break;
            List<byte> transportTargets = new List<byte>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Disconnected && player != TransportPlayer1)
                {
                    if (!player.Data.IsDead) transportTargets.Add(player.PlayerId);
                    else
                    {
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                        {
                            if (body.ParentId == player.PlayerId) transportTargets.Add(player.PlayerId);
                        }
                    }
                }
            }
            byte[] transporttargetIDs = transportTargets.ToArray();
            var pk = new PlayerMenu((x) =>
            {
                TransportPlayer2 = x;
                HandleMedicPlague(HudManager.Instance);
            }, (y) =>
            {
                return transporttargetIDs.Contains(y.PlayerId);
            });
            Coroutines.Start(pk.Open(0f, true));
        }
    }
}