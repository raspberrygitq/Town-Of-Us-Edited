using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] NetworkedPlayerInfo player,
            ref float __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorLightMod;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewLightMod;
                }
                return false;
            }

            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var playerControl = Utils.PlayerByData(player);
            if (playerControl.IsBlinded())
            {
                __result = 0f;
                return false;
            }

            var switchSystem = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? null : __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
            var t = switchSystem != null ? switchSystem.Value / 255f : 1;

            var playerRole = Role.GetRole(playerControl);
            foreach (var infectious in Role.GetRoles(RoleEnum.Infectious))
            {
                var infectiousRole = (Infectious)infectious;
                if (playerRole != null  && playerRole.InfectionState > 2 && infectiousRole.Infected.Contains(playerControl.PlayerId)
                && !infectiousRole.Player.Data.IsDead)
                {
                    if (!player._object.Is(Faction.Crewmates))
                    {
                        __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod/2;
                    }
                    else if (player._object.Is(RoleEnum.Mayor))
                    {
                        __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius/3, t) *
                            GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                    }
                    else
                    {
                        __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius/2, t) *
                            GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                    }
                    return false;
                }
            }

            if (player.IsImpostor() || player._object.Is(RoleEnum.Glitch) ||
                player._object.Is(RoleEnum.Juggernaut) || player._object.Is(RoleEnum.Pestilence) ||
                (player._object.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision) ||
                (player._object.Is(RoleEnum.Vulture) && CustomGameOptions.VultureImpVision) ||
                (player._object.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoImpVision) ||
                (player._object.Is(RoleEnum.Vampire) && CustomGameOptions.VampImpVision) ||
                (player._object.Is(RoleEnum.SerialKiller) && CustomGameOptions.SkImpVision) ||
                (player._object.Is(Faction.Madmates) && CustomGameOptions.MadmateHasImpoVision)||
                player._object.Is(RoleEnum.WhiteWolf) || player._object.Is(RoleEnum.Player) ||
                player._object.Is(RoleEnum.Terrorist) || player._object.Is(Faction.Coven) ||
                (player._object.Is(RoleEnum.Lighter) && Role.GetRole<Lighter>(player._object).UsingLight) ||
                player._object.Is(RoleEnum.Infectious) || player._object.Is(RoleEnum.Doppelganger))
            {
                __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                return false;
            }
            else if (player._object.Is(RoleEnum.Maul))
            {
                var role = Role.GetRole<Maul>(player._object);
                if (role.Rampaged)
                {
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                    return false;
                }
            }

            else if (player._object.Is(RoleEnum.Mutant))
            {
                var role = Role.GetRole<Mutant>(player._object);
                if (role.IsTransformed == true)
                {
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                    return false;
                }
            }

            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (player._object.Is(ModifierEnum.Torch)) __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                return false;
            }

            if (player._object.Is(ModifierEnum.Torch)) t = 1;

            if (player._object.Is(RoleEnum.Mayor))
            {
                var role = Role.GetRole<Mayor>(player._object);
                if (role.Revealed)
                {
                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius/2, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                    return false;
                }
            }

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            return false;
        }
    }
}