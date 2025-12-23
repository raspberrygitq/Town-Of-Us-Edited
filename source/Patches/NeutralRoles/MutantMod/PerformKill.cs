using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.Roles;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Patches.NeutralRoles.MutantMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mutant))
                return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            var role = Role.GetRole<Mutant>(PlayerControl.LocalPlayer);
            var killbutton = HudManager.Instance.KillButton;
                
            if (__instance == role.TransformButton)
            {
                if (role.TransformButton.graphic.sprite == TownOfUsEdited.TransformSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;

                    PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    Coroutines.Start(Utils.FlashCoroutine(Colors.Mutant, 0.5f));
                    var sound = GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX;
                    SoundManager.Instance.PlaySound(Object.Instantiate(sound, HudManager.Instance.transform.parent), false, 1f, null);
                    Utils.Rpc(CustomRPC.Transform, PlayerControl.LocalPlayer.PlayerId);
                    role.IsTransformed = true;
                    __instance.graphic.sprite = TownOfUsEdited.UnTransformSprite;
                    if (role.Cooldown > 5f) role.Cooldown = 5f;
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;

                    PlayerControl.LocalPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                    Utils.Rpc(CustomRPC.UnTransform, PlayerControl.LocalPlayer.PlayerId);
                    role.IsTransformed = false;
                    __instance.graphic.sprite = TownOfUsEdited.TransformSprite;
                    role.TransformCooldown = CustomGameOptions.TransformCD;
                    return false;
                }
            }
            
            if (__instance == killbutton)
            {
                if (role.ClosestPlayer == null)
                    return false;

                if (role.Cooldown > 0)
                return false;

                // Kill the closest player
                role.Kill(role.ClosestPlayer);
            }

            return false;
        }
    }
}