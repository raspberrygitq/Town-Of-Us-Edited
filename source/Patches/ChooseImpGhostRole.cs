using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.ImpostorRoles.BlinderMod;
using TownOfUs.ImpostorRoles.FreezerMod;
using TownOfUs.ImpostorRoles.SpiritMod;

namespace TownOfUs.ChooseImpGhostRole
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    public class PickImpRole
    {
        public static List<string> GhostRoles = new List<string>{};
        public static bool ranlist = false;

        public static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            PickGhostRole(exiled);
        }

        public static void PickGhostRole(PlayerControl exiled)
        {
            if (CustomGameOptions.GameMode != GameMode.Classic && CustomGameOptions.GameMode != GameMode.AllAny) return;
            if (!AmongUsClient.Instance.AmHost) return;
            var random = new System.Random();
            if (RpcHandling.Check(CustomGameOptions.SpiritOn) && SetSpirit.WillBeSpirit == null && !ranlist && !GhostRoles.Contains("Spirit")) GhostRoles.Add("Spirit");
            if (RpcHandling.Check(CustomGameOptions.BlinderOn) && SetBlinder.WillBeBlinder == null && !ranlist && !GhostRoles.Contains("Blinder")) GhostRoles.Add("Blinder");
            if (RpcHandling.Check(CustomGameOptions.FreezerOn) && SetFreezer.WillBeFreezer == null && !ranlist && !GhostRoles.Contains("Freezer")) GhostRoles.Add("Freezer");
            if (GhostRoles.Count <= 0)
            {
                ranlist = false;
                return;
            }
            int index = random.Next(GhostRoles.Count);
            var chosenRole = GhostRoles[index];
            GhostRoles.RemoveAt(index);
            var toChooseFromImpo = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Is(ModifierEnum.Lover) && (x.Data.IsDead || x == exiled)).ToList();
            toChooseFromImpo.RemoveAll(player => SetSpirit.WillBeSpirit == player);
            toChooseFromImpo.RemoveAll(player => SetBlinder.WillBeBlinder == player);
            toChooseFromImpo.RemoveAll(player => SetFreezer.WillBeFreezer == player);
            // Set the Spirit, if there is one enabled.
            if (chosenRole == "Spirit" && toChooseFromImpo.Count != 0)
            {
                var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromImpo.Count);
                var pc = toChooseFromImpo[rand];

                SetSpirit.WillBeSpirit = pc;

                Utils.Rpc(CustomRPC.SetSpirit, pc.PlayerId);
            }
            // Set the Blinder, if there is one enabled.
            else if (chosenRole == "Blinder" && toChooseFromImpo.Count != 0)
            {
                var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromImpo.Count);
                var pc = toChooseFromImpo[rand];

                SetBlinder.WillBeBlinder = pc;

                Utils.Rpc(CustomRPC.SetBlinder, pc.PlayerId);
            }
            // Set the Freezer, if there is one enabled.
            else if (chosenRole == "Freezer" && toChooseFromImpo.Count != 0)
            {
                var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromImpo.Count);
                var pc = toChooseFromImpo[rand];

                SetFreezer.WillBeFreezer = pc;

                Utils.Rpc(CustomRPC.SetFreezer, pc.PlayerId);
            }
            if (GhostRoles.Count >= 1)
            {
                ranlist = true;
                GhostRoles.Clear();
                PickGhostRole(exiled);
            }
            else
            {
                ranlist = false;
                GhostRoles.Clear();
            }
        }
    }
}