using System.Linq;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Roles.Cultist;
using TownOfUs.Patches.ScreenEffects;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.ImpostorRoles.BomberMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class SerialKiller : Role
    {
        public SerialKiller(PlayerControl owner) : base(owner)
        {
            Name = "Serial Killer";
            ImpostorText = () => "Kill everyone to win";
            TaskText = () => "Convert someone to help you and kill the others.\nFake Tasks:";
            Color = Patches.Colors.SerialKiller;
            RoleType = RoleEnum.SerialKiller;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
            Cooldown = CustomGameOptions.SerialKillerKCD;
            ConvertCooldown = CustomGameOptions.SerialKillerKCD;
        }

        public PlayerControl ClosestPlayer;
        public KillButton _skconvertButton;
        public float ConvertCooldown { get; set; }
        public bool convertCoolingDown => ConvertCooldown > 0f;
        public bool Converted { get; set; } = false;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public KillButton skconvertButton
        {
            get => _skconvertButton;
            set
            {
                _skconvertButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven) || x.IsCrewKiller())) == 1)
            {
                SKWin();
                Utils.EndGame();
                System.Console.WriteLine("GAME OVER REASON: Serial Killer Win");
                return;
            }
            else
            {
                var sksAlives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.SerialKiller)).ToList();
                if (sksAlives.Count == 1) return;
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var killersAlive = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(RoleEnum.SerialKiller) && (x.Is(Faction.Impostors) || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven) || x.IsCrewKiller())).ToList();
                if (killersAlive.Count > 0) return;
                if (alives.Count <= 4)
                {
                    SKWin();
                    Utils.EndGame();
                    System.Console.WriteLine("GAME OVER REASON: Serial Killer Win");
                    return;
                }
                return;
            }
        }

        public void Kill(PlayerControl target)
        {
            Utils.Interact(PlayerControl.LocalPlayer, target, true);

            // Set the last kill time
            Cooldown = CustomGameOptions.SerialKillerKCD;
        }
        public void SKConvertAbility(PlayerControl target)
        {
            var sks = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller)).ToList();
            // Check if the Serial Killer can use the ability

            if (sks.Count != 1)
                return;

            if ((target.Is(Faction.Impostors) || target.Is(Faction.Madmates)) && !CustomGameOptions.SKConvertImp)
                return;

            if (target.Is(Faction.NeutralKilling) && !CustomGameOptions.SKConvertNK)
                return;

            if (target.Is(Faction.Coven) && !CustomGameOptions.SKConvertCoven)
                return;

            Convert(target);
            Utils.Rpc(CustomRPC.SKConvert, target.PlayerId);
            Converted = true;

            // Set the last ability use time
            ConvertCooldown = CustomGameOptions.SerialKillerKCD;
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
        public float SKTimer()
        {
            if (!convertCoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                ConvertCooldown -= Time.deltaTime;
                return ConvertCooldown;
            }
            else return ConvertCooldown;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var skTeam = new List<PlayerControl>();
            skTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = skTeam;
        }

        public static void Convert(PlayerControl newsk)
        {
            var oldRole = Role.GetRole(newsk);
            var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);

            if (newsk.Is(RoleEnum.Snitch))
            {
                var snitch = Role.GetRole<Snitch>(newsk);
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            if (newsk == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

            if (newsk.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(newsk);
                ga.UnProtect();
            }

            if (newsk.Is(RoleEnum.Chameleon))
            {
                var chamRole = Role.GetRole<Chameleon>(newsk);
                if (chamRole.IsSwooped)
                chamRole.UnSwoop();
                Utils.Rpc(CustomRPC.ChameleonUnSwoop, newsk.PlayerId);
            }

            if (newsk.Is(RoleEnum.Medium))
            {
                var medRole = Role.GetRole<Medium>(newsk);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (PlayerControl.LocalPlayer == newsk)
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

            Role.RoleDictionary.Remove(newsk.PlayerId);

            if (PlayerControl.LocalPlayer == newsk)
            {
                var role = new SerialKiller(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.TaskText = () => "Help your teamate to kill everyone!";
                role.RegenTask();
            }
            else
            {
                newsk.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                RoleManager.Instance.SetRole(newsk, RoleTypes.Crewmate);
                var role = new SerialKiller(newsk);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();
            }

            Role.GetRole<SerialKiller>(newsk).Converted = true;
            if (CustomGameOptions.NewSKCanGuess
            && !CustomGameOptions.AssassinImpostorRole && !newsk.Is(AbilityEnum.Assassin)) new Roles.Modifiers.Assassin(newsk);
        }
    }
}