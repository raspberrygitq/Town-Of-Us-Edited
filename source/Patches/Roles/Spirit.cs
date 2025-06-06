using System.Collections.Generic;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Spirit : Role
    {
        public bool Caught;
        public bool CompletedTasks;
        public bool Faded;
        public bool Revealed;
        public List<ArrowBehaviour> Arrows = new List<ArrowBehaviour>();

        public Spirit(PlayerControl player) : base(player)
        {
            Name = "Spirit";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks without being caught!\nFake Tasks:";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Spirit;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorGhost;
        }

        public void Fade()
        {
            if (Player == null || Player.gameObject == null) return;
            Faded = true;
            Player.Visible = true;
            Player.Collider.enabled = true;
            Player.gameObject.layer = LayerMask.NameToLayer("Players");
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.GhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = " "
                });
            }
            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
            Player.cosmetics.SetBodyCosmeticsVisible(false);
        }
    }
}