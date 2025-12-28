using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUsEdited.Roles;
using UnityEngine;

namespace TownOfUsEdited.ImpostorRoles.WraithMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUsEdited.Arrow;
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Wraith)) return;
            var role = Role.GetRole<Wraith>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.WraithTasksRemainingAlert && !role.Caught)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Wraith))
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
                if (__instance == PlayerControl.LocalPlayer)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    var toChooseFromPlayer = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.Madmates) && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(RoleEnum.Pestilence)).ToList();
                    var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromPlayer.Count);
                    var pc = toChooseFromPlayer[rand];
                    Utils.MurderPlayer(role.Player, pc, false);
                    Utils.Rpc(CustomRPC.WraithKill, role.Player.PlayerId, pc.PlayerId);
                }
                else
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                role.CompletedTasks = true;
                role.Caught = true;
            }
        }
    }
}