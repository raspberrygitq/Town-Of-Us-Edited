using System;
using HarmonyLib;
using TownOfUsEdited.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsEdited.Patches;
using System.Linq;
using TownOfUsEdited.Extensions;

namespace TownOfUsEdited.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartImitate.ExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class StartImitate
    {
        public static PlayerControl ImitatingPlayer;
        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (exiled == PlayerControl.LocalPlayer) return;

            var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitator.ImitatePlayer == null) return;

            Imitate(imitator);

            Utils.Rpc(CustomRPC.StartImitate, imitator.Player.PlayerId);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static Sprite Sprite => TownOfUsEdited.Arrow;

        public static void Imitate(Imitator imitator)
        {
            if (imitator.ImitatePlayer == null) return;
            ImitatingPlayer = imitator.Player;
            var imitatorRole = Role.GetRole(imitator.ImitatePlayer).RoleType;
            if (imitatorRole == RoleEnum.Haunter)
            {
                var haunter = Role.GetRole<Haunter>(imitator.ImitatePlayer);
                imitatorRole = haunter.formerRole;
            }
            if (imitatorRole == RoleEnum.Helper)
            {
                var helper = Role.GetRole<Helper>(imitator.ImitatePlayer);
                imitatorRole = helper.formerRole;
            }
            if (imitatorRole == RoleEnum.Guardian)
            {
                var guardian = Role.GetRole<Guardian>(imitator.ImitatePlayer);
                imitatorRole = guardian.formerRole;
            }
            if (imitatorRole == RoleEnum.Crewmate) return;
            var role = Role.GetRole(ImitatingPlayer);
            var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            Role.RoleDictionary.Remove(ImitatingPlayer.PlayerId);
            if (imitatorRole == RoleEnum.Detective) new Detective(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Captain) new Captain(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Doctor) new Doctor(ImitatingPlayer);
            if (imitatorRole == RoleEnum.TimeLord) new TimeLord(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Superstar) new Superstar(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Avenger) new Avenger(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Investigator) new Investigator(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Mystic) new Mystic(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Seer) new Seer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Spy) new Spy(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Tracker) new Tracker(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Sheriff) new Sheriff(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Veteran) new Veteran(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Astral) new Astral(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Altruist) new Altruist(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Engineer) new Engineer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Lighter) new Lighter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medium) new Medium(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Transporter) new Transporter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Informant) new Informant(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Trapper) new Trapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Oracle) new Oracle(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Aurial) new Aurial(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Crewmate) new Crewmate(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Lookout) new Lookout(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Crusader) new Crusader(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Paranoïac) new Paranoïac(ImitatingPlayer);

            
            else if (imitatorRole == RoleEnum.Snitch)
            {
                var snitch = new Snitch(ImitatingPlayer);
                var taskinfos = ImitatingPlayer.Data.Tasks.ToArray();
                var tasksLeft = taskinfos.Count(x => !x.Complete);
                if (tasksLeft <= CustomGameOptions.SnitchTasksRemaining && ((PlayerControl.LocalPlayer.Data.IsImpostor() && (!PlayerControl.LocalPlayer.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
                            || (PlayerControl.LocalPlayer.Is(Faction.NeutralKilling) && CustomGameOptions.SnitchSeesNeutrals)))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.ImpArrows.Add(arrow);
                }
                else if (tasksLeft == 0 && PlayerControl.LocalPlayer == ImitatingPlayer)
                {
                    var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsImpostor());
                    foreach (var imp in impostors)
                    {
                        if (!imp.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor)
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            snitch.SnitchArrows.Add(imp.PlayerId, arrow);
                        }
                    }
                }
            }
            else if (imitatorRole == RoleEnum.Deputy) new Deputy(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Hunter) new Hunter(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Jailor) new Jailor(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Vigilante) new Vigilante(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Warden) new Warden(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Mayor)
            {
                var mayor = new Mayor(ImitatingPlayer);
                if (CustomGameOptions.ImitatorCanBecomeMayor)
                {
                    mayor.Revealed = true;
                    if (PlayerControl.LocalPlayer == ImitatingPlayer) mayor.RegenTask();
                }
            }
            else if (imitatorRole == RoleEnum.Politician) new Politician(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Prosecutor) new Prosecutor(ImitatingPlayer);
            else if (imitatorRole == RoleEnum.Swapper) new Swapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medic)
            {
                var medic = new Medic(ImitatingPlayer);
                medic.UsedAbility = true;
                medic.StartingCooldown = medic.StartingCooldown.AddSeconds(-10f);
            }
            if (imitatorRole == RoleEnum.VampireHunter)
            {
                var vh = new VampireHunter(ImitatingPlayer);
                vh.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                vh.AddedStakes = true;
            }
            if (imitatorRole == RoleEnum.Knight)
            {
                var knightRole = new Knight(ImitatingPlayer);
                knightRole.Cooldown = CustomGameOptions.KnightKCD;
                knightRole.UsesLeft = 0;
            }

            var newRole = Role.GetRole(ImitatingPlayer);
            if (imitatorRole != RoleEnum.Mayor || !CustomGameOptions.ImitatorCanBecomeMayor) newRole.RemoveFromRoleHistory(newRole.RoleType);
            else ImitatingPlayer = null;
            newRole.Kills = killsList.Kills;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectKills = killsList.IncorrectKills;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            newRole.DeathReason = role.DeathReason;
        }
    }
}