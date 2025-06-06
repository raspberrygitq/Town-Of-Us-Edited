using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.AvengerMod
{
    public class KillButtonTarget
    {
        public static byte DontRevive = byte.MaxValue;

        public static void SetTarget(KillButton __instance, DeadBody target, Avenger role)
        {
            if (role.Avenging) return;
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (target != null && target.ParentId == DontRevive) target = null;
            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.red);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                role.AvengeText.color = Palette.EnabledColor;
                role.AvengeText.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
            role.AvengeText.color = Palette.DisabledClear;
            role.AvengeText.material.SetFloat("_Desat", 1f);
        }
    }
}