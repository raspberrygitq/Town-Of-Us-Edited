using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.SpiritMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Spirit)) return;
            var role = Role.GetRole<Spirit>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.SpiritTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Spirit))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                }
                else
                {
                    role.Revealed = true;
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.Arrows.Add(arrow);
                }
            }

            if (tasksLeft == 0 && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Spirit))
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                else
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                role.CompletedTasks = true;
                var toChooseFromPlayer = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromPlayer.Count);
                var pc = toChooseFromPlayer[rand];
                Utils.MurderPlayer(role.Player, pc, false);
                role.Caught = true;
            }
        }
    }
}