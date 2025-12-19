using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Undertaker role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                role.DragDropButton.graphic.color = Palette.EnabledColor;
                role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
                role.DragDropButton.buttonLabelText.color = Palette.EnabledColor;
                role.DragDropButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
            role.DragDropButton.graphic.color = Palette.DisabledClear;
            role.DragDropButton.graphic.material.SetFloat("_Desat", 1f);
            role.DragDropButton.buttonLabelText.color = Palette.DisabledClear;
            role.DragDropButton.buttonLabelText.material.SetFloat("_Desat", 1f);
        }
    }
}