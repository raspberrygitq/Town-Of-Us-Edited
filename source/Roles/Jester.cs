using Il2CppSystem.Collections.Generic;
using Reactor.Utilities;

namespace TownOfUsEdited.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public bool SpawnedAs = true;


        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "Get Voted Out";
            TaskText = () => SpawnedAs ? "Get voted out!\nFake Tasks:" : "Your target was killed. Now you get voted out!\nFake Tasks:";
            Color = Patches.Colors.Jester;
            RoleType = RoleEnum.Jester;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                VotedOut = true;
                if (AmongUsClient.Instance.AmHost && CustomGameOptions.NeutralEvilWinEndsGame)
                {
                    Coroutines.Start(WaitForEnd());
                    PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Jester Win");
                }
            }
            else
            {
                HudManager.Instance.ShowPopUp("Normally, the game would've ended and the Jester would've won. In Freeplay, nothing happens.");
            }
        }
    }
}