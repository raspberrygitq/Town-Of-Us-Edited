﻿using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.Patches
{
    [HarmonyPatch]
    public static class SizePatch
    {
        public static float Radius = 0.2233912f;
        public static float Offset = 0.3636057f;
        public static GameObject heh;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                CircleCollider2D collider = player.Collider.Caster<CircleCollider2D>();
                if (player.Data != null && !(player.Data.IsDead || player.Data.Disconnected) && player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage)
                {
                    if (player.transform.localScale != player.GetAppearance().SizeFactor)
                    {
                        if (player.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                        {
                            var pos = player.transform.localPosition;
                            pos -= new Vector3(0f, Radius * 0.75f, 0f);
                            player.transform.localPosition = pos;
                            player.NetTransform.SnapTo(pos);
                        }
                        else if (player.transform.localScale == new Vector3(0.4f, 0.4f, 1.0f))
                        {
                            var pos = player.transform.localPosition;
                            pos += new Vector3(0f, Radius * 0.75f, 0f);
                            player.transform.localPosition = pos;
                            player.NetTransform.SnapTo(pos);
                        }
                    }
                    player.transform.localScale = player.GetAppearance().SizeFactor;
                    if (player.GetAppearance().SizeFactor == new Vector3(0.4f, 0.4f, 1.0f))
                    {
                        collider.radius = Radius * 1.75f;
                        collider.offset = Offset / 1.75f * Vector2.down;
                    }
                    else
                    {
                        collider.radius = Radius;
                        collider.offset = Offset * Vector2.down;
                    }
                }
                else
                {
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
                    collider.radius = Radius;
                    collider.offset = Offset * Vector2.down;
                }
            }

            var playerBindings = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.isDummy).ToDictionary(player => player.PlayerId);
            var bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (var body in bodies)
            {
                try {
                    var matches = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected && x.Is(RoleEnum.Reviver) && Role.GetRole<Reviver>(x).UsedRevive && x.PlayerId == body.ParentId).ToList();
                    if (matches != null)
                    {
                        var modifiers = Modifier.GetModifiers(Utils.PlayerById(body.ParentId));
                        if (modifiers != null && modifiers.Any(x => x is IVisualAlteration))
                        {
                            var modifier = modifiers.FirstOrDefault(x => x is IVisualAlteration);
                            if (modifier is IVisualAlteration alteration)
                            {
                                alteration.TryGetModifiedAppearance(out VisualAppearance appearance);
                                body.transform.localScale = appearance.SizeFactor;
                            }
                        }
                        else
                        {
                            body.transform.localScale = playerBindings[body.ParentId].GetDefaultAppearance().SizeFactor;
                        }
                    }
                    else if (!body.IsDouble()) body.transform.localScale = playerBindings[body.ParentId].GetAppearance().SizeFactor;
                    else
                    {
                        var modifiers = Modifier.GetModifiers(Utils.PlayerById(body.ParentId));
                        if (modifiers != null && modifiers.Any(x => x is IVisualAlteration))
                        {
                            var modifier = modifiers.FirstOrDefault(x => x is IVisualAlteration);
                            if (modifier is IVisualAlteration alteration)
                            {
                                alteration.TryGetModifiedAppearance(out VisualAppearance appearance);
                                body.transform.localScale = appearance.SizeFactor;
                            }
                        }
                    }
                } catch {
                }
            }
        }
    }
}