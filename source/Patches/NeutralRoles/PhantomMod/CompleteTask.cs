using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUsEdited.Roles;

namespace TownOfUsEdited.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            var role = Role.GetRole<Phantom>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
                if (AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
                {
                    if (AmongUsClient.Instance.AmHost)
                    {
                        Utils.Rpc(CustomRPC.PhantomWin, role.Player.PlayerId);
                        if (CustomGameOptions.PhantomWinEndsGame)
                        {
                            Coroutines.Start(Role.WaitForEnd());
                            PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Phantom Win");
                        }
                        else
                        {
                            role.Caught = true;
                            if (PlayerControl.LocalPlayer == role.Player) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        }
                    }
                }
                else if (!role.Caught)
                {
                    HudManager.Instance.ShowPopUp("Normally, the game would've ended and the Phantom would've won. In Freeplay, nothing happens.");
                    role.Caught = true;
                }
            }
        }
    }
}