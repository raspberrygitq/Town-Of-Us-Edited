using System.Linq;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.Patches.ScreenEffects;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Roles
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
            Cooldown = CustomGameOptions.CovenKCD;
        }

        public KillButton _sabotageButton;
        public KillButton _recruitButton;
        public PlayerControl ClosestPlayer;
        public bool Converted = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public void Recruit(PlayerControl target)
        {
            // Check if the Hex Master can hex
            if (Cooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
            if (interact[4] == true)
            {
                Convert(target);
                Converted = true;
                Utils.Rpc(CustomRPC.CovenConvert, target.PlayerId);
                Cooldown = CustomGameOptions.CovenKCD;
                return;
            }
            if (interact[0] == true)
            {
                Cooldown = CustomGameOptions.ProtectKCReset;
                return;
            }
            else if (interact[1] == true)
            {
                Cooldown = CustomGameOptions.VestKCReset;
                return;
            }
            else if (interact[3] == true) return;
        }

        public void Kill(PlayerControl target)
        {
            // Check if the Coven can kill
            if (Cooldown > 0)
                return;

            if (target.Is(Faction.Coven))
                return;

            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            // Set the last kill time
            Cooldown = CustomGameOptions.CovenKCD;
        }
        public KillButton SabotageButton
        {
            get => _sabotageButton;
            set
            {
                _sabotageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
        public float KillTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
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

            if (newcoven == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

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

            if (PlayerControl.LocalPlayer == newcoven)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Knight)) HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);

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
        }
    }
}