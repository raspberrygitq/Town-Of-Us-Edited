using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.Roles.Modifiers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.ImpostorRoles.TraitorMod;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();

        public static bool NobodyWins;
        public static bool SurvOnlyWins;
        public static bool VampireWins;
        public static bool SKWins;
        public static bool CovenWins;
        public static bool ImpostorWins;
        public static bool CrewmateWins;
        public static bool SeeRoles { get; set; } = false;

        public List<KillButton> ExtraButtons = new List<KillButton>();

        public Func<string> ImpostorText;
        public Func<string> TaskText;

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
            //TotalTasks = player.Data.Tasks.Count;
            //TasksLeft = TotalTasks;
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        protected float Scale { get; set; } = 1f;
        protected internal Color Color { get; set; }
        protected internal Color oldColor { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        protected internal int InfectionState { get; set; } = 0;
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;
        protected internal int Kills { get; set; } = 0;
        protected internal int CorrectKills { get; set; } = 0;
        protected internal int IncorrectKills { get; set; } = 0;
        protected internal int CorrectAssassinKills { get; set; } = 0;
        protected internal int IncorrectAssassinKills { get; set; } = 0;
        protected internal float KillCooldown { get; set; } = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        protected internal bool Hidden { get; set; } = false;

        protected internal Faction Faction { get; set; } = Faction.Crewmates;
        protected internal DeathReasons DeathReason { get; set; } = DeathReasons.Killed;
        protected internal Alignment Alignment { get; set; } = Alignment.None;

        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

        public void AddToRoleHistory(RoleEnum role)
        {
            RoleHistory.Add(KeyValuePair.Create(_player.PlayerId, role));
        }

        public void RemoveFromRoleHistory(RoleEnum role)
        {
            RoleHistory.Remove(KeyValuePair.Create(_player.PlayerId, role));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        //public static T Gen<T>()

        internal virtual bool Criteria()
        {
            return DeadCriteria() || MadmateCriteria() || ImpostorCriteria() || VampireCriteria() || SerialKillerCriteria() || CovenCriteria() || LoverCriteria() || SelfCriteria() || RoleCriteria() || SeerCriteria() || GuardianAngelCriteria() || Local;
        }

        internal virtual bool ColorCriteria()
        {
            return SelfCriteria() || DeadCriteria() || ((MadmateCriteria() || ImpostorCriteria() || VampireCriteria() || SerialKillerCriteria() || CovenCriteria() || RoleCriteria() || SeerCriteria() || GuardianAngelCriteria()) && (!PlayerControl.LocalPlayer.IsHypnotised() || MeetingHud.Instance));
        }

        internal virtual bool DeadCriteria()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && CustomGameOptions.GameMode != GameMode.Werewolf) return Utils.ShowDeadBodies;
            return false;
        }

        internal virtual bool ImpostorCriteria()
        {
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() &&
                CustomGameOptions.ImpostorSeeRoles) return true;
            return false;
        }

        internal virtual bool MadmateCriteria()
        {
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Is(Faction.Madmates) && !PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)
            && CustomGameOptions.ImpostorSeeRoles) return true;
            if (Faction == Faction.Madmates && PlayerControl.LocalPlayer.Is(Faction.Impostors)
            && CustomGameOptions.ImpostorSeeRoles) return true;
            if (Faction == Faction.Madmates && PlayerControl.LocalPlayer.Is(Faction.Madmates)
            && CustomGameOptions.ImpostorSeeRoles) return true;
            return false;
        }

        internal virtual bool VampireCriteria()
        {
            if (RoleType == RoleEnum.Vampire && PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return true;
            return false;
        }

        internal virtual bool SerialKillerCriteria()
        {
            if (RoleType == RoleEnum.SerialKiller && PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) return true;
            return false;
        }

        internal virtual bool CovenCriteria()
        {
            if (Faction == Faction.Coven && PlayerControl.LocalPlayer.Is(Faction.Coven)) return true;
            return false;
        }

        internal virtual bool LoverCriteria()
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Lover))
            {
                if (Local) return true;
                var lover = Modifier.GetModifier<Lover>(PlayerControl.LocalPlayer);
                if (lover.OtherLover.Player != Player) return false;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
                if (MeetingHud.Instance || Utils.ShowDeadBodies) return true;
                if (lover.OtherLover.Player.Is(RoleEnum.Mayor))
                {
                    var mayor = GetRole<Mayor>(lover.OtherLover.Player);
                    if (mayor.Revealed) return true;
                }
            }
            return false;
        }

        internal virtual bool SelfCriteria()
        {
            return GetRole(PlayerControl.LocalPlayer) == this;
        }

        internal virtual bool RoleCriteria()
        {
            return PlayerControl.LocalPlayer.Is(ModifierEnum.Sleuth) && Modifier.GetModifier<Sleuth>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId) || SeeRoles == true || (Player.Data.IsDead && CustomGameOptions.GameMode == GameMode.Werewolf);
        }
        internal virtual bool GuardianAngelCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAKnowsTargetRole && Player == GetRole<GuardianAngel>(PlayerControl.LocalPlayer).target;
        }

        internal virtual bool SeerCriteria()
        {
            return CustomGameOptions.GameMode == GameMode.Werewolf && PlayerControl.LocalPlayer.Is(RoleEnum.Seer) && Role.GetRole<Seer>(PlayerControl.LocalPlayer).Revealed.Contains(Player.PlayerId);
        }

        protected virtual void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
        }

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }
        public static void SurvOnlyWin()
        {
            SurvOnlyWins = true;
        }
        public static void VampWin()
        {
            foreach (var jest in GetRoles(RoleEnum.Jester))
            {
                var jestRole = (Jester)jest;
                if (jestRole.VotedOut) return;
            }
            foreach (var troll in GetRoles(RoleEnum.Troll))
            {
                var trollRole = (Troll)troll;
                if (trollRole.TrolledVotedOut) return;
            }
            foreach (var exe in GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)exe;
                if (exeRole.TargetVotedOut) return;
            }
            foreach (var doom in GetRoles(RoleEnum.Doomsayer))
            {
                var doomRole = (Doomsayer)doom;
                if (doomRole.WonByGuessing) return;
            }
            foreach (var sc in GetRoles(RoleEnum.SoulCollector))
            {
                var scRole = (SoulCollector)sc;
                if (scRole.CollectedSouls) return;
            }
            foreach (var vult in GetRoles(RoleEnum.Vulture))
            {
                var vultureRole = (Vulture)vult;
                if (vultureRole.VultureWins) return;
            }

            VampireWins = true;

            Utils.Rpc(CustomRPC.VampireWin);
        }

        public static void SKWin()
        {
            foreach (var jest in GetRoles(RoleEnum.Jester))
            {
                var jestRole = (Jester)jest;
                if (jestRole.VotedOut) return;
            }
            foreach (var troll in GetRoles(RoleEnum.Troll))
            {
                var trollRole = (Troll)troll;
                if (trollRole.TrolledVotedOut) return;
            }
            foreach (var exe in GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)exe;
                if (exeRole.TargetVotedOut) return;
            }
            foreach (var doom in GetRoles(RoleEnum.Doomsayer))
            {
                var doomRole = (Doomsayer)doom;
                if (doomRole.WonByGuessing) return;
            }
            foreach (var sc in GetRoles(RoleEnum.SoulCollector))
            {
                var scRole = (SoulCollector)sc;
                if (scRole.CollectedSouls) return;
            }
            foreach (var vult in GetRoles(RoleEnum.Vulture))
            {
                var vultureRole = (Vulture)vult;
                if (vultureRole.VultureWins) return;
            }

            SKWins = true;

            Utils.Rpc(CustomRPC.SKwin);
        }
        public static bool NeutralEvilWin()
        {
            foreach (var role in AllRoles)
            {
                return 
                    CustomGameOptions.NeutralEvilWinEndsGame && !role.PauseEndCrit &&
                    (GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut) == true ||
                    GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut) == true ||
                    GetRoles(RoleEnum.Troll).Any(x => ((Troll)x).TrolledVotedOut) == true ||
                    GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing) == true ||
                    GetRoles(RoleEnum.Vulture).Any(x => ((Vulture)x).VultureWins) == true ||
                    GetRoles(RoleEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks) == true ||
                    GetRoles(RoleEnum.SoulCollector).Any(x => ((SoulCollector)x).CollectedSouls) == true);
            }
            return false;
        }
        public static bool LoverWin()
        {
            return Modifier.GetModifiers(ModifierEnum.Lover).Any(x => ((Lover)x).LoveCoupleWins) == true;
        }
        public static bool NeutralKillingWin()
        {
            return 
                GetRoles(RoleEnum.Juggernaut).Any(x => ((Juggernaut)x).JuggernautWins) == true ||
                GetRoles(RoleEnum.Glitch).Any(x => ((Glitch)x).GlitchWins) == true ||
                GetRoles(RoleEnum.Player).Any(x => ((Player)x).PlayerWins) == true ||
                GetRoles(RoleEnum.Pestilence).Any(x => ((Pestilence)x).PestilenceWins) == true ||
                GetRoles(RoleEnum.Arsonist).Any(x => ((Arsonist)x).ArsonistWins) == true ||
                GetRoles(RoleEnum.Doppelganger).Any(x => ((Doppelganger)x).DoppelgangerWins) == true ||
                GetRoles(RoleEnum.Attacker).Any(x => ((Attacker)x).AttackerWins) == true ||
                GetRoles(RoleEnum.Terrorist).Any(x => ((Terrorist)x).TerroristWins) == true ||
                GetRoles(RoleEnum.Plaguebearer).Any(x => ((Plaguebearer)x).PlaguebearerWins) == true ||
                SKWins == true ||
                VampireWins == true ||
                GetRoles(RoleEnum.Mutant).Any(x => ((Mutant)x).MutantWins) == true ||
                GetRoles(RoleEnum.Infectious).Any(x => ((Infectious)x).InfectiousWins) == true ||
                GetRoles(RoleEnum.Maul).Any(x => ((Maul)x).WerewolfWins) == true ||
                GetRoles(RoleEnum.WhiteWolf).Any(x => ((WhiteWolf)x).WhiteWolfWins) == true;
        }

        internal static bool NobodyEndCriteria(LogicGameFlowNormal __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.NeutralEvil || role.Faction == Faction.NeutralBenign;

                    return flag2;
                });

                return flag;
            }

            bool SurvOnly()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = false;
                foreach (var player in alives)
                {
                    if (player.Is(RoleEnum.Survivor)) flag = true;
                }
                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                if (SurvOnly())
                {
                    Utils.Rpc(CustomRPC.SurvivorOnlyWin);

                    SurvOnlyWin();
                    Utils.EndGame();
                    System.Console.WriteLine("GAME OVER REASON: Survivor Only Win");
                    return false;
                }
                else
                {
                    Utils.Rpc(CustomRPC.NobodyWins);

                    NobodyWinsFunc();
                    Utils.EndGame();
                    System.Console.WriteLine("GAME OVER REASON: Nobody Wins");
                    return false;
                }
            }
            return true;
        }

        private static bool CheckLoversWin()
        {
            //System.Console.WriteLine("CHECKWIN");
            var players = PlayerControl.AllPlayerControls.ToArray();
            var alives = players.Where(x => !x.Data.IsDead).ToList();

            var firstlover = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(ModifierEnum.Lover)).ToList();
            foreach (var lover1 in firstlover)
            {
                var lover2 = Modifier.GetModifier<Lover>(lover1).OtherLover.Player;
                {
                    return !lover1.Data.IsDead && !lover1.Data.Disconnected && !lover2.Data.IsDead && !lover2.Data.Disconnected &&
                        (alives.Count == 3) | (alives.Count == 2);
                }
            }
            return false;
        }

        internal virtual void NeutralWin(LogicGameFlowNormal __instance)
        {
            return;
        }

        internal bool PauseEndCrit = false;
        public static bool ForceGameEnd = false;

        protected virtual string NameText(bool revealTasks, bool revealRole, bool revealModifier, bool revealLover, PlayerVoteArea player = null)
        {
            if (PlayerControl.LocalPlayer.IsHypnotised() && Player.GetCustomOutfitType() == CustomPlayerOutfitType.Morph && player == null) return PlayerControl.LocalPlayer.GetDefaultOutfit().PlayerName;
            else if (((CamouflageUnCamouflage.IsCamoed && !PlayerControl.LocalPlayer.IsHypnotised()) || (PlayerControl.LocalPlayer.IsHypnotised() && PlayerControl.LocalPlayer != Player)) && player == null) return "";

            if (Player == null) return "";

            String PlayerName = Player.GetDefaultOutfit().PlayerName;

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;
                if (Player == ga.target && ((Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                    || (PlayerControl.LocalPlayer.Data.IsDead && !ga.Player.Data.IsDead)))
                {
                    PlayerName += "<color=#B3FFFFFF> ★</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner) role;
                if (Player == exe.target && PlayerControl.LocalPlayer.Data.IsDead && !exe.Player.Data.IsDead)
                {
                    PlayerName += "<color=#8C4005FF> X</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Troll))
            {
                var troll = (Troll) role;
                if (Player == troll.TrolledPlayer && (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer == role.Player))
                {
                    PlayerName += "<color=#8C4005FF> T</color>";
                }
            }

            var modifier = Modifier.GetModifier(Player);
            if (modifier != null && modifier.GetColoredSymbol() != null)
            {
                if (modifier.ModifierType == ModifierEnum.Lover && (revealModifier || revealLover))
                    PlayerName += $" {modifier.GetColoredSymbol()}";
                else if (modifier.ModifierType != ModifierEnum.Lover && revealModifier)
                    PlayerName += $" {modifier.GetColoredSymbol()}";
            }

            if (revealTasks && (Faction == Faction.Crewmates || RoleType == RoleEnum.Phantom))
            {
                if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SeeTasksWhenDead) || (MeetingHud.Instance && CustomGameOptions.SeeTasksDuringMeeting) || (!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && CustomGameOptions.SeeTasksDuringRound))
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
            }

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                                   MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return PlayerName;

            if (!revealRole) return PlayerName;

            Player.nameText().transform.localPosition = new Vector3(0f, 0.15f, -0.5f);

            return PlayerName + "\n" + Name;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }

        public void RegenTask()
        {
            bool createTask;
            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name} (Press F2 for Role infos)\n{TaskText()}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text =
                $"{ColorString}Role: {Name} (Press F2 for Role infos)\n{TaskText()}</color>";
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(rpc, player.PlayerId);
            return role;
        }

        public static T GenRole<T>(Type type, PlayerControl player)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(CustomRPC.SetRole, player.PlayerId, (string)type.FullName);
            return role;
        }

        public static T GenModifier<T>(Type type, PlayerControl player)
        {
            var modifier = (T)Activator.CreateInstance(type, new object[] { player });

            Utils.Rpc(CustomRPC.SetModifier, player.PlayerId, (string)type.FullName);
            return modifier;
        }

        public static T GenRole<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var role = GenRole<T>(type, player);
            players.Remove(player);
            return role;
        }
        public static T GenModifier<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var modifier = GenModifier<T>(type, player);
            players.Remove(player);
            return modifier;
        }

        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static IEnumerable<Role> GetFactions(Faction faction)
        {
            return AllRoles.Where(x => x.Faction == faction);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro ModifierText;

            public static float Scale;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
            public static class IntroCutscene_ShowTeam__d_MoveNext
            {
                public static void Prefix(IntroCutscene._ShowTeam_d__38 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null && role.Faction != Faction.Impostors && role.Faction != Faction.Madmates) role.IntroPrefix(__instance);
                    else if (role.Faction == Faction.Impostors || role.Faction == Faction.Madmates)
                    {
                        var impTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                        impTeam.Add(PlayerControl.LocalPlayer);
                        var toAdd = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors) || x.Is(Faction.Madmates)) && x != PlayerControl.LocalPlayer).ToList();
                        foreach (var player in toAdd)
                        {
                            impTeam.Add(player);
                        }
                        __instance.teamToShow = impTeam;
                    }
                }
                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    // var alpha = __instance.__4__this.RoleText.color.a;
                    if (role != null && !role.Hidden)
                    {
                        if (CustomGameOptions.GameMode == GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Battle Royale";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Player;
                            __instance.__4__this.BackgroundBar.material.color = role.Color;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        if (role.Faction == Faction.NeutralEvil)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            var sound = GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Object.Instantiate(sound, HudManager.Instance.transform.parent);
                        }

                        if (role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }

                        if (role.Faction == Faction.Coven)
                        {
                            __instance.__4__this.TeamTitle.text = "Coven";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Coven;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Coven;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }

                        if (role.Alignment == Alignment.CrewmateKilling && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = PlayerControl.LocalPlayer.KillSfx;
                        }
                        
                        if (role.Alignment == Alignment.CrewmateSupport && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Engineer);
                        }

                        if (role.Alignment == Alignment.CrewmateProtective && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Scientist);
                        }

                        if (role.Alignment == Alignment.CrewmateInvestigative && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = HudManager.Instance.TaskCompleteSound;
                        }

                        if (role.Faction == Faction.NeutralKilling && CustomGameOptions.GameMode != GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = DestroyableSingleton<HnSImpostorScreamSfx>.Instance.HnSLocalImpostorTransformSfx;
                        }

                        if (role.Player.Is(Faction.Madmates))
                        {
                            __instance.__4__this.TeamTitle.text = "Madmate";
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        else if (role.Faction == Faction.Impostors && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Werewolf";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Werewolf;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Werewolf;
                        }

                        else if (role.Faction == Faction.Crewmates && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Villager";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Villager;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Villager;
                        }
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        //    __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        // __instance.__4__this.BackgroundBar.material.color = role.Color;
                        //                        TestScale = Mathf.Max(__instance.__this.Title.scale, TestScale);
                        //                        __instance.__this.Title.scale = TestScale / role.Scale;
                    }
                    /*else if (!__instance.isImpostor)
                    {
                        __instance.__this.ImpostorText.text = "Haha imagine being a boring old crewmate";
                    }*/

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }
                        ModifierText.color = modifier.Color;

                        //
                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        if (!PlayerControl.LocalPlayer.Is(Faction.Madmates))
                        {
                        ModifierText.gameObject.SetActive(true);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
            public static class IntroCutscene_ShowRole_d__41
            {
                public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (CustomGameOptions.GameMode == GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Battle Royale";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Player;
                            __instance.__4__this.BackgroundBar.material.color = role.Color;
                        }

                        else if (role.Faction == Faction.NeutralEvil)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            var sound = GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Object.Instantiate(sound, HudManager.Instance.transform.parent);
                        }

                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        if (role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }

                        if (role.Faction == Faction.Coven)
                        {
                            __instance.__4__this.TeamTitle.text = "Coven";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Coven;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Coven;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }

                        if (role.Alignment == Alignment.CrewmateKilling && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = PlayerControl.LocalPlayer.KillSfx;
                        }
                        
                        if (role.Alignment == Alignment.CrewmateSupport && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Engineer);
                        }

                        if (role.Alignment == Alignment.CrewmateProtective && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Scientist);
                        }

                        if (role.Alignment == Alignment.CrewmateInvestigative && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = HudManager.Instance.TaskCompleteSound;
                        }

                        else if (role.Faction == Faction.NeutralKilling && CustomGameOptions.GameMode != GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = DestroyableSingleton<HnSImpostorScreamSfx>.Instance.HnSLocalImpostorTransformSfx;
                        }

                        else if (role.Player.Is(Faction.Madmates))
                        {
                            __instance.__4__this.TeamTitle.text = "Madmate";
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        else if (role.Faction == Faction.Impostors && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Werewolf";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Werewolf;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Werewolf;
                        }

                        else if (role.Faction == Faction.Crewmates && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Villager";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Villager;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Villager;
                        }
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        // __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else if (!PlayerControl.LocalPlayer.Is(Faction.Madmates))
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }
                        ModifierText.color = modifier.Color;

                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        if (!PlayerControl.LocalPlayer.Is(Faction.Madmates))
                        {
                        ModifierText.gameObject.SetActive(true);
                        }
                    }

                    var coven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven)).ToList();
                    var imps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors)).ToList();

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Werewolf && imps.Count == 1)
                        __instance.__4__this.ImpostorText.text = "There is <color=#A86629FF>1 Werewolf</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Werewolf && imps.Count > 1)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#A86629FF>{imps.Count} Werewolves</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.BattleRoyale)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text = "";
                    else if (imps.Count == 1)
                        __instance.__4__this.ImpostorText.text = "There is <color=#FF0000>1 Impostor</color> among us";
                    else if (imps.Count > 1)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#FF0000>{imps.Count} Impostors</color> among us";

                    if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count != 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text += $"\nAnd also <color=#bf5fff>{coven.Count} Coven Members</color>";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count != 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text += $"\nAnd also <color=#bf5fff>{coven.Count} Coven Member</color>";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count == 0)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#bf5fff>{coven.Count} Coven Members</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count == 0)
                        __instance.__4__this.ImpostorText.text = $"There is <color=#bf5fff>{coven.Count} Coven Member</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count != 0 && PlayerControl.LocalPlayer.Is(Faction.Impostors))
                    {
                        __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        __instance.__4__this.ImpostorText.text += $"There are <color=#bf5fff>{coven.Count} Coven Members</color> among us";
                    }
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count != 0 && PlayerControl.LocalPlayer.Is(Faction.Impostors))
                    {
                        __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        __instance.__4__this.ImpostorText.text += $"\nThere is <color=#bf5fff>{coven.Count} Coven Member</color> among us";
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
            public static class IntroCutscene_CoBegin_d__29
            {
                public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    if (role != null && !role.Hidden)
                    {
                        if (CustomGameOptions.GameMode == GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Battle Royale";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Player;
                            __instance.__4__this.BackgroundBar.material.color = role.Color;
                        }

                        if (role.Faction == Faction.NeutralEvil)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            var sound = GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = Object.Instantiate(sound, HudManager.Instance.transform.parent);
                        }

                        if (role.Faction == Faction.Impostors)
                        {
                            __instance.__4__this.TeamTitle.text = "Impostor";
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        if (role.Faction == Faction.Coven)
                        {
                            __instance.__4__this.TeamTitle.text = "Coven";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Coven;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Coven;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
                        }

                        if (role.Alignment == Alignment.CrewmateKilling && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = PlayerControl.LocalPlayer.KillSfx;
                        }
                        
                        if (role.Alignment == Alignment.CrewmateSupport && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Engineer);
                        }

                        if (role.Alignment == Alignment.CrewmateProtective && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Scientist);
                        }

                        if (role.Alignment == Alignment.CrewmateInvestigative && role.Faction != Faction.Madmates)
                        {
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = HudManager.Instance.TaskCompleteSound;
                        }

                        if (role.Faction == Faction.NeutralBenign)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Crewmate);
                        }

                        if (role.Faction == Faction.NeutralKilling && CustomGameOptions.GameMode != GameMode.BattleRoyale)
                        {
                            __instance.__4__this.TeamTitle.text = "Neutral";
                            __instance.__4__this.TeamTitle.color = Color.white;
                            __instance.__4__this.BackgroundBar.material.color = Color.white;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = DestroyableSingleton<HnSImpostorScreamSfx>.Instance.HnSLocalImpostorTransformSfx;
                        }

                        if (role.Player.Is(Faction.Madmates))
                        {
                            __instance.__4__this.TeamTitle.text = "Madmate";
                            __instance.__4__this.BackgroundBar.material.color = Palette.ImpostorRed;
                            __instance.__4__this.TeamTitle.color = Palette.ImpostorRed;
                            PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Impostor);
                        }

                        else if (role.Faction == Faction.Impostors && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Werewolf";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Werewolf;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Werewolf;
                        }

                        else if (role.Faction == Faction.Crewmates && CustomGameOptions.GameMode == GameMode.Werewolf)
                        {
                            __instance.__4__this.TeamTitle.text = "Villager";
                            __instance.__4__this.TeamTitle.color = Patches.Colors.Villager;
                            __instance.__4__this.BackgroundBar.material.color = Patches.Colors.Villager;
                        }

                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else if (!PlayerControl.LocalPlayer.Is(Faction.Madmates))
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }
                        ModifierText.color = modifier.Color;

                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        if (!PlayerControl.LocalPlayer.Is(Faction.Madmates))
                        {
                        ModifierText.gameObject.SetActive(true);
                        }
                    }

                    var coven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven)).ToList();
                    var imps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors)).ToList();

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There are an <color=#FF0000FF>Unknown Number of Impostors</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Werewolf && imps.Count == 1)
                        __instance.__4__this.ImpostorText.text = "There is <color=#A86629FF>1 Werewolf</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Werewolf && imps.Count > 1)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#A86629FF>{imps.Count} Werewolves</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.BattleRoyale)
                        __instance.__4__this.ImpostorText.text = "";
                    else if (PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text = "";
                    else if (imps.Count == 1)
                        __instance.__4__this.ImpostorText.text = "There is <color=#FF0000>1 Impostor</color> among us";
                    else if (imps.Count > 1)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#FF0000>{imps.Count} Impostors</color> among us";

                    if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count != 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text += $"\nAnd also <color=#bf5fff>{coven.Count} Coven Members</color>";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count != 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                        __instance.__4__this.ImpostorText.text += $"\nAnd also <color=#bf5fff>{coven.Count} Coven Member</color>";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count == 0)
                        __instance.__4__this.ImpostorText.text = $"There are <color=#bf5fff>{coven.Count} Coven Members</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count == 0)
                        __instance.__4__this.ImpostorText.text = $"There is <color=#bf5fff>{coven.Count} Coven Member</color> among us";
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count > 1 && imps.Count != 0 && PlayerControl.LocalPlayer.Is(Faction.Impostors))
                    {
                        __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        __instance.__4__this.ImpostorText.text += $"There are <color=#bf5fff>{coven.Count} Coven Members</color> among us";
                    }
                    else if (CustomGameOptions.GameMode == GameMode.Classic && coven.Count == 1 && imps.Count != 0 && PlayerControl.LocalPlayer.Is(Faction.Impostors))
                    {
                        __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        __instance.__4__this.ImpostorText.text += $"\nThere is <color=#bf5fff>{coven.Count} Coven Member</color> among us";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__141), nameof(PlayerControl._CoSetTasks_d__141.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__141 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                var role = GetRole(player);
                var modifier = Modifier.GetModifier(player);

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (role == null || role.Hidden) return;
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name}  (Press F2 for Role infos)\n{role.TaskText()}</color>";
                player.myTasks.Insert(0, task);
            }
        }

        [HarmonyPatch]
        public static class ShipStatus_KMPKPPGPNIH
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return false;
                if (!AmongUsClient.Instance.AmHost) return false;
                if (ShipStatus.Instance.Systems != null)
                {
                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                    {
                        var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        if (lifeSuppSystemType.Countdown < 0f)
                        {
                            ImpostorWins = true;
                            Utils.Rpc(CustomRPC.ImpostorWin);
                            Utils.EndGame(GameOverReason.ImpostorByVote);
                            System.Console.WriteLine("GAME OVER REASON: Sabotage");
                            return false;
                        }
                    }

                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                    {
                        var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        if (reactorSystemType.Countdown < 0f)
                        {
                            ImpostorWins = true;
                            Utils.Rpc(CustomRPC.ImpostorWin);
                            Utils.EndGame(GameOverReason.ImpostorByVote);
                            System.Console.WriteLine("GAME OVER REASON: Sabotage");
                            return false;
                        }
                    }

                    if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                    {
                        var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                        if (reactorSystemType.Countdown < 0f)
                        {
                            ImpostorWins = true;
                            Utils.Rpc(CustomRPC.ImpostorWin);
                            Utils.EndGame(GameOverReason.ImpostorByVote);
                            System.Console.WriteLine("GAME OVER REASON: Sabotage");
                            return false;
                        }
                    }
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                {
                    var crews = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Data.Disconnected).ToList();
                    if (crews.Count != 0 && CustomGameOptions.GameMode != GameMode.Werewolf)
                    {
                        CrewmateWins = true;
                        Utils.Rpc(CustomRPC.CrewmateWin);
                        Utils.EndGame(GameOverReason.HumansByVote);
                        System.Console.WriteLine("GAME OVER REASON: Tasks Completed");
                        return false;
                    }
                }

                if (Role.ForceGameEnd == true)
                {
                    Utils.EndGame(GameOverReason.ImpostorBySabotage);
                    System.Console.WriteLine("GAME OVER REASON: Host Forced End Game");
                    return false;
                }

                if (NeutralEvilWin())
                {
                    Utils.EndGame();
                    System.Console.WriteLine("GAME OVER REASON: Neutral Evil Win");
                    return false;
                }

                foreach (var role in AllRoles)
                {
                    var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                    var traitorIsEnd = false;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled)
                        {
                            alives.Add(player);
                        }
                    }
                    if (SetTraitor.WillBeTraitor != null)
                    {
                        if (SetTraitor.WillBeTraitor.Data.IsDead || SetTraitor.WillBeTraitor.Data.Disconnected || alives.Count < CustomGameOptions.LatestSpawn || impsAlive.Count * 2 >= alives.Count) traitorIsEnd = false;
                        else traitorIsEnd = true;
                    }
                    if (role.PauseEndCrit || traitorIsEnd || Role.ForceGameEnd || NeutralEvilWin()) return false;

                    var aliveimps = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors) || x.Is(Faction.Madmates)) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivenk = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivecoven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivekillers = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralKilling) || x.Is(Faction.Impostors) || x.Is(Faction.Coven)) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivesnonkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(Faction.NeutralKilling) && !x.Is(Faction.Madmates) && !x.Is(Faction.Coven) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivevamps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivesks = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var alivelovers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Is(RoleEnum.Astral) && Role.GetRole<Astral>(player).Enabled)
                        {
                            alivesnonkiller.Add(player);
                        }
                    }
                    if (CheckLoversWin())
                    {
                        foreach (var lover in alivelovers)
                        {
                            var loverRole = Modifier.GetModifier(lover);
                            loverRole.ModifierWin(__instance);
                            Utils.EndGame();
                            // Lovers win
                            return false;
                        }
                    }
                    else if (alivenk.Count == 1 && alivecoven.Count <= 0 && aliveimps.Count <= 0 && alivesnonkiller.Count <= 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
                    {
                        foreach (var nk in alivenk)
                        {
                            var nkRole = Role.GetRole(nk);
                            nkRole.NeutralWin(__instance);
                            Utils.EndGame();
                            // Nk win
                            return false;
                        }
                    }
                    else if (alivenk.Count == alivesks.Count && alivecoven.Count <= 0 && aliveimps.Count <= 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivesks.Count >= alivesnonkiller.Count && alivelovers.Count < 2)
                    {
                        foreach (var nk in alivenk)
                        {
                            var nkRole = Role.GetRole(nk);
                            nkRole.NeutralWin(__instance);
                            Utils.EndGame();
                            // Sk Win
                            return false;
                        }
                    }
                    else if (alivenk.Count == alivevamps.Count && alivecoven.Count <= 0 && aliveimps.Count <= 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue) && alivevamps.Count >= alivesnonkiller.Count && alivelovers.Count < 2)
                    {
                        foreach (var nk in alivenk)
                        {
                            var nkRole = Role.GetRole(nk);
                            nkRole.NeutralWin(__instance);
                            Utils.EndGame();
                            // Vampires Win
                            return false;
                        }
                    }
                    else if (alivenk.Count <= 0 && alivecoven.Count <= 0 && alivesnonkiller.Count <= aliveimps.Count && CustomGameOptions.GameMode != GameMode.Chaos && aliveimps.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
                    {
                        ImpostorWins = true;
                        Utils.Rpc(CustomRPC.ImpostorWin);
                        Utils.EndGame(GameOverReason.ImpostorByVote);
                        System.Console.WriteLine("GAME OVER REASON: Impostor Victory");
                        return false;
                    }
                    else if (alivenk.Count <= 0 && aliveimps.Count <= 0 && alivesnonkiller.Count <= alivecoven.Count && alivecoven.Count > 0 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
                    {
                        CovenWins = true;
                        Utils.Rpc(CustomRPC.CovenWin);
                        Utils.EndGame();
                        System.Console.WriteLine("GAME OVER REASON: Coven Win");
                        return false;
                    }
                    else if (alivekillers.Count <= 0)
                    {
                        CrewmateWins = true;
                        Utils.Rpc(CustomRPC.CrewmateWin);
                        Utils.EndGame(GameOverReason.HumansByVote);
                        System.Console.WriteLine("GAME OVER REASON: No Killers Left");
                        return false;
                    }
                    else if (CustomGameOptions.GameMode == GameMode.Chaos && alivesnonkiller.Count <= 0)
                    {
                        ImpostorWins = true;
                        Utils.Rpc(CustomRPC.ImpostorWin);
                        Utils.EndGame(GameOverReason.ImpostorByVote);
                        System.Console.WriteLine("GAME OVER REASON: Chaos GameMode Impostor Win");
                        return false;
                    }
                }

                if (!NobodyEndCriteria(__instance)) return false;

                return false;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.Values.DestroyAll();
                    ((Snitch)role).SnitchArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
                {
                    ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                    ((Amnesiac)role).BodyArrows.Clear();
                }
                /*foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Aurial))
                {
                    ((Aurial)role).SenseArrows.Values.DestroyAll();
                    ((Aurial)role).SenseArrows.Clear();
                }*/
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
                {
                    ((Medium)role).MediatedPlayers.Values.DestroyAll();
                    ((Medium)role).MediatedPlayers.Clear();
                }

                RoleDictionary.Clear();
                RoleHistory.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null) return;
                if (CustomGameOptions.GameMode == GameMode.Werewolf)
                {
                    if (ExileController.Instance.initData.networkedPlayer == null) return;
                    var info = ExileController.Instance.initData.networkedPlayer;
                    var role = GetRole(info.Object);
                    if (role == null) return;
                    var roleName = role.Name;
                    __result = $"{info.PlayerName} was {roleName}.";
                    return;
                }
                switch (name)
                {
                    case StringNames.NoExileTie:
                        if (ExileController.Instance.initData.networkedPlayer == null)
                        {
                            foreach (var oracle in GetRoles(RoleEnum.Oracle))
                            {
                                var oracleRole = (Oracle)oracle;
                                if (oracleRole.SavedConfessor)
                                {
                                    oracleRole.SavedConfessor = false;
                                    __result = $"{oracleRole.Confessor.GetDefaultOutfit().PlayerName} was blessed by an Oracle!";
                                }
                            }
                            foreach (var terrorist in Role.GetRoles(RoleEnum.Terrorist))
                            {
                                var terroristRole = (Terrorist)terrorist;
                                if (terroristRole.SavedVote)
                                {
                                    terroristRole.SavedVote = false;
                                    __result = "The Terrorist has taken over the vote!";
                                }
                            }
                        }
                        return;
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            if (ExileController.Instance.initData.networkedPlayer == null) return;
                            var info = ExileController.Instance.initData.networkedPlayer;
                            var role = GetRole(info.Object);
                            foreach (var role2 in Role.GetRoles(RoleEnum.Reviver))
                            {
                                var reviver = (Reviver)role2;
                                if (reviver.UsedRevive && info.Object.PlayerId == reviver.Player.PlayerId)
                                {
                                    Utils.Unmorph(reviver.Player);
                                }
                                else if (!reviver.UsedRevive && info.Object.PlayerId == reviver.Player.PlayerId)
                                {
                                    reviver.UsedRevive = true;
                                }       
                            }
                            if (role == null) return;
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
                            __result = $"{info.PlayerName} was {roleName}.";
                            return;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {

            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    player.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                    var role = GetRole(player);
                    if (role != null)
                    {
                        bool selfFlag = role.SelfCriteria();
                        bool deadFlag = role.DeadCriteria();
                        bool impostorFlag = role.ImpostorCriteria();
                        bool madmateFlag = role.MadmateCriteria();
                        bool vampireFlag = role.VampireCriteria();
                        bool serialFlag = role.SerialKillerCriteria();
                        var covenFlag = role.CovenCriteria();
                        bool loverFlag = role.LoverCriteria();
                        bool roleFlag = role.RoleCriteria();
                        bool seerFlag = role.SeerCriteria();
                        bool gaFlag = role.GuardianAngelCriteria();
                        player.NameText.text = role.NameText(
                            selfFlag || deadFlag || role.Local,
                            selfFlag || deadFlag || impostorFlag || madmateFlag || vampireFlag || serialFlag || covenFlag || roleFlag || seerFlag || gaFlag,
                            selfFlag || deadFlag,
                            loverFlag,
                            player
                        );
                        if(role.ColorCriteria())
                            player.NameText.color = role.Color;
                    }
                    else
                    {
                        try
                        {
                            player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor()))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = Color.white;
                    }

                    var role = GetRole(player);
                    if (role != null)
                    {
                        if (role.Criteria())
                        {
                            bool selfFlag = role.SelfCriteria();
                            bool deadFlag = role.DeadCriteria();
                            bool impostorFlag = role.ImpostorCriteria();
                            bool madmateFlag = role.MadmateCriteria();
                            bool vampireFlag = role.VampireCriteria();
                            bool serialFlag = role.SerialKillerCriteria();
                            var covenFlag = role.CovenCriteria();
                            bool loverFlag = role.LoverCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool seerFlag = role.SeerCriteria();
                            bool gaFlag = role.GuardianAngelCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || madmateFlag || vampireFlag || serialFlag || covenFlag || roleFlag || seerFlag || gaFlag,
                                selfFlag || deadFlag,
                                loverFlag
                             );

                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                        }
                    }
                }
            }
        }
        public static AudioClip GetIntroSound(RoleTypes roleType)
        {
            return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleType).FirstOrDefault().IntroSound;
        }
    }
}