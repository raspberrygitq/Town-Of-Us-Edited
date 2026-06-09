using Il2CppSystem.Collections.Generic;
using Reactor.Utilities;

namespace TownOfUsEdited.Roles
{
    public class Troll : Role
    {
        public bool TrolledVotedOut;
        public bool UsedTroll = false;
        public PlayerControl TrolledPlayer;
        public PlayerControl ClosestPlayer;

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            ImpostorText = () => "Troll the crew";
            TaskText = () => "Make a player kill you and be ejected!\nFake Tasks:";
            Color = Patches.Colors.Troll;
            RoleType = RoleEnum.Troll;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public void TrollAbility(PlayerControl target, PlayerControl troll)
        {
            TrolledPlayer = target;
            UsedTroll = true;

            Utils.Interact(target, troll, true);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var TrollTeam = new List<PlayerControl>();
            TrollTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = TrollTeam;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Troll edition");
            if (Player.Data.Disconnected) return;
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                TrolledVotedOut = true;
                if (AmongUsClient.Instance.AmHost && CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    Coroutines.Start(WaitForEnd());
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Troll Win");
                }
            }
            else
            {
                HudManager.Instance.ShowPopUp("Normally, the game would've ended and the Troll would've won. In Freeplay, nothing happens.");
            }
        }
    }
}