using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ConverterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ConverterHudManagerUpdate
    {
        public static byte DontRevive = byte.MaxValue;
        public static void Postfix(HudManager __instance)
        {
            var player = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Converter)) return;

            // Get the Converter role instance
            var converter = Role.GetRole<Converter>(PlayerControl.LocalPlayer);

            if (converter.ConvertButton == null)
            {
                converter.ConvertButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                converter.ConvertButton.graphic.enabled = true;
                converter.ConvertButton.gameObject.SetActive(false);
            }

            converter.ConvertButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && converter.AbilityUsed == false && !player.Data.Disconnected);
                    
            converter.ConvertButton.graphic.sprite = TownOfUs.Revive2Sprite;
            converter.ConvertButton.transform.localPosition = new Vector3(-2f, 1f, 0f);

            if (!converter.ConvertButton.isActiveAndEnabled) return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            converter.ConvertButton.SetCoolDown(converter.ConvertTimer(), CustomGameOptions.ConverterCD);

            if (converter.CurrentTarget && converter.CurrentTarget != closestBody)
            {
                foreach (var body in converter.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (closestBody != null && closestBody.ParentId == DontRevive) closestBody = null;
            converter.CurrentTarget = closestBody;
            if (converter.CurrentTarget == null)
            {
                converter.ConvertButton.graphic.color = Palette.DisabledClear;
                converter.ConvertButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }
            var player2 = Utils.PlayerById(converter.CurrentTarget.ParentId);

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                var player3 = Utils.PlayerById(deadBody.ParentId);
                var player3Role = Role.GetRole(player3);
                if (closestBody != null && deadBody.ParentId == closestBody.ParentId)
                {
                    if ((!player3.Is(RoleEnum.Astral) || Role.GetRole<Astral>(player3).Enabled != true)
                    && converter.CurrentTarget && converter.ConvertButton.enabled && player2.Is(Faction.Crewmates)
                    && player3Role.InfectionState != 4)
                    {
                        SpriteRenderer component = null;
                        foreach (var body in converter.CurrentTarget.bodyRenderers) component = body;
                        component.material.SetFloat("_Outline", 1f);
                        component.material.SetColor("_OutlineColor", Color.red);
                        converter.ConvertButton.graphic.color = Palette.EnabledColor;
                        converter.ConvertButton.graphic.material.SetFloat("_Desat", 0f);
                        return;
                    }
                }
            }

            converter.ConvertButton.graphic.color = Palette.DisabledClear;
            converter.ConvertButton.graphic.material.SetFloat("_Desat", 1f);

            return;
            
        }
    }
}