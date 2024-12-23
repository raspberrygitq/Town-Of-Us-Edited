using System.Collections;
using TownOfUs.ImpostorRoles.ImpostorMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    public class Coroutine
    {
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");

        public static IEnumerator CleanCoroutine(DeadBody body, Janitor role)
        {
            KillButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
            if (PlayerControl.LocalPlayer == role.Player)
            {
                Role.GetRole(PlayerControl.LocalPlayer).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
            }
            SpriteRenderer renderer = null;
            var player = Utils.PlayerById(body.ParentId);
            if (player.Is(RoleEnum.Astral))
            {
                var astralRole = Role.GetRole<Astral>(player);
                if (astralRole.Enabled)
                {
                    astralRole.TimeRemaining = 0f;
                    astralRole.Enabled = false;
                }
            }
            foreach (var body2 in body.bodyRenderers) renderer = body2;
            var backColor = renderer.material.GetColor(BackColor);
            var bodyColor = renderer.material.GetColor(BodyColor);
            var newColor = new Color(1f, 1f, 1f, 0f);
            for (var i = 0; i < 60; i++)
            {
                if (body == null) yield break;
                renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                yield return null;
            }

            Object.Destroy(body.gameObject);
        }
    }
}