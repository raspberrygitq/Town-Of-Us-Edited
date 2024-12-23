using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] NetworkedPlayerInfo voterPlayer,
            [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
        {
            SpriteRenderer spriteRenderer = Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);
            var player = Utils.PlayerByData(voterPlayer);

            if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes && (!CustomGameOptions.DeadSeeRoles || !PlayerControl.LocalPlayer.Data.IsDead)
            && !PlayerControl.LocalPlayer.Is(ModifierEnum.Spotter))
            {
                //PlayerControl.SetPlayerMaterialColors(Palette.DisabledGrey, spriteRenderer);
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            }
            else if (player.Is(RoleEnum.Doppelganger) && Role.GetRole<Doppelganger>(player).TransformedPlayer != null &&
            !voterPlayer.IsDead)
            {
                var TargetData = Role.GetRole<Doppelganger>(player).TransformedPlayer.Data;
                PlayerMaterial.SetColors(TargetData.DefaultOutfit.ColorId, spriteRenderer);
            }
            else if (player.Is(RoleEnum.Reviver) && Role.GetRole<Reviver>(player).UsedRevive == true &&
            !voterPlayer.IsDead)
            {
                var TargetData = Role.GetRole<Reviver>(player).RevivedPlayer.Data;
                PlayerMaterial.SetColors(TargetData.DefaultOutfit.ColorId, spriteRenderer);
            }
            else
            {
                //PlayerControl.SetPlayerMaterialColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }
            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;
            var component = parent.GetComponent<PlayerVoteArea>();
            if (component != null)
            {
                spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
            }

            var Base = __instance as MonoBehaviour;
            Base.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }
}