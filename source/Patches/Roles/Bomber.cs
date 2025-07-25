using UnityEngine;
using System;
using TownOfUsEdited.ImpostorRoles.BomberMod;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles.Modifiers;
using TownOfUsEdited.CrewmateRoles.ClericMod;

namespace TownOfUsEdited.Roles
{
    public class Bomber : Role

    {
        public KillButton _plantButton;
        public float TimeRemaining;
        public bool Enabled = false;
        public bool Detonated = true;
        public Vector3 DetonatePoint;
        public Bomb Bomb = new Bomb();
        public static Material bombMaterial = TownOfUsEdited.bundledAssets.Get<Material>("bomb");
        public DateTime StartingCooldown { get; set; }

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            ImpostorText = () => "Plant Bombs To Kill Multiple Crewmates At Once";
            TaskText = () => "Plant bombs to kill <color=#00FFFF>Crewmates</color>\nFake Tasks:";
            Color = Palette.ImpostorRed;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Bomber;
            Alignment = Alignment.ImpostorKilling;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton PlantButton
        {
            get => _plantButton;
            set
            {
                _plantButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
        public bool Detonating => TimeRemaining > 0f;
        public void DetonateTimer()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (MeetingHud.Instance) Detonated = true;
            if (TimeRemaining <= 0 && !Detonated)
            {
                var bomber = GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.Bomb.ClearBomb();
                DetonateKillStart();
            }
        }
        public void DetonateKillStart()
        {
            Detonated = true;
            var playersToDie = Utils.GetClosestPlayers(DetonatePoint, CustomGameOptions.DetonateRadius, false);
            playersToDie = Shuffle(playersToDie);
            while (playersToDie.Count > CustomGameOptions.MaxKillsInDetonation) playersToDie.Remove(playersToDie[playersToDie.Count - 1]);
            foreach (var player in playersToDie)
            {
                if (!player.Is(RoleEnum.Pestilence) && !player.IsShielded() && !player.IsProtected() && !player.IsBarriered() && player != ShowShield.FirstRoundShielded && !PlayerControl.LocalPlayer.IsJailed())
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded() && !PlayerControl.LocalPlayer.IsJailed())
                {
                    foreach (var medic in player.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, player.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }
                else if (player.IsBarriered())
                {
                    foreach (var cleric in player.GetCleric())
                    {
                        StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Bloodlust) && playersToDie.Count > 0)
            {
                var modifier = Modifier.GetModifier<Bloodlust>(PlayerControl.LocalPlayer);
                var diedPlayers = playersToDie.Count;
                while (diedPlayers > 0)
                {
                    modifier.KilledThisRound++;
                    diedPlayers--;
                }
            }
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> Shuffle(Il2CppSystem.Collections.Generic.List<PlayerControl> playersToDie)
        {
            var count = playersToDie.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = playersToDie[i];
                playersToDie[i] = playersToDie[r];
                playersToDie[r] = tmp;
            }
            return playersToDie;
        }
    }
}