﻿using System.Collections.Generic;
using TownOfUsEdited.CrewmateRoles.ClericMod;
using TownOfUsEdited.Patches;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class Cleric : Role
    {
        public Cleric(PlayerControl player) : base(player)
        {
            Name = "Cleric";
            ImpostorText = () => "Save The Crewmates";
            TaskText = () => "Barrier and Cleanse crewmates";
            Color = Patches.Colors.Cleric;
            Cooldown = CustomGameOptions.BarrierCD;
            RoleType = RoleEnum.Cleric;
            Alignment = Alignment.CrewmateProtective;
            AddToRoleHistory(RoleType);
        }

        private KillButton _cleanseButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl Barriered;
        public Dictionary<byte, List<EffectType>> CleansedPlayers =  new Dictionary<byte, List<EffectType>>();
        public float TimeRemaining;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public KillButton CleanseButton
        {
            get => _cleanseButton;
            set
            {
                _cleanseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float BarrierTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        public void UnBarrier()
        {
            TimeRemaining = 0f;
            ShowShield.ResetVisor(Barriered, Player);
            Barriered = null;
        }

        public List<EffectType> FindNegativeEffects(PlayerControl cleansed)
        {
            var id = cleansed.PlayerId;
            var effects = new List<EffectType>();
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var arso = (Arsonist)role;
                if (arso.DousedPlayers.Contains(id)) effects.Add(EffectType.Douse);
            }
            foreach (var role in GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                if (glitch.Hacked == cleansed) effects.Add(EffectType.Hack);
            }
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var pb = (Plaguebearer)role;
                if (pb.InfectedPlayers.Contains(id)) effects.Add(EffectType.Infect);
            }
            foreach (var role in GetRoles(RoleEnum.Infectious))
            {
                var infect = (Infectious)role;
                if (infect.Infected.Contains(id)) effects.Add(EffectType.Infect2);
            }
            foreach (var role in GetRoles(RoleEnum.Blackmailer))
            {
                var bmer = (Blackmailer)role;
                if (bmer.Blackmailed == cleansed) effects.Add(EffectType.Blackmail);
            }
            foreach (var role in GetRoles(RoleEnum.Blinder))
            {
                var blinder = (Blinder)role;
                if (blinder.BlindedPlayer == cleansed) effects.Add(EffectType.Blind);
            }
            foreach (var role in GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier)role;
                if (grenadier.flashedPlayers.Contains(cleansed)) effects.Add(EffectType.Flash);
            }
            foreach (var role in GetRoles(RoleEnum.Hypnotist))
            {
                var hypnotist = (Hypnotist)role;
                if (hypnotist.HypnotisedPlayers.Contains(id)) effects.Add(EffectType.Hypnosis);
            }
            return effects;
        }
    }
}