using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUsEdited.CrewmateRoles.InvestigatorMod;
using TownOfUsEdited.CrewmateRoles.TrapperMod;
using System.Linq;
using TownOfUsEdited.Patches;
using TownOfUsEdited.CrewmateRoles.PlumberMod;
using TownOfUsEdited.NeutralRoles.SoulCollectorMod;

namespace TownOfUsEdited.NeutralRoles.VampireMod
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
            var maxDistance = LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
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
                else if ((role.ClosestPlayer.Is(Faction.Crewmates) || (role.ClosestPlayer.Is(Faction.NeutralBenign)
                    && CustomGameOptions.CanBiteNeutralBenign) || (role.ClosestPlayer.Is(Faction.NeutralEvil)
                    && CustomGameOptions.CanBiteNeutralEvil)) &&
                    aliveVamps.Count == 1 && vamps.Count < CustomGameOptions.MaxVampiresPerGame)
                {
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (interact[4] == true)
                    {
                        role.Cooldown = CustomGameOptions.BiteCd;
                        Convert(role.ClosestPlayer);
                        Utils.Rpc(CustomRPC.Bite, role.ClosestPlayer.PlayerId);
                        return false;
                    }
                    if (interact[0] == true)
                    {
                        role.Cooldown = CustomGameOptions.TempSaveCdReset;
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
                if (PlayerControl.LocalPlayer.IsControlled() && role.ClosestPlayer.Is(Faction.Coven))
                {
                    Utils.Interact(role.ClosestPlayer, PlayerControl.LocalPlayer, true);
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
                    role.Cooldown = CustomGameOptions.TempSaveCdReset;
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

            if (newVamp.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(newVamp);
                ga.UnProtect();
            }

            if (newVamp.Is(RoleEnum.Chameleon))
            {
                var chamRole = Role.GetRole<Chameleon>(newVamp);
                if (chamRole.IsSwooped) chamRole.UnSwoop();
                Utils.Rpc(CustomRPC.ChameleonUnSwoop, newVamp.PlayerId);
            }

            if (newVamp.Is(RoleEnum.Medium))
            {
                var medRole = Role.GetRole<Medium>(newVamp);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (newVamp.Is(RoleEnum.Warden))
            {
                var warden = Role.GetRole<Warden>(newVamp);
                if (warden.Fortified != null) ShowShield.ResetVisor(warden.Fortified, warden.Player);
            }

            if (newVamp.Is(RoleEnum.Medic))
            {
                var medic = Role.GetRole<Medic>(newVamp);
                if (medic.ShieldedPlayer != null) ShowShield.ResetVisor(medic.ShieldedPlayer, medic.Player);
            }

            if (newVamp.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(newVamp);
                if (cleric.Barriered != null) cleric.UnBarrier();
            }

            if (newVamp.Is(RoleEnum.Plumber))
            {
                var plumberRole = Role.GetRole<Plumber>(newVamp);
                foreach (GameObject barricade in plumberRole.Barricades)
                {
                    UnityEngine.Object.Destroy(barricade);
                }
            }

            if (PlayerControl.LocalPlayer == newVamp)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Cleric))
                {
                    var clericRole = Role.GetRole<Cleric>(PlayerControl.LocalPlayer);
                    clericRole.CleanseButton.SetTarget(null);
                    clericRole.CleanseButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
                {
                    var oracleRole = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                    oracleRole.BlessButton.SetTarget(null);
                    oracleRole.BlessButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                {
                    var hunterRole = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(hunterRole.UsesText);
                    hunterRole.StalkButton.SetTarget(null);
                    hunterRole.StalkButton.gameObject.SetActive(false);
                    HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
                {
                    var sc = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
                    SoulExtensions.ClearSouls(sc.Souls);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Plumber))
                {
                    var plumberRole = Role.GetRole<Plumber>(PlayerControl.LocalPlayer);
                    plumberRole.Vent = null;
                    UnityEngine.Object.Destroy(plumberRole.UsesText);
                }

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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary))
                {
                    var mercRole = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
                    mercRole.GuardButton.SetTarget(null);
                    mercRole.GuardButton.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(mercRole.UsesText);
                    UnityEngine.Object.Destroy(mercRole.GoldText);
                }
            }

            if (oldRole.ExtraButtons.Any())
            {
                foreach (var button in oldRole.ExtraButtons)
                {
                    if (PlayerControl.LocalPlayer == newVamp)
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            }

            if (oldRole.ButtonLabels.Any())
            {
                foreach (var label in oldRole.ButtonLabels)
                {
                    if (PlayerControl.LocalPlayer == newVamp)
                    {
                        label.gameObject.SetActive(false);
                    }
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
                role.RegenTask(); // To see Vampire task text with mci fr fr
            }

            if (CustomGameOptions.NewVampCanAssassin
            && !CustomGameOptions.AssassinImpostorRole) new Roles.Modifiers.Assassin(newVamp);

            PlayerControl_Die.CheckEnd();
        }
    }
}
