using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUsEdited.CrewmateRoles.ImitatorMod;
using TownOfUsEdited.CrewmateRoles.InvestigatorMod;
using TownOfUsEdited.CrewmateRoles.SnitchMod;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Modifiers.AssassinMod;
using TownOfUsEdited.NeutralRoles.SoulCollectorMod;
using TownOfUsEdited.Roles;
using UnityEngine;
using static TownOfUsEdited.Roles.Modifiers.Madmate;

namespace TownOfUsEdited.Patches.Modifiers.MadmateMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ImpostorDeathUpdate
    {
        public static void Prefix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            var aliveimps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (aliveimps.Count <= 0)
            {
                if ((CustomGameOptions.MadmateOnImpoDeath == BecomeMadmateOptions.Die || CustomGameOptions.GameMode == GameMode.Cultist) && !PlayerControl.LocalPlayer.Data.IsDead
                && !PlayerControl.LocalPlayer.Data.Disconnected && PlayerControl.LocalPlayer.Is(Faction.Madmates))
                {
                    if (!MeetingHud.Instance)
                    {
                        Utils.Interact(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true);
                        return;
                    }
                    else
                    {
                        AssassinKill.MurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                        Utils.Rpc(CustomRPC.AssassinKill, PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                        return;
                    }
                }
                if (CustomGameOptions.GameMode == GameMode.Cultist) return;
                else if (CustomGameOptions.MadmateOnImpoDeath == BecomeMadmateOptions.OriginalFaction && PlayerControl.LocalPlayer.Is(Faction.Madmates))
                {
                    Utils.TurnCrewmateTeam(PlayerControl.LocalPlayer);
                    Utils.Rpc(CustomRPC.TurnCrewmateTeam, PlayerControl.LocalPlayer.PlayerId);
                    return;
                }
                else if (CustomGameOptions.MadmateOnImpoDeath == BecomeMadmateOptions.Impostor)
                {
                    if (!AmongUsClient.Instance.AmHost) return;
                    var toChooseFromMad = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Madmates) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    if (toChooseFromMad.Count != 0)
                    {
                        var rand = UnityEngine.Random.RandomRangeInt(0, toChooseFromMad.Count);
                        var pc = toChooseFromMad[rand];

                        var oldRole = Role.GetRole(pc);
                        var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                        Role.RoleDictionary.Remove(pc.PlayerId);
                        var role = new Impostor(pc);
                        role.CorrectKills = killsList.CorrectKills;
                        role.IncorrectKills = killsList.IncorrectKills;
                        role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                        role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                        role.TaskText = () => "You have become the new Impostor, kill the remaining crew!";
                        role.RegenTask();

                        TurnImp(pc);
                        Utils.Rpc(CustomRPC.TurnImpostor, pc.PlayerId);
                    }
                }
            }
        }
        public static Sprite Sprite => TownOfUsEdited.Arrow;
        public static void TurnImp(PlayerControl player)
        {
            if (player.Is(RoleEnum.Warden))
            {
                var warden = Role.GetRole<Warden>(player);
                if (warden.Fortified != null) ShowShield.ResetVisor(warden.Fortified, warden.Player);
            }

            if (player.Is(RoleEnum.Medic))
            {
                var medic = Role.GetRole<Medic>(player);
                if (medic.ShieldedPlayer != null) ShowShield.ResetVisor(medic.ShieldedPlayer, medic.Player);
            }

            if (player.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(player);
                if (cleric.Barriered != null) cleric.UnBarrier();
            }

            if (player.Is(RoleEnum.Plumber))
            {
                var plumberRole = Role.GetRole<Plumber>(player);
                foreach (GameObject barricade in plumberRole.Barricades)
                {
                    UnityEngine.Object.Destroy(barricade);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                {
                    var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                    mysticRole.BodyArrows.Values.DestroyAll();
                    mysticRole.BodyArrows.Clear();
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

                if (StartImitate.ImitatingPlayers.Contains(PlayerControl.LocalPlayer.PlayerId)) StartImitate.ImitatingPlayers.Remove(PlayerControl.LocalPlayer.PlayerId);

                var oldRole = Role.GetRole(PlayerControl.LocalPlayer);
                var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                var role = new Impostor(PlayerControl.LocalPlayer);
                role.CorrectKills = killsList.CorrectKills;
                role.IncorrectKills = killsList.IncorrectKills;
                role.CorrectAssassinKills = killsList.CorrectAssassinKills;
                role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                role.TaskText = () => "You have become the new Impostor, kill the remaining crew!";
                role.RegenTask();
            }

            Role.GetRole(player).KillCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
                Coroutines.Start(Utils.FlashCoroutine(Color.red, 3f));
            }

            foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
            {
                var snitchRole = (Snitch)snitch;
                if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
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
                else if (snitchRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Impostor) && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected)
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
                if (haunterRole.Revealed && PlayerControl.LocalPlayer.Is(RoleEnum.Impostor) && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected)
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
    }
}