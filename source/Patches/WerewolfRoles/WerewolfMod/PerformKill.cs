using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.Patches.WerewolfRoles.WerewolfMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                
            if (__instance == role.RampageButton)
            {
                if (role.RampageButton.graphic.sprite == TownOfUsEdited.RampageSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;

                    PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    Utils.Rpc(CustomRPC.WerewolfRampage, PlayerControl.LocalPlayer.PlayerId);
                    role.Rampaged = true;
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    
                    PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                    Utils.Rpc(CustomRPC.WerewolfUnRampage, PlayerControl.LocalPlayer.PlayerId);
                    role.Rampaged = false;
                    __instance.graphic.sprite = TownOfUsEdited.RampageSprite;
                    role.RampageCooldown = CustomGameOptions.RampageCD;
                    return false;
                }
            }

            return false;
        }
    }
}