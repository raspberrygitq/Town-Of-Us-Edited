using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Sprite => TownOfUsEdited.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HudManager.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return true;
            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var target = role.ClosestPlayer;
            if (!role.ButtonUsable) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                if (!CamouflageUnCamouflage.IsCamoed)
                {
                    if (RainbowUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                    {
                        renderer.color = RainbowUtils.Rainbow;
                    }
                    else
                    {
                        renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
                    }
                }
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = target.transform.position;

                role.TrackerArrows.Add(target.PlayerId, arrow);
                role.UsesLeft--;
            }
            if (interact[0] == true)
            {
                role.Cooldown = CustomGameOptions.TrackCd;
                return false;
            }
            else if (interact[1] == true)
            {
                role.Cooldown = CustomGameOptions.TempSaveCdReset;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
