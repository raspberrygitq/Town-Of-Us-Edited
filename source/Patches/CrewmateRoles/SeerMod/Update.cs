using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Seer seer)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            if ((player.Is(Faction.Crewmates) && !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Deputy) || player.Is(RoleEnum.Fighter) || player.Is(RoleEnum.Avenger) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Jailor) || player.Is(RoleEnum.Knight))) ||
                            ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy) || player.Is(RoleEnum.Fighter) || player.Is(RoleEnum.Avenger) || player.Is(RoleEnum.Jailor) || player.Is(RoleEnum.Knight)) && !CustomGameOptions.CrewKillingRed) ||
                            (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                            (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                            (player.Is(Faction.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                            {
                                state.NameText.color = Color.green;
                            }
                            else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    if ((traitor.formerRole == RoleEnum.Sheriff || traitor.formerRole == RoleEnum.Vigilante ||
                                        traitor.formerRole == RoleEnum.Veteran || traitor.formerRole == RoleEnum.Hunter ||
                                        traitor.formerRole == RoleEnum.Jailor) && CustomGameOptions.CrewKillingRed) state.NameText.color = Color.red;
                                    else state.NameText.color = Color.green;
                                }
                            }
                            else
                            {
                                state.NameText.color = Color.red;
                            }
                            break;
                    }
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)

        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, seer);

            if (!PlayerControl.LocalPlayer.IsHypnotised() && !Utils.CommsCamouflaged())
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!seer.Investigated.Contains(player.PlayerId)) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            var colour = Color.red;
                            if ((player.Is(Faction.Crewmates) && !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Deputy) || player.Is(RoleEnum.Fighter) || player.Is(RoleEnum.Avenger) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Jailor))) ||
                                ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Deputy) || player.Is(RoleEnum.Fighter) || player.Is(RoleEnum.Avenger) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Jailor)) && !CustomGameOptions.CrewKillingRed) ||
                                (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                                (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                                (player.Is(Faction.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                            {
                                colour = Color.green;
                            }
                            else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    if ((traitor.formerRole == RoleEnum.Sheriff || traitor.formerRole == RoleEnum.Vigilante ||
                                        traitor.formerRole == RoleEnum.Veteran || traitor.formerRole == RoleEnum.Hunter ||
                                        traitor.formerRole == RoleEnum.Jailor) && CustomGameOptions.CrewKillingRed) colour = Color.red;
                                    else colour = Color.green;
                                }
                            }

                            if (player.Is(ModifierEnum.Shy)) colour.a = Modifier.GetModifier<Shy>(player).Opacity;
                            player.nameText().color = colour;

                            break;
                    }
                }
            }
        }
    }
}