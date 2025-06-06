using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.DoctorMod
{
    public class KillButtonTarget
    {

        public static void SetTarget(KillButton __instance, DeadBody target, Doctor role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            role.CurrentTarget = target;
            var label = __instance.buttonLabelText;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.green);
                role.DragDropButton.graphic.color = Palette.EnabledColor;
                role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                label.color = Palette.EnabledColor;
                label.material.SetFloat("_Desat", 0f);
                return;
            }

            role.DragDropButton.graphic.color = Palette.DisabledClear;
            role.DragDropButton.graphic.material.SetFloat("_Desat", 1f);
            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
            label.color = Palette.DisabledClear;
            label.material.SetFloat("_Desat", 1f);
        }
    }
}