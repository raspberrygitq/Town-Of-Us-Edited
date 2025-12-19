using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.MedicMod;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Modifiers.SuperstarMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ArrowUpdate
    {
        public static Sprite Arrow => TownOfUsEdited.Arrow;
        public static Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();

        public static void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture) && CustomGameOptions.VultureArrow) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac) && CustomGameOptions.RememberArrows) return;

            foreach (var role in Modifier.GetModifiers(ModifierEnum.Superstar))
            {
                var superstar = (Superstar)role;
                if (superstar.Player.Data.IsDead && superstar.Reported == false)
                {
                    var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x =>
                        Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.PlayerId == superstar.Player.PlayerId));

                    foreach (var bodyArrow in BodyArrows.Keys)
                    {
                        if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        {
                            DestroyArrow(bodyArrow);
                        }
                    }

                    foreach (var arrow in BodyArrows)
                    {
                        arrow.Value.image.color = Patches.Colors.Superstar;
                    }

                    foreach (var body in validBodies)
                    {
                        if (!BodyArrows.ContainsKey(body.ParentId))
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Arrow;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            BodyArrows.Add(body.ParentId, arrow);
                        }
                        BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                    }
                }
                else
                {
                    if (BodyArrows.Count != 0)
                    {
                        BodyArrows.Values.DestroyAll();
                        BodyArrows.Clear();
                    }
                }
            }
        }
    }
}
