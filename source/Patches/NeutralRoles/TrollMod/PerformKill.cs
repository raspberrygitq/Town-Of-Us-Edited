using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Troll))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var troll = Role.GetRole<Troll>(PlayerControl.LocalPlayer);
            var killbutton = HudManager.Instance.KillButton;

            if (troll.ClosestPlayer == null)
                return false;

            if (__instance == killbutton)
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, troll.ClosestPlayer);
                if (interact[4] == true)
                {
                    troll.TrollAbility(troll.ClosestPlayer, PlayerControl.LocalPlayer);
                    Utils.Rpc(CustomRPC.Troll, PlayerControl.LocalPlayer.PlayerId, troll.ClosestPlayer.PlayerId);
                }
                return false;
            }

            return false;
        }
    }
}