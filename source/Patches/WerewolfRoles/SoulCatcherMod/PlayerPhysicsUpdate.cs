using HarmonyLib;
using UnityEngine;
using System;
using TownOfUs.Extensions;

namespace TownOfUs.WerewolfRoles.SoulCatcherMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
    public static class PlayerPhysics_LateUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SoulCatcher)) return;
            if (MeetingHud.Instance) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (__instance.myPlayer.Data.IsDead)
            {
                __instance.myPlayer.Visible = true;
                __instance.myPlayer.SetOutfit(CustomPlayerOutfitType.Camouflage, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = __instance.myPlayer.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
                __instance.myPlayer.nameText().color = Color.white;
                PlayerMaterial.SetColors(Color.grey, __instance.myPlayer.myRend());
            }
        }
    }
}