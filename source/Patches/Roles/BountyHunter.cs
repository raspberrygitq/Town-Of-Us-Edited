using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Roles
{
    public class BountyHunter : Role
    {
        public Dictionary<byte, ArrowBehaviour> TargetArrow = new Dictionary<byte, ArrowBehaviour>();
        public KillButton _timerButton;
        public PlayerControl BountyTarget;
        public DateTime TargetSwitch;
        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            ImpostorText = () => "Hunt Down Your Target";
            if (BountyTarget != null)
            {
                TaskText = () => $"Kill your target to get a short kill cooldown\nCurrent Target: {BountyTarget.name}\nFake Tasks:";
            }
            else TaskText = () => $"Kill your target to get a short kill cooldown\nYou don't have a target for some reason... weird...\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.BountyHunter;
            Alignment = Alignment.ImpostorKilling;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public KillButton TimerButton
        {
            get => _timerButton;
            set
            {
                _timerButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float TargetTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - TargetSwitch;
            ;
            var num = CustomGameOptions.TargetDuration * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TargetArrow.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TargetArrow.Remove(arrow.Key);
        }
    }
}