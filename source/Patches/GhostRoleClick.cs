using HarmonyLib;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.HaunterMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (MeetingHud.Instance) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Tasks == null) return;
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var nearghost = !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.GetTruePosition(), __instance.GetTruePosition(), Constants.ShipAndObjectsMask, false);
            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining && nearghost)
                {
                    var role = Role.GetRole<Phantom>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchPhantom, role.Player.PlayerId);
                }
            }
            else if (__instance.Is(RoleEnum.Wraith))
            {
                if (PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                else if (PlayerControl.LocalPlayer.Is(Faction.Madmates)) return;
                if (tasksLeft <= CustomGameOptions.WraithTasksRemainingClicked && nearghost)
                {
                    var role = Role.GetRole<Wraith>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchWraith, role.Player.PlayerId);
                }
            }
            else if (__instance.Is(RoleEnum.Haunter))
            {
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.ImpsOnly && !PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))) return;
                if (tasksLeft <= CustomGameOptions.HaunterTasksRemainingClicked && nearghost)
                {
                    var role = Role.GetRole<Haunter>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchHaunter, role.Player.PlayerId);
                }
            }
        }
    }
}