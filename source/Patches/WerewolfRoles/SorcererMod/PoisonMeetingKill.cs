using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.WerewolfRoles.SorcererMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class PoisonMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            var sorcerers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Sorcerer)).ToList();
            foreach (var sorcerer in sorcerers)
            {
                var role = Role.GetRole<Sorcerer>(sorcerer);
                if (sorcerer != role.PoisonedPlayer && role.PoisonedPlayer != null)
                {
                    if (!role.PoisonedPlayer.Data.IsDead)
                        Utils.MurderPlayer(sorcerer, role.PoisonedPlayer, false);
                    if (PlayerControl.LocalPlayer == sorcerer)
                    {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
                    }
                }
                return;
            }
        }
    }
}
