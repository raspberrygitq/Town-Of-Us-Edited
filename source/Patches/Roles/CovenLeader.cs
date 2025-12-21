using System.Linq;
using TMPro;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;
using TownOfUsEdited.CrewmateRoles.InvestigatorMod;
using TownOfUsEdited.CrewmateRoles.TrapperMod;
using TownOfUsEdited.ImpostorRoles.BomberMod;
using TownOfUsEdited.NeutralRoles.SoulCollectorMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public class CovenLeader : Role
    {
        public CovenLeader(PlayerControl owner) : base(owner)
        {
            Name = "Coven Leader";
            ImpostorText = () => "Lead The Coven";
            TaskText = () => "Recruit a player and lead the coven to victory\nFake Tasks:";
            Color = Patches.Colors.Coven;
            RoleType = RoleEnum.CovenLeader;
            AddToRoleHistory(RoleType);
            Faction = Faction.Coven;
        }
        public KillButton _recruitButton;
        public PlayerControl ClosestPlayer;
        public bool Converted = false;
        public TextMeshPro RecruitText;
        public void Recruit(PlayerControl target)
        {
            // Check if the Hex Master can hex
            if (KillCooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                Converted = true;
                KillCooldown = CustomGameOptions.CovenKCD;
                Convert(target);
                Utils.Rpc(CustomRPC.CovenConvert, target.PlayerId);
                return;
            }
            if (interact[0] == true)
            {
                KillCooldown = CustomGameOptions.TempSaveCdReset;
                return;
            }
            else if (interact[3] == true) return;
        }
        public KillButton RecruitButton
        {
            get => _recruitButton;
            set
            {
                _recruitButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var covenTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            covenTeam.Add(PlayerControl.LocalPlayer);
            var toAdd = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && x != PlayerControl.LocalPlayer).ToList();
            foreach (var player in toAdd)
            {
                covenTeam.Add(player);
            }
            __instance.teamToShow = covenTeam;
        }

        public static void Convert(PlayerControl newcoven)
        {
            var oldRole = Role.GetRole(newcoven);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);

            if (newcoven.Is(RoleEnum.Snitch))
            {
                var snitch = Role.GetRole<Snitch>(newcoven);
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            if (StartImitate.ImitatingPlayers.Contains(PlayerControl.LocalPlayer.PlayerId)) StartImitate.ImitatingPlayers.Remove(PlayerControl.LocalPlayer.PlayerId);
            
            if (newcoven.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(newcoven);
                ga.UnProtect();
            }

            if (newcoven.Is(RoleEnum.Chameleon))
            {
                var chamRole = Role.GetRole<Chameleon>(newcoven);
                if (chamRole.IsSwooped)
                chamRole.UnSwoop();
                Utils.Rpc(CustomRPC.ChameleonUnSwoop, newcoven.PlayerId);
            }

            if (newcoven.Is(RoleEnum.Medium))
            {
                var medRole = Role.GetRole<Medium>(newcoven);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (newcoven.Is(RoleEnum.Warden))
            {
                var warden = Role.GetRole<Warden>(newcoven);
                if (warden.Fortified != null) ShowShield.ResetVisor(warden.Fortified, warden.Player);
            }

            if (newcoven.Is(RoleEnum.Medic))
            {
                var medic = Role.GetRole<Medic>(newcoven);
                if (medic.ShieldedPlayer != null) ShowShield.ResetVisor(medic.ShieldedPlayer, medic.Player);
            }

            if (newcoven.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(newcoven);
                if (cleric.Barriered != null) cleric.UnBarrier();
            }

            if (newcoven.Is(RoleEnum.Plumber))
            {
                var plumberRole = Role.GetRole<Plumber>(newcoven);
                foreach (GameObject barricade in plumberRole.Block)
                {
                    UnityEngine.Object.Destroy(barricade);
                }
            }

            if (PlayerControl.LocalPlayer == newcoven)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                HudManager.Instance.SabotageButton.gameObject.SetActive(false);
                HudManager.Instance.SabotageButton.buttonLabelText.gameObject.SetActive(false);

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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
                {
                    var sc = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
                    SoulExtensions.ClearSouls(sc.Souls);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                {
                    var hunterRole = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(hunterRole.UsesText);
                    hunterRole.StalkButton.SetTarget(null);
                    hunterRole.StalkButton.gameObject.SetActive(false);
                    HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Knight))
                {
                    var knightRole = Role.GetRole<Knight>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(knightRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    UnityEngine.Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                {
                    var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                    mysticRole.BodyArrows.Values.DestroyAll();
                    mysticRole.BodyArrows.Clear();
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
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Watcher))
                {
                    var watcherRole = Role.GetRole<Watcher>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(watcherRole.UsesText);
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

                if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin))
                {
                    Ability.AbilityDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
                {
                    var swooperRole = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
                    if (swooperRole.IsSwooped)
                    {
                    swooperRole.UnSwoop();
                    Utils.Rpc(CustomRPC.UnSwoop, PlayerControl.LocalPlayer.PlayerId);
                    }
                    swooperRole.Enabled = false;
                    swooperRole.Cooldown = CustomGameOptions.SwoopCd;
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
                {
                    var bomberRole = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
                    bomberRole.Bomb.ClearBomb();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary))
                {
                    var mercRole = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
                    mercRole.GuardButton.SetTarget(null);
                    mercRole.GuardButton.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(mercRole.UsesText);
                    UnityEngine.Object.Destroy(mercRole.GoldText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
                {
                    var amne = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);
                    if (amne.BodyArrows.Count != 0)
                    {
                        amne.BodyArrows.Values.DestroyAll();
                        amne.BodyArrows.Clear();
                    }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
                {
                    var vulture = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);
                    if (vulture.BodyArrows.Count != 0)
                    {
                        vulture.BodyArrows.Values.DestroyAll();
                        vulture.BodyArrows.Clear();
                    }
                    UnityEngine.Object.Destroy(vulture.BodiesText);
                }
            }

            if (oldRole.ExtraButtons.Any())
            {
                foreach (var button in oldRole.ExtraButtons)
                {
                    if (PlayerControl.LocalPlayer == newcoven)
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            }

            if (oldRole.ButtonLabels.Any())
            {
                foreach (var label in oldRole.ButtonLabels)
                {
                    if (PlayerControl.LocalPlayer == newcoven)
                    {
                        label.gameObject.SetActive(false);
                    }
                }
            }

            Role.RoleDictionary.Remove(newcoven.PlayerId);

            if (PlayerControl.LocalPlayer == newcoven)
            {
                var role = new Coven(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }
            else
            {
                var role = new Coven(newcoven);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }
            
            PlayerControl_Die.CheckEnd();
        }
    }
}