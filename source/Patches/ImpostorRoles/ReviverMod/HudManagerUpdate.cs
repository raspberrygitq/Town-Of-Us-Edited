using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.ReviverMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ReviverHudManagerUpdate
    {
        public static byte DontRevive = byte.MaxValue;
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Reviver)) return;

            // Get the Reviver role instance
            var role = Role.GetRole<Reviver>(PlayerControl.LocalPlayer);

            if (role.ReviveButton == null)
            {
                role.ReviveButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ReviveButton.graphic.enabled = true;
                role.ReviveButton.gameObject.SetActive(false);
            }

            role.ReviveButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && role.UsedRevive == false && !player.Data.Disconnected
                    && role.CanRevive == true);
            role.ReviveButton.buttonLabelText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                    && role.UsedRevive == false && !player.Data.Disconnected
                    && role.CanRevive == true);

            role.ReviveButton.graphic.sprite = TownOfUsEdited.Revive2Sprite;
            role.ReviveButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            role.ReviveButton.buttonLabelText.text = "Ressurect";

            if (!role.ReviveButton.isActiveAndEnabled) return;

            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            if (role.CurrentTarget && role.CurrentTarget != closestBody)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (closestBody != null && closestBody.ParentId == DontRevive) closestBody = null;
            role.CurrentTarget = closestBody;
            if (role.CurrentTarget == null)
            {
                role.ReviveButton.graphic.color = Palette.DisabledClear;
                role.ReviveButton.graphic.material.SetFloat("_Desat", 1f);
                role.ReviveButton.buttonLabelText.color = Palette.DisabledClear;
                role.ReviveButton.buttonLabelText.material.SetFloat("_Desat", 1f);
                return;
            }
            var player2 = Utils.PlayerById(role.CurrentTarget.ParentId);
            if (role.CurrentTarget && role.ReviveButton.enabled
            && player2 != PlayerControl.LocalPlayer &&
            !role.CurrentTarget.IsDouble())
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.red);
                role.ReviveButton.graphic.color = Palette.EnabledColor;
                role.ReviveButton.graphic.material.SetFloat("_Desat", 0f);
                role.ReviveButton.buttonLabelText.color = Palette.EnabledColor;
                role.ReviveButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                return;
            }

            role.ReviveButton.graphic.color = Palette.DisabledClear;
            role.ReviveButton.graphic.material.SetFloat("_Desat", 1f);
            role.ReviveButton.buttonLabelText.color = Palette.DisabledClear;
            role.ReviveButton.buttonLabelText.material.SetFloat("_Desat", 1f);

            role.ReviveButton.SetCoolDown(0f, 1f);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateReviverKillButton
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Reviver)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = Role.GetRole<Reviver>(PlayerControl.LocalPlayer);

            if (!role.UsedRevive) return;

            var aliveimps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(RoleEnum.Mafioso) && !x.Is(RoleEnum.Reviver) && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (aliveimps.Count > 0)
            {
                __instance.KillButton.Hide();
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix()
        {
            foreach (var reviver in Role.GetRoles(RoleEnum.Reviver))
            {
                var role = Role.GetRole<Reviver>(reviver.Player);
                if (role.Player.Data.IsDead && role.UsedRevive == false && role.CanRevive == true)
                {
                    role.CanRevive = false;
                }
            }
        }
    }
}