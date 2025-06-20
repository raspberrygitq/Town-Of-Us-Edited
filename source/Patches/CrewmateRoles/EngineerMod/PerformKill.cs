using HarmonyLib;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.CrewmateRoles.EngineerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Engineer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system == null) return false;
            var sabActive = system.AnyActive;
            if (!sabActive) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            role.UsesLeft -= 1;
            Utils.Rpc(CustomRPC.EngineerFix, PlayerControl.LocalPlayer.NetId);
            int mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) mapId = AmongUsClient.Instance.TutorialMapId;
            switch (mapId)
            {
                case 0:
                case 3:
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixComms();
                    var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor1.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen1.IsActive) return FixOxygen();
                    var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights1.IsActive) return FixLights(lights1);

                    break;
                case 1:
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixMiraComms();
                    var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor2.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen2.IsActive) return FixOxygen();
                    var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights2.IsActive) return FixLights(lights2);
                    break;

                case 2:
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixComms();
                    var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (seismic.IsActive) return FixReactor(SystemTypes.Laboratory);
                    var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights3.IsActive) return FixLights(lights3);
                    break;
                case 4:
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixComms();
                    var reactor = ShipStatus.Instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                    if (reactor.IsActive) return FixAirshipReactor();
                    var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights4.IsActive) return FixLights(lights4);
                    break;
                case 5:
                    var reactor7 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor7.IsActive) return FixReactor(SystemTypes.Reactor);
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixMiraComms();
                    var mushroom = ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                    if (mushroom.IsActive)
                    {
                        mushroom.currentSecondsUntilHeal = 0.1f;
                        return false;
                    }
                    break;
                case 6:
                    var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor5.IsActive) return FixReactor(SystemTypes.Reactor);
                    var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights5.IsActive) return FixLights(lights5);
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixComms();
                    foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (i.TaskType == Patches.SubmergedCompatibility.RetrieveOxygenMask)
                        {
                            return FixSubOxygen();
                        }
                    }
                    break;
                case 7:
                    if (PlayerControl.LocalPlayer.AreCommsAffected()) return FixComms();
                    var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor6.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen6.IsActive) return FixOxygen();
                    var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights6.IsActive) return FixLights(lights6);
                    break;
            }

            

            return false;
        }

        private static bool FixComms()
        {
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 0);
            return false;
        }

        private static bool FixMiraComms()
        {
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        private static bool FixAirshipReactor()
        {
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
            return false;
        }

        private static bool FixReactor(SystemTypes system)
        {
            ShipStatus.Instance.RpcUpdateSystem(system, 16);
            return false;
        }

        private static bool FixOxygen()
        {
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);
            return false;
        }

        private static bool FixSubOxygen()
        {
            Patches.SubmergedCompatibility.RepairOxygen();

            Utils.Rpc(CustomRPC.SubmergedFixOxygen, PlayerControl.LocalPlayer.NetId);

            return false;
        }

        private static bool FixLights(SwitchSystem lights)
        {
            Utils.Rpc(CustomRPC.FixLights);

            lights.ActualSwitches = lights.ExpectedSwitches;

            return false;
        }
    }
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public class PerformSabotage
    {
        public static bool Prefix(SabotageButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(Faction.Madmates) && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) return true;
            if (__instance != DestroyableSingleton<HudManager>.Instance.SabotageButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(Faction.Impostors);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (MeetingHud.Instance) return false;
            DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
            {
                Mode = MapOptions.Modes.Sabotage
            });
            return false;
        }
    }

    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetMapOptions))]
    public class MapPatch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref MapOptions __result)
        {
            if (!PlayerControl.LocalPlayer.Is(Faction.Madmates)) return;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Engineer);
            if (!flag) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (MeetingHud.Instance) return;

            __result = new MapOptions
		    {
			    Mode = MapOptions.Modes.Sabotage
		    };
        }
    }
}