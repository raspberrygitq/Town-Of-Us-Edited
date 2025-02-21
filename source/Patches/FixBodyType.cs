using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetBodyType))]
    public class FixBodyType
    {
        public static bool Prefix([HarmonyArgument(0)] PlayerControl player, ref PlayerBodyTypes __result)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return true;
            if (!player.isKilling) return true;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.Normal) return true;
            if (player.Is(Faction.Impostors) && CustomGameOptions.GameMode == GameMode.Werewolf) __result = PlayerBodyTypes.Seeker;
            else if (player.Is(RoleEnum.WhiteWolf) && CustomGameOptions.GameMode == GameMode.Werewolf) __result = PlayerBodyTypes.Seeker;
            else if (CustomGameOptions.GameMode == GameMode.Chaos && player.Is(Faction.Impostors)) __result = PlayerBodyTypes.Seeker;
            else if (player.Is(RoleEnum.Mutant) && Role.GetRole<Mutant>(player).IsTransformed == true) __result = PlayerBodyTypes.Seeker;
            else if (AprilFoolsMode.ShouldHorseAround()) __result = PlayerBodyTypes.Horse;
            else if (AprilFoolsMode.ShouldLongAround()) __result = PlayerBodyTypes.Long;
            else __result = PlayerBodyTypes.Normal;
            return false;
        }
    }
}