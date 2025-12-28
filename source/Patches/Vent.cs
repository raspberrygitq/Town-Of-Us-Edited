using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerVentPatch
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if(__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.CachedPlayerData) && !MeetingHud.Instance;
            if(active != __instance.ImpostorVentButton.gameObject.active)
            __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanVent(PlayerControl player, NetworkedPlayerInfo playerInfo)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;

            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2 && !player.Is(RoleEnum.Haunter) && !player.Is(RoleEnum.Phantom) && !player.Is(RoleEnum.Wraith)
            && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                if (player.inVent)
                {
                    player.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    player.MyPhysics.ExitAllVents();
                }
                return false;
            }

            if (player.inVent) return true;

            if (playerInfo.IsDead)
                return false;

            var aliveimp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead).ToList();

            if (player.Is(RoleEnum.Morphling) && !CustomGameOptions.MorphlingVent
                || player.Is(RoleEnum.Poisoner) && !CustomGameOptions.PoisonerVent
                || player.Is(RoleEnum.Swooper) && !CustomGameOptions.SwooperVent
                || player.Is(RoleEnum.Grenadier) && !CustomGameOptions.GrenadierVent
                || player.Is(RoleEnum.Undertaker) && !CustomGameOptions.UndertakerVent
                || player.Is(RoleEnum.Escapist) && !CustomGameOptions.EscapistVent
                || player.Is(RoleEnum.Bomber) && !CustomGameOptions.BomberVent
                || player.Is(RoleEnum.Noclip) && !CustomGameOptions.NoclipVent
                || player.Is(RoleEnum.Mafioso) && aliveimp.Count > 1
                || player.Is(RoleEnum.Manipulator) && Role.GetRole<Manipulator>(player).UsingManipulation
                || (player.Is(RoleEnum.Undertaker) && Role.GetRole<Undertaker>(player).CurrentlyDragging != null && !CustomGameOptions.UndertakerVentWithBody)
                || CustomGameOptions.GameMode == GameMode.Chaos)
                return false;

            if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Coven) || player.Is(RoleEnum.CovenLeader) || 
                player.Is(RoleEnum.Ritualist) || player.Is(RoleEnum.Spiritualist) || player.Is(RoleEnum.VoodooMaster) ||
                (player.Is(RoleEnum.PotionMaster) && Role.GetRole<PotionMaster>(player).UsingPotion && Role.GetRole<PotionMaster>(player).Potion == "Strength") ||
                player.Is(Faction.Madmates) || player.Is(RoleEnum.Paranoïac) ||
                (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) || 
                (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) ||
                (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) || 
                (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) ||
                (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent) || 
                (player.Is(RoleEnum.SerialKiller) && CustomGameOptions.SerialKillerVent) ||
                (player.Is(RoleEnum.Player) && !CustomGameOptions.BattleDisableVent) || 
                (player.Is(RoleEnum.Terrorist) && CustomGameOptions.TerroristVent) ||
                (player.Is(RoleEnum.Vulture) && CustomGameOptions.VultureVent) || 
                (player.Is(RoleEnum.Infectious) && CustomGameOptions.InfectiousVent) ||
                (player.Is(RoleEnum.Doppelganger) && CustomGameOptions.DoppelVent) || 
                (player.Is(RoleEnum.SoulCollector) && CustomGameOptions.SCVent) ||
                (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) ||
                (player.Is(RoleEnum.HexMaster) && CustomGameOptions.HexMasterVent))
                return true;

            if (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent)
            {
                var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                if (role.Rampaged) return true;
            }

            if (player.Is(RoleEnum.Mutant) && CustomGameOptions.MutantVent)
            {
                var role = Role.GetRole<Mutant>(PlayerControl.LocalPlayer);
                if (role.IsTransformed) return true;
            }

            return playerInfo.IsImpostor();
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;

            if (Utils.Rewinding())
            {
                canUse = true;
                couldUse = true;
                return;
            }

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) couldUse = CanVent(playerControl, playerInfo) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && playerControl.Data.IsImpostor()) couldUse = false;
            else couldUse = canUse;

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            {
                couldUse = false;
            }

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Plumber))
            {
                var plumber = (Plumber)role;
                if (plumber.VentsBlocked.Contains((byte)__instance.Id) && (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter) || Role.GetRole<Haunter>(PlayerControl.LocalPlayer).Caught)
                && (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) || Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
                && (!PlayerControl.LocalPlayer.Is(RoleEnum.Wraith) || Role.GetRole<Wraith>(PlayerControl.LocalPlayer).Caught))
                {
                    couldUse = false;
                }
            }

            canUse = couldUse;

            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);

                if (__instance.Id == 14 && SubmergedCompatibility.isSubmerged())
                    canUse &= (double)num <= (double)__instance.UsableDistance;
                else
                    canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
                
            }

            __result = num;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class JesterEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                return false;
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Paranoïac) && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
                return false;
            return true;
        }
    }
}