using HarmonyLib;
using System;
using TownOfUsEdited.Patches;
using TownOfUsEdited.CustomOption;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;

namespace TownOfUsEdited
{
    [HarmonyPatch]
    class RandomMap
    {
        public static byte previousMap;
        public static float vision;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;
        public static bool LevelImpLoaded => IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out _);

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        [HarmonyPrefix]
        public static bool Prefix(GameStartManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                previousMap = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                vision = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                commonTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
                shortTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
                longTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
                byte map = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                if (CustomGameOptions.RandomMapEnabled)
                {
                    map = GetRandomMap();
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = map;
                }
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Tracker, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Noisemaker, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Phantom, 0, 0);
                Utils.Rpc(CustomRPC.SetSettings, map);
                AdjustSettings(map);
            }
            return true;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        [HarmonyPostfix]
        public static void Postfix(AmongUsClient __instance)
        {
            if (__instance.AmHost)
            {
                Reset();
            }
        }

        public static void Reset()
        {
            if (CustomGameOptions.SmallMapHalfVision && vision != 0) GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = vision;
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1 && CustomGameOptions.AutoAdjustCooldowns) AdjustCooldowns(CustomGameOptions.SmallMapDecreasedCooldown);
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId >= 4 && CustomGameOptions.AutoAdjustCooldowns) AdjustCooldowns(-CustomGameOptions.LargeMapIncreasedCooldown);
            if (CustomGameOptions.RandomMapEnabled) GameOptionsManager.Instance.currentNormalGameOptions.MapId = previousMap;
            if (!(commonTasks == 0 && shortTasks == 0 && longTasks == 0))
            {
                GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = shortTasks;
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTasks;
            }
        }

        public static byte GetRandomMap()
        {
            Random _rnd = new Random();
            float totalWeight = 0;
            totalWeight += CustomGameOptions.RandomMapSkeld;
            totalWeight += CustomGameOptions.RandomMapMira;
            totalWeight += CustomGameOptions.RandomMapPolus;
            totalWeight += CustomGameOptions.RandomMapAirship;
            totalWeight += CustomGameOptions.RandomMapFungle;
            totalWeight += CustomGameOptions.RandomMapSubmerged;
            totalWeight += CustomGameOptions.RandomMapLevelImpostor;

            if (totalWeight == 0) return GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            float randomNumber = _rnd.Next(0, (int)totalWeight);
            if (randomNumber < CustomGameOptions.RandomMapSkeld) return 0;
            randomNumber -= CustomGameOptions.RandomMapSkeld;
            if (randomNumber < CustomGameOptions.RandomMapMira) return 1;
            randomNumber -= CustomGameOptions.RandomMapMira;
            if (randomNumber < CustomGameOptions.RandomMapPolus) return 2;
            randomNumber -= CustomGameOptions.RandomMapPolus;
            if (randomNumber < CustomGameOptions.RandomMapAirship) return 4;
            randomNumber -= CustomGameOptions.RandomMapAirship;
            if (randomNumber < CustomGameOptions.RandomMapFungle) return 5;
            randomNumber -= CustomGameOptions.RandomMapFungle;
            if (SubmergedCompatibility.Loaded && randomNumber < CustomGameOptions.RandomMapSubmerged) return 6;
            randomNumber -= CustomGameOptions.RandomMapSubmerged;
            if (LevelImpLoaded && randomNumber < CustomGameOptions.RandomMapLevelImpostor) return 7;

            return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        }

        public static void AdjustSettings(byte map)
        {
            if (map <= 1)
            {
                if (CustomGameOptions.SmallMapHalfVision) GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod *= 0.5f;
                if (CustomGameOptions.AutoAdjustCooldowns)
                {
                    GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks += CustomGameOptions.SmallMapIncreasedShortTasks;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks += CustomGameOptions.SmallMapIncreasedLongTasks;
                }
            }
            if (map == 1 && CustomGameOptions.AutoAdjustCooldowns) AdjustCooldowns(-CustomGameOptions.SmallMapDecreasedCooldown);
            if (map >= 4)
            {
                if (CustomGameOptions.AutoAdjustCooldowns)
                {
                    GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks -= CustomGameOptions.LargeMapDecreasedShortTasks;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks -= CustomGameOptions.LargeMapDecreasedLongTasks;
                    AdjustCooldowns(CustomGameOptions.LargeMapIncreasedCooldown);
                }
            }
            return;
        }

        public static void AdjustCooldowns(float change)
        {
            Generate.ExamineCooldown.Set((float)Generate.ExamineCooldown.Value + change, false);
            Generate.ZoomCooldown.Set((float)Generate.ZoomCooldown.Value + change, false);
            Generate.ShiftCD.Set((float)Generate.ShiftCD.Value + change, false);
            Generate.TransformCD.Set((float)Generate.TransformCD.Value + change, false);
            Generate.TransformKCD.Set((float)Generate.TransformKCD.Value + change, false);
            Generate.MutantKCD.Set((float)Generate.MutantKCD.Value + change, false);
            Generate.InfectiousCD.Set((float)Generate.InfectiousCD.Value + change, false);
            Generate.SerialKillerKCD.Set((float)Generate.SerialKillerKCD.Value + change, false);
            Generate.DoppelKCD.Set((float)Generate.DoppelKCD.Value + change, false);
            Generate.PoisonCooldown.Set((float)Generate.PoisonCooldown.Value + change, false);
            Generate.SeerCooldown.Set((float)Generate.SeerCooldown.Value + change, false);
            Generate.SheriffKillCd.Set((float)Generate.SheriffKillCd.Value + change, false);
            Generate.AlertCooldown.Set((float)Generate.AlertCooldown.Value + change, false);
            Generate.GhostCooldown.Set((float)Generate.GhostCooldown.Value + change, false);
            Generate.WatchCooldown.Set((float)Generate.WatchCooldown.Value + change, false);
            Generate.TransportCooldown.Set((float)Generate.TransportCooldown.Value + change, false);
            Generate.ProtectCd.Set((float)Generate.ProtectCd.Value + change, false);
            Generate.VestCd.Set((float)Generate.VestCd.Value + change, false);
            Generate.DouseCooldown.Set((float)Generate.DouseCooldown.Value + change, false);
            Generate.InfectCooldown.Set((float)Generate.InfectCooldown.Value + change, false);
            Generate.PestKillCooldown.Set((float)Generate.PestKillCooldown.Value + change, false);
            Generate.MimicCooldownOption.Set((float)Generate.MimicCooldownOption.Value + change, false);
            Generate.HackCooldownOption.Set((float)Generate.HackCooldownOption.Value + change, false);
            Generate.GlitchKillCooldownOption.Set((float)Generate.GlitchKillCooldownOption.Value + change, false);
            Generate.RampageCooldown.Set((float)Generate.RampageCooldown.Value + change, false);
            Generate.GrenadeCooldown.Set((float)Generate.GrenadeCooldown.Value + change, false);
            Generate.MorphlingCooldown.Set((float)Generate.MorphlingCooldown.Value + change, false);
            Generate.SwoopCooldown.Set((float)Generate.SwoopCooldown.Value + change, false);
            Generate.MineCooldown.Set((float)Generate.MineCooldown.Value + change, false);
            Generate.DragCooldown.Set((float)Generate.DragCooldown.Value + change, false);
            Generate.EscapeCooldown.Set((float)Generate.EscapeCooldown.Value + change, false);
            Generate.JuggKillCooldown.Set((float)Generate.JuggKillCooldown.Value + change, false);
            Generate.BiteCooldown.Set((float)Generate.BiteCooldown.Value + change, false);
            Generate.StakeCooldown.Set((float)Generate.StakeCooldown.Value + change, false);
            Generate.ChargeUpDuration.Set((float)Generate.ChargeUpDuration.Value + change, false);
            Generate.AbilityCooldown.Set((float)Generate.AbilityCooldown.Value + change, false);
            Generate.CampaignCooldown.Set((float)Generate.CampaignCooldown.Value + change, false);
            Generate.HypnotiseCooldown.Set((float)Generate.HypnotiseCooldown.Value + change, false);
            Generate.JailCD.Set((float)Generate.JailCD.Value + change, false);
            Generate.ReapCooldown.Set((float)Generate.ReapCooldown.Value + change, false);
            Generate.FlushCooldown.Set((float)Generate.FlushCooldown.Value + change, false);
            Generate.BlindCooldown.Set((float)Generate.BlindCooldown.Value + change, false);
            Generate.BlindDuration.Set((float)Generate.BlindDuration.Value + change, false);
            Generate.FreezeCooldown.Set((float)Generate.FreezeCooldown.Value + change, false);
            Generate.FreezeDuration.Set((float)Generate.FreezeDuration.Value + change, false);
            Generate.BarrierCd.Set((float)Generate.BarrierCd.Value + change, false);
            GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown += change;
            if (change % 5 != 0)
            {
                if (change > 0) change -= 2.5f;
                else if (change < 0) change += 2.5f;
            }
            GameOptionsManager.Instance.currentNormalGameOptions.EmergencyCooldown += (int)change;
            return;
        }
    }
}