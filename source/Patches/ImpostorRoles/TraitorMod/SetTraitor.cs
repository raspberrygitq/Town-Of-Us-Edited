using HarmonyLib;
using TownOfUsEdited.Roles;
using System.Linq;
using TownOfUsEdited.CrewmateRoles.InvestigatorMod;
using TownOfUsEdited.CrewmateRoles.SnitchMod;
using TownOfUsEdited.Extensions;
using UnityEngine;
using Reactor.Utilities;
using TownOfUsEdited.Patches;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;
using Assassin = TownOfUsEdited.Roles.Modifiers.Assassin;
using Assassin2 = TownOfUsEdited.Roles.Assassin;
using TownOfUsEdited.NeutralRoles.SoulCollectorMod;

namespace TownOfUsEdited.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(AirshipExileController._WrapUpAndSpawn_d__11), nameof(AirshipExileController._WrapUpAndSpawn_d__11.MoveNext))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController._WrapUpAndSpawn_d__11 __instance) => SetTraitor.ExileControllerPostfix(__instance.__4__this);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetTraitor
    {
        public static PlayerControl WillBeTraitor;
        public static Sprite Sprite => TownOfUsEdited.Arrow;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData?.networkedPlayer?.Object;
            var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
            var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors)).ToList();
            foreach (var player in alives)
            {
                if (player.Data.IsImpostor() || (player.Is(Faction.NeutralKilling) && CustomGameOptions.NeutralKillingStopsTraitor))
                {
                    return;
                }
            }
            if (WillBeTraitor.Is(RoleEnum.Warden))
            {
                var warden = Role.GetRole<Warden>(WillBeTraitor);
                if (warden.Fortified != null) ShowShield.ResetVisor(warden.Fortified, warden.Player);
            }

            if (WillBeTraitor.Is(RoleEnum.Medic))
            {
                var medic = Role.GetRole<Medic>(WillBeTraitor);
                if (medic.ShieldedPlayer != null) ShowShield.ResetVisor(medic.ShieldedPlayer, medic.Player);
            }

            if (WillBeTraitor.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(WillBeTraitor);
                if (cleric.Barriered != null) cleric.UnBarrier();
            }

            if (WillBeTraitor.Is(RoleEnum.Plumber))
            {
                var plumberRole = Role.GetRole<Plumber>(WillBeTraitor);
                foreach (GameObject barricade in plumberRole.Barricades)
                {
                    UnityEngine.Object.Destroy(barricade);
                }
            }
            if (PlayerControl.LocalPlayer.Data.IsDead || exiled == PlayerControl.LocalPlayer) return;
            if (alives.Count < CustomGameOptions.LatestSpawn) return;
            if (impostors.Count == 0) return;
            if (PlayerControl.LocalPlayer != WillBeTraitor) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                {
                    var snitchRole = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
                    snitchRole.ImpArrows.DestroyAll();
                    snitchRole.SnitchArrows.Values.DestroyAll();
                    snitchRole.SnitchArrows.Clear();
                    CompleteTask.Postfix(PlayerControl.LocalPlayer);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

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
                    Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    Object.Destroy(trackerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                {
                    var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                    Object.Destroy(transporterRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                {
                    var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                    Object.Destroy(trapperRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                {
                    var aurialRole = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                    aurialRole.SenseArrows.Values.DestroyAll();
                    aurialRole.SenseArrows.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary))
                {
                    var mercRole = Role.GetRole<Mercenary>(PlayerControl.LocalPlayer);
                    mercRole.GuardButton.SetTarget(null);
                    mercRole.GuardButton.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(mercRole.UsesText);
                    UnityEngine.Object.Destroy(mercRole.GoldText);
                }

                var oldRole = Role.GetRole(PlayerControl.LocalPlayer);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                var role = new Traitor(PlayerControl.LocalPlayer);
                if (StartImitate.ImitatingPlayers.Contains(WillBeTraitor.PlayerId))
                {
                    role.formerRole = RoleEnum.Imitator;
                    StartImitate.ImitatingPlayers.Remove(WillBeTraitor.PlayerId);
                }
                else role.formerRole = oldRole.RoleType;
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.RegenTask();

                Utils.Rpc(CustomRPC.TraitorSpawn);

                TurnImp(PlayerControl.LocalPlayer);
            }
        }

        public static void TurnImp(PlayerControl player)
        {
            Role.GetRole(player).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }

            if (CustomGameOptions.TraitorCanAssassin && !CustomGameOptions.AssassinImpostorRole) new Assassin(player);

            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
                Coroutines.Start(Utils.FlashCoroutine(Color.red, 3f));
            }

            foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
            {
                var snitchRole = (Snitch)snitch;
                if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.SnitchArrows.Add(player.PlayerId, arrow);
                }
                else if (snitchRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitchRole.ImpArrows.Add(arrow);
                }
            }

            foreach (var haunter in Role.GetRoles(RoleEnum.Haunter))
            {
                var haunterRole = (Haunter)haunter;
                if (haunterRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Traitor))
                {
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = Sprite;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    haunterRole.ImpArrows.Add(arrow);
                }
            }
        }

        // Might be triggered using a button in the future instead of right after the meeting, that way you can change your role whenever you want
        public static void StartChangeRole(PlayerControl player)
        {
            if (!player.Is(RoleEnum.Traitor)) return;
            if (player != PlayerControl.LocalPlayer) return;
            var role = Role.GetRole<Traitor>(PlayerControl.LocalPlayer);
            if (!role.CanBeRoles.Contains(RoleEnum.Impostor)) role.CanBeRoles.Add(RoleEnum.Impostor);
            if (CustomGameOptions.JanitorOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Janitor)) role.CanBeRoles.Add(RoleEnum.Janitor);
            if (CustomGameOptions.MorphlingOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Morphling)) role.CanBeRoles.Add(RoleEnum.Morphling);
            if (CustomGameOptions.MinerOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Miner)) role.CanBeRoles.Add(RoleEnum.Miner);
            if (CustomGameOptions.SwooperOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Swooper)) role.CanBeRoles.Add(RoleEnum.Swooper);
            if (CustomGameOptions.UndertakerOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Undertaker)) role.CanBeRoles.Add(RoleEnum.Undertaker);
            if (CustomGameOptions.EscapistOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Escapist)) role.CanBeRoles.Add(RoleEnum.Escapist);
            if (CustomGameOptions.GrenadierOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Grenadier)) role.CanBeRoles.Add(RoleEnum.Grenadier);
            if (CustomGameOptions.BlackmailerOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Blackmailer)) role.CanBeRoles.Add(RoleEnum.Blackmailer);
            if (CustomGameOptions.BomberOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Bomber)) role.CanBeRoles.Add(RoleEnum.Bomber);
            if (CustomGameOptions.WarlockOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Warlock)) role.CanBeRoles.Add(RoleEnum.Warlock);
            if (CustomGameOptions.VenererOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Venerer)) role.CanBeRoles.Add(RoleEnum.Venerer);
            if (CustomGameOptions.HypnotistOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Hypnotist)) role.CanBeRoles.Add(RoleEnum.Hypnotist);
            if (CustomGameOptions.AssassinOn > 0 && CustomGameOptions.AssassinImpostorRole && !role.CanBeRoles.Contains(RoleEnum.Assassin)) role.CanBeRoles.Add(RoleEnum.Assassin);
            if (CustomGameOptions.WitchOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Witch)) role.CanBeRoles.Add(RoleEnum.Witch);
            if (CustomGameOptions.PoisonerOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Poisoner)) role.CanBeRoles.Add(RoleEnum.Poisoner);
            if (CustomGameOptions.ShooterOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Shooter)) role.CanBeRoles.Add(RoleEnum.Shooter);
            if (CustomGameOptions.ConverterOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Converter)) role.CanBeRoles.Add(RoleEnum.Converter);
            if (CustomGameOptions.ManipulatorOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Manipulator)) role.CanBeRoles.Add(RoleEnum.Manipulator);
            if (CustomGameOptions.ConjurerOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Conjurer)) role.CanBeRoles.Add(RoleEnum.Conjurer);
            if (CustomGameOptions.BountyHunterOn > 0 && !role.CanBeRoles.Contains(RoleEnum.BountyHunter)) role.CanBeRoles.Add(RoleEnum.BountyHunter);
            if (CustomGameOptions.ReviverOn > 0 && !role.CanBeRoles.Contains(RoleEnum.Reviver)) role.CanBeRoles.Add(RoleEnum.Reviver);
            PlayerControl.LocalPlayer.NetTransform.Halt();
            if (role.CanBeRoles.Count > 0)
            {
                role.CanBeRoles.Shuffle();
                while (role.CanBeRoles.Count > 3) role.CanBeRoles.RemoveAt(0);
                if (role.SelectedRoles.Count <= 0) role.SelectedRoles = role.CanBeRoles.ToArray().ToList();
                var pk = new TraitorMenu((x) =>
                {
                    RoleEnum selectedRole = x;
                    Utils.Rpc(CustomRPC.AddTraitorRole, PlayerControl.LocalPlayer.PlayerId, (byte)selectedRole);
                    ChangeRole(role, selectedRole);
                });
                Coroutines.Start(pk.Open(0f));
            }
        }

        public static void ChangeRole(Traitor role, RoleEnum selectedRole)
        {
            var traitor = role.Player;
            if (PlayerControl.LocalPlayer == traitor) role.ChangeRoleButton.gameObject.SetActive(false);
            var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            var formerRole = role.formerRole;
            Role.RoleDictionary.Remove(traitor.PlayerId);
            if (selectedRole == RoleEnum.Janitor) new Janitor(traitor);
            else if (selectedRole == RoleEnum.Morphling) new Morphling(traitor);
            else if (selectedRole == RoleEnum.Miner) new Miner(traitor);
            else if (selectedRole == RoleEnum.Swooper) new Swooper(traitor);
            else if (selectedRole == RoleEnum.Undertaker) new Undertaker(traitor);
            else if (selectedRole == RoleEnum.Escapist) new Escapist(traitor);
            else if (selectedRole == RoleEnum.Grenadier) new Grenadier(traitor);
            else if (selectedRole == RoleEnum.Blackmailer) new Blackmailer(traitor);
            else if (selectedRole == RoleEnum.Bomber) new Bomber(traitor);
            else if (selectedRole == RoleEnum.Warlock) new Warlock(traitor);
            else if (selectedRole == RoleEnum.Venerer) new Venerer(traitor);
            else if (selectedRole == RoleEnum.Hypnotist) new Hypnotist(traitor);
            else if (selectedRole == RoleEnum.Assassin)
            {
                new Assassin(traitor);
                new Assassin2(traitor);
            }
            else if (selectedRole == RoleEnum.Witch) new Witch(traitor);
            else if (selectedRole == RoleEnum.Poisoner) new Poisoner(traitor);
            else if (selectedRole == RoleEnum.Shooter) new Shooter(traitor);
            else if (selectedRole == RoleEnum.Converter) new Converter(traitor);
            else if (selectedRole == RoleEnum.Manipulator) new Manipulator(traitor);
            else if (selectedRole == RoleEnum.Conjurer) new Conjurer(traitor);
            else if (selectedRole == RoleEnum.BountyHunter) new BountyHunter(traitor);
            else if (selectedRole == RoleEnum.Reviver) new Reviver(traitor);
            else new Impostor(traitor);
            var newRole = Role.GetRole(traitor);
            newRole.Kills = killsList.Kills;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectKills = killsList.IncorrectKills;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            newRole.formerRole = formerRole;
            newRole.Name = "Traitor";
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            if (PlayerControl.LocalPlayer == traitor) newRole.RegenTask();
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new System.Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}