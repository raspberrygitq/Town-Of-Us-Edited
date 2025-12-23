using AmongUs.GameOptions;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Spectator : Role
    {
        public Spectator(PlayerControl player) : base(player)
        {
            Name = "Spectator";
            ImpostorText = () => "Spectate The Game";
            TaskText = () => "Enjoy the game";
            RoleType = RoleEnum.Spectator;
            AddToRoleHistory(RoleType);
            Color = Patches.Colors.Spectator;
            DeathReason = DeathReasons.Spectator;
            Faction = Faction.NeutralBenign;
        }

        public void StartSpectate(PlayerControl player)
        {
            var hudManager = HudManager.Instance;
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
            }
            RoleManager.Instance.SetRole(player, RoleTypes.CrewmateGhost);
            Utils.ShowDeadBodies = true;
        }
    }
}