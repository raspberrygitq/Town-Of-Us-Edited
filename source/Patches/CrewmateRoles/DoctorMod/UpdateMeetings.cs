using HarmonyLib;
using TownOfUsEdited.Roles;
using System.Linq;
using AmongUs.QuickChat;

namespace TownOfUsEdited.CrewmateRoles.DoctorMod
{
    public class ReviveMeetingUpdate
    {
        [HarmonyPatch]
        public class StopChatting
        {
            [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
            public static bool Prefix()
            {
                if (CustomGameOptions.GameMode == GameMode.Chaos) return true;
                var doctors = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Doctor && x.Player != null).Cast<Doctor>();
                foreach (var role in doctors)
                {
                    var revived = role.RevivedList;
                    foreach (var player in revived)
                    {
                        if (!PlayerControl.LocalPlayer.Data.IsDead && player.PlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            [HarmonyPatch(typeof(QuickChatMenu), nameof(QuickChatMenu.CanSend), MethodType.Getter)]
            
            public static bool Prefix(ref bool __result)
            {
                if (CustomGameOptions.GameMode == GameMode.Chaos) return true;
                var doctors = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Doctor && x.Player != null).Cast<Doctor>();
                foreach (var role in doctors)
                {
                    var revived = role.RevivedList;
                    foreach (var player in revived)
                    {
                        if (!PlayerControl.LocalPlayer.Data.IsDead && player.PlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                        {
                        __result = false;
                        return false;
                        }
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
            public class AddChatText
            {
                public static void Postfix(ChatController __instance)
                {
                    var doctors = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Doctor && x.Player != null).Cast<Doctor>();
                    foreach (var role in doctors)
                    {
                        var revived = role.RevivedList;
                        foreach (var player in revived)
                        {
                            if (!PlayerControl.LocalPlayer.Data.IsDead && player.PlayerId == PlayerControl.LocalPlayer.PlayerId &&
                            AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                            {
                                __instance.sendRateMessageText.gameObject.SetActive(true);
                                __instance.sendRateMessageText.color = Patches.Colors.Doctor;
			                    __instance.sendRateMessageText.text = "You have been resurected, you can no longer chat.";
                            }
                        }
                    }
                }
            }
        }
    }
}