using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Astral : Role
    {
        public bool Enabled = false;
        public float TimeRemaining;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;

        public Astral(PlayerControl player) : base(player)
        {
            Name = "Astral";
            ImpostorText = () => "Project Your Spirit To Investigate";
            TaskText = () => "Use Your Ability To Turn Into Ghost";
            Color = Patches.Colors.Astral;
            Cooldown = CustomGameOptions.GhostCD;
            RoleType = RoleEnum.Astral;
            Faction = Faction.Crewmates;
            Alignment = Alignment.CrewmateInvestigative;
            AddToRoleHistory(RoleType);
        }

        public bool UsingGhost => TimeRemaining > 0f;

        public float AstralTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void TurnGhost(PlayerControl player)
        {
            if (!Enabled)
            {
                Die(player);
            }
            TimeRemaining -= Time.deltaTime;
            Enabled = true;
        }

        public void Die(PlayerControl player)
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var amOwner = player.AmOwner;
            player.gameObject.layer = LayerMask.NameToLayer("Ghost");
            player.Visible = false;
            if (amOwner)
            {
                try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                importantTextTask.Text = "";
                player.myTasks.Insert(0, importantTextTask);
            }
            var deadBody = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = DateTime.UtcNow
            };

            Murder.KilledPlayers.Add(deadBody);
            player.MyPhysics.StartCoroutine(player.KillAnimations.Random().CoPerformKill(player, player));
        }

        public void TurnBack(PlayerControl player)
        {
            if (Enabled)
            {
                Revive(player);
            }
            Enabled = false;
            Cooldown = CustomGameOptions.GhostCD;
        }

        public void Revive(PlayerControl player)
        {
            Vector2 position;
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (!GameObject.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == player.PlayerId) && !MeetingHud.Instance) return;
            }
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == player.PlayerId)
                {
                    position = deadBody.TruePosition;
                    player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
                }
            }
            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == player.PlayerId) deadBody.gameObject.Destroy();
            }

            var revived = new List<PlayerControl>();

            player.Revive();
            revived.Add(player);
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }

            if (PlayerControl.LocalPlayer == player)
            {
                player.myTasks.RemoveAt(0);
                if (!MeetingHud.Instance)
                {
                    HudManager.Instance.Chat.gameObject.SetActive(false);
                }
                else
                {
                    HudManager.Instance.Chat.gameObject.SetActive(true);
                    MeetingHud.Instance.amDead = false;
		            MeetingHud.Instance.SkipVoteButton.AmDead = false;
		            MeetingHud.Instance.Glass.sprite = default;
		            MeetingHud.Instance.Glass.color = Palette.White;
                    if (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.No)
                    {
                        MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(true);
                    }
                }
            }
            return;
        }
    }
}