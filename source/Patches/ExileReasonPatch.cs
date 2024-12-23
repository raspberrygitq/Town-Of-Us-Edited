using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches
{    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class CustomExileController
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) 
                return;

            var player = exiled.Object;

            if (player.AmOwner)
            {
                var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (var i = 0; i < player.myTasks.Count; i++)
                    {
                        var playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);
            }

            var role = Role.GetRole(player);
            if (role == null) 
                return;

            role.DeathReason = DeathReasons.Exiled;
        }
    }
}
