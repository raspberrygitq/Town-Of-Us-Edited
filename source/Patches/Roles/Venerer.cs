using System;
using TownOfUsEdited.Extensions;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Venerer : Role
    {
        public KillButton _abilityButton;
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float TimeRemaining;
        public float KillsAtStartAbility;

        public Venerer(PlayerControl player) : base(player)
        {
            Name = "Venerer";
            ImpostorText = () => "With Each Kill Your Ability Becomes Stronger";
            TaskText = () => "Kill players to unlock ability perks\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            Cooldown = CustomGameOptions.AbilityCd;
            RoleType = RoleEnum.Venerer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Alignment = Alignment.ImpostorConcealing;
        }

        public bool IsCamouflaged => TimeRemaining > 0f;

        public KillButton AbilityButton
        {
            get => _abilityButton;
            set
            {
                _abilityButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float AbilityTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void Ability()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
                PlayerMaterial.SetColors(Color.grey, Player.myRend());
                Player.nameText().color = Color.clear;
                Player.cosmetics.colorBlindText.color = Color.clear;
            }
        }


        public void StopAbility()
        {
            Enabled = false;
            Cooldown = CustomGameOptions.AbilityCd;
            if (!CamouflageUnCamouflage.IsCamoed) Utils.Unmorph(Player);
        }
    }
}