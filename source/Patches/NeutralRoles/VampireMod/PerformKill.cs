using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using System.Linq;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Roles.Cultist;
using TownOfUs.Patches.ScreenEffects;

namespace TownOfUs.NeutralRoles.VampireMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Bite
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vampire);
            if (!flag) return true;
            var role = Role.GetRole<Vampire>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.Cooldown > 0) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            if (__instance == role.BiteButton)
            {
                var vamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire)).ToList();
                foreach (var phantom in Role.GetRoles(RoleEnum.Phantom))
                {
                    var phantomRole = (Phantom)phantom;
                    if (phantomRole.formerRole == RoleEnum.Vampire) vamps.Add(phantomRole.Player);
                }
                var aliveVamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (role.ClosestPlayer.Is(RoleEnum.VampireHunter))
                {
                    role.Cooldown = CustomGameOptions.BiteCd;
                    return false;
                }
                if (PlayerControl.LocalPlayer.IsJailed()) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                else if ((role.ClosestPlayer.Is(Faction.Crewmates) || (role.ClosestPlayer.Is(Faction.NeutralBenign)
                    && CustomGameOptions.CanBiteNeutralBenign) || (role.ClosestPlayer.Is(Faction.NeutralEvil)
                    && CustomGameOptions.CanBiteNeutralEvil)) &&
                    aliveVamps.Count == 1 && vamps.Count < CustomGameOptions.MaxVampiresPerGame)
                {
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (interact[4] == true)
                    {
                        Convert(role.ClosestPlayer);
                        Utils.Rpc(CustomRPC.Bite, role.ClosestPlayer.PlayerId);
                        role.Cooldown = CustomGameOptions.BiteCd;
                        return false;
                    }
                    if (interact[0] == true)
                    {
                        role.Cooldown = CustomGameOptions.ProtectKCReset;
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        role.Cooldown = CustomGameOptions.VestKCReset;
                        return false;
                    }
                    else if (interact[3] == true) return false;
                    return false;
                }
                else
                {
                    role.Cooldown = CustomGameOptions.BiteCd;
                    return false;
                }
            }
            else
            {
                if (PlayerControl.LocalPlayer.IsJailed()) return false;
                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
                    return false;
                }
                else if (role.ClosestPlayer.Is(RoleEnum.PotionMaster) && Role.GetRole<PotionMaster>(role.ClosestPlayer).UsingPotion
                && Role.GetRole<PotionMaster>(role.ClosestPlayer).Potion == "Shield")
                {
                    role.Cooldown = CustomGameOptions.PotionKCDReset;
                    return false;
                }
                if (role.ClosestPlayer.IsGuarded2())
                {
                    role.Cooldown = CustomGameOptions.GuardKCReset;
                    return false; 
                }
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[4] == true) return false;
                else if (interact[0] == true)
                {
                    role.Cooldown = CustomGameOptions.BiteCd;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.Cooldown = CustomGameOptions.ProtectKCReset;
                return false;
                }
                else if (interact[2] == true)
                {
                    role.Cooldown = CustomGameOptions.VestKCReset;
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }

        public static void Convert(PlayerControl newVamp)
        {
            var oldRole = Role.GetRole(newVamp);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);

            if (newVamp.Is(RoleEnum.Snitch))
            {
                var snitch = Role.GetRole<Snitch>(newVamp);
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            if (newVamp == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

            if (newVamp.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(newVamp);
                ga.UnProtect();
            }

            if (newVamp.Is(RoleEnum.Chameleon))
            {
                var chamRole = Role.GetRole<Chameleon>(newVamp);
                if (chamRole.IsSwooped)
                chamRole.UnSwoop();
                Utils.Rpc(CustomRPC.ChameleonUnSwoop, newVamp.PlayerId);
            }

            if (newVamp.Is(RoleEnum.Medium))
            {
                var medRole = Role.GetRole<Medium>(newVamp);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (PlayerControl.LocalPlayer == newVamp)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Knight)) HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                {
                    var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    UnityEngine.Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Knight))
                {
                    var knightRole = Role.GetRole<Knight>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(knightRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(transporterRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                {
                    var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(trapperRole.UsesText);
                    trapperRole.traps.ClearTraps();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Doctor))
                {
                    var docRole = Role.GetRole<Doctor>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(docRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                {
                    var detecRole = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                    detecRole.ExamineButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                {
                    var aurialRole = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                    aurialRole.SenseArrows.Values.DestroyAll();
                    aurialRole.SenseArrows.Clear();
                    DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
                {
                    var survRole = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(survRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var gaRole = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(gaRole.UsesText);
                }
            }

            Role.RoleDictionary.Remove(newVamp.PlayerId);

            if (PlayerControl.LocalPlayer == newVamp)
            {
                var role = new Vampire(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.SpawnedAs = false;
                role.OldRole = oldRole;
                role.RegenTask();
            }
            else
            {
                var role = new Vampire(newVamp);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.SpawnedAs = false;
                role.OldRole = oldRole;
            }

            if (CustomGameOptions.NewVampCanAssassin
            && !CustomGameOptions.AssassinImpostorRole) new Roles.Modifiers.Assassin(newVamp);
        }
    }
}
