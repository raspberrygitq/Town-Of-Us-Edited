using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);
                    var flag = PlayerControl.LocalPlayer.Is(Faction.Madmates);
                    var flag2 = !PlayerControl.LocalPlayer.Data.IsDead || !CustomGameOptions.DeadSeeRoles;
                    if (player.Is(Faction.Impostors) && !player.Is(RoleEnum.Traitor) && !flag)
                        state.NameText.color = Palette.ImpostorRed;
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor && !flag)
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(Faction.NeutralKilling) && CustomGameOptions.SnitchSeesNeutrals && !flag)
                        state.NameText.color = role.Color;
                    if (player.Is(Faction.Coven) && !flag)
                        state.NameText.color = Patches.Colors.Coven;

                    if (player.Is(Faction.Crewmates) && flag && flag2)
                        state.NameText.color = Palette.CrewmateBlue;
                    else if ((player.Is(Faction.NeutralBenign) || player.Is(Faction.NeutralEvil)
                    || player.Is(Faction.NeutralKilling)) && flag && flag2)
                        state.NameText.color = Color.gray;
                    else if (player.Is(Faction.Coven) && flag && flag2)
                        state.NameText.color = Patches.Colors.Coven;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            if (!PlayerControl.LocalPlayer.IsHypnotised() && !PlayerControl.LocalPlayer.Is(Faction.Madmates))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && !player.Is(RoleEnum.Traitor))
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                    {
                        var colour = Palette.ImpostorRed;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    if (player.Is(Faction.Coven))
                    {
                        var colour = Patches.Colors.Coven;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    var playerRole = Role.GetRole(player);
                    if (playerRole.Faction == Faction.NeutralKilling && CustomGameOptions.SnitchSeesNeutrals)
                    {
                        var colour = playerRole.Color;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                }
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Madmates) && !PlayerControl.LocalPlayer.IsHypnotised()
            && (!PlayerControl.LocalPlayer.Data.IsDead || !CustomGameOptions.DeadSeeRoles))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Crewmates))
                    {
                        var colour = Palette.CrewmateBlue;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else if (player.Is(Faction.NeutralKilling) || player.Is(Faction.NeutralBenign) ||
                    player.Is(Faction.NeutralEvil))
                    {
                        var colour = Color.gray;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                    else if (player.Is(Faction.Coven))
                    {
                        var colour = Patches.Colors.Coven;
                        if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                        player.nameText().color = colour;
                    }
                }
            }
        }
    }
}