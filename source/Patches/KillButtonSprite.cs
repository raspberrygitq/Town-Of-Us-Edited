using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;

namespace TownOfUsEdited
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        private static Sprite Dissociate => TownOfUsEdited.Dissociate;
        private static Sprite Rewind => TownOfUsEdited.Rewind;
        private static Sprite Campaign => TownOfUsEdited.CampaignSprite;
        private static Sprite Fortify => TownOfUsEdited.FortifySprite;
        private static Sprite Reap => TownOfUsEdited.ReapSprite;
        private static Sprite Eat => TownOfUsEdited.Eat;
        private static Sprite Fix => TownOfUsEdited.EngineerFix;
        private static Sprite Protect2 => TownOfUsEdited.Protect2Sprite;
        private static Sprite Poison => TownOfUsEdited.PoisonSprite;
        private static Sprite RevealRole => TownOfUsEdited.RevealRoleSprite;
        private static Sprite Jail => TownOfUsEdited.JailSprite;
        private static Sprite ZoomButton => TownOfUsEdited.CapZoomButton;
        private static Sprite DocReviveButton => TownOfUsEdited.DocReviveButton;
        private static Sprite Medic => TownOfUsEdited.MedicSprite;
        private static Sprite Seer => TownOfUsEdited.SeerSprite;
        private static Sprite Douse => TownOfUsEdited.DouseSprite;
        private static Sprite Revive => TownOfUsEdited.ReviveSprite;
        private static Sprite Alert => TownOfUsEdited.AlertSprite;
        private static Sprite Remember => TownOfUsEdited.RememberSprite;
        private static Sprite Shift => TownOfUsEdited.ShiftButton;
        private static Sprite Transport => TownOfUsEdited.TransportSprite;
        private static Sprite Mediate => TownOfUsEdited.MediateSprite;
        private static Sprite Vest => TownOfUsEdited.VestSprite;
        private static Sprite Protect => TownOfUsEdited.ProtectSprite;
        private static Sprite Infect => TownOfUsEdited.InfectSprite;
        private static Sprite Trap => TownOfUsEdited.TrapSprite;
        private static Sprite Inspect => TownOfUsEdited.InspectSprite;
        private static Sprite Track => TownOfUsEdited.TrackSprite;
        public static Sprite ChameleonSwoop => TownOfUsEdited.ChameleonSwoop;

        private static Sprite Observe => TownOfUsEdited.ObserveSprite;
        private static Sprite Stake => TownOfUsEdited.StakeSprite;
        private static Sprite Confess => TownOfUsEdited.ConfessSprite;
        private static Sprite Radiate => TownOfUsEdited.RadiateSprite;
        private static Sprite Hex => TownOfUsEdited.Hex;
        private static Sprite Camp => TownOfUsEdited.CampSprite;
        private static Sprite Watch => TownOfUsEdited.WatchSprite;
        private static Sprite Block => TownOfUsEdited.BlockSprite;
        private static Sprite Bribe => TownOfUsEdited.BribeSprite;
        private static Sprite Barrier => TownOfUsEdited.BarrierSprite;

        private static Sprite Kill;


        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            if (!__instance.KillButton.isActiveAndEnabled) return;
            __instance.KillButton.graphic.SetCooldownNormalizedUvs();

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                flag = true;
                if (CustomGameOptions.GameMode != GameMode.Werewolf)
                {
                    __instance.KillButton.graphic.sprite = Seer;
                }
                else __instance.KillButton.graphic.sprite = RevealRole;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = Medic;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = Douse;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
            {
                __instance.KillButton.graphic.sprite = Revive;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Astral))
            {
                __instance.KillButton.graphic.sprite = Dissociate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord))
            {
                __instance.KillButton.graphic.sprite = Rewind;
                __instance.KillButton.buttonLabelText.text = "Rewind";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                __instance.KillButton.buttonLabelText.text = "Remember";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                __instance.KillButton.buttonLabelText.text = "Track";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Informant) || PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
            {
                __instance.KillButton.buttonLabelText.text = "Admin";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = Mediate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                __instance.KillButton.graphic.sprite = Vest;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.buttonLabelText.text = "Protect";
                var ga = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.GuardianAngel).Cast<GuardianAngelRole>();
                __instance.KillButton.graphic.sprite = ga.Ability.Image;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = Infect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = Fix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = Trap;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Captain))
            {
                __instance.KillButton.graphic.sprite = ZoomButton;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                __instance.KillButton.graphic.sprite = Jail;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
            {
                __instance.KillButton.graphic.sprite = Campaign;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Warden))
            {
                __instance.KillButton.graphic.sprite = Fortify;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doctor))
            {
                __instance.KillButton.graphic.sprite = DocReviveButton;
                __instance.KillButton.buttonLabelText.text = "Revive";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                __instance.KillButton.graphic.sprite = TownOfUsEdited.ChameleonSwoop;
                __instance.KillButton.buttonLabelText.text = "Swoop";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                __instance.KillButton.graphic.sprite = Observe;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                __instance.KillButton.graphic.sprite = Stake;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = Confess;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
            {
                __instance.KillButton.graphic.sprite = Radiate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Shifter))
            {
                __instance.KillButton.graphic.sprite = Shift;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Sorcerer))
            {
                __instance.KillButton.graphic.sprite = Poison;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guard))
            {
                __instance.KillButton.graphic.sprite = Protect2;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                __instance.KillButton.buttonLabelText.text = "Eat";
                __instance.KillButton.graphic.sprite = Eat;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Helper))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Guardian))
            {
                __instance.KillButton.buttonLabelText.text = "Protect";
                var ga = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.GuardianAngel).Cast<GuardianAngelRole>();
                __instance.KillButton.graphic.sprite = ga.Ability.Image;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.HexMaster))
            {
                __instance.KillButton.buttonLabelText.text = "Hex";
                __instance.KillButton.graphic.sprite = Hex;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                __instance.KillButton.graphic.sprite = Reap;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plumber))
            {
                __instance.KillButton.graphic.sprite = Block;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary))
            {
                __instance.KillButton.graphic.sprite = Bribe;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cleric))
            {
                __instance.KillButton.graphic.sprite = Barrier;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                __instance.KillButton.graphic.sprite = Camp;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                __instance.KillButton.graphic.sprite = Watch;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
            {
                __instance.KillButton.graphic.sprite = Poison;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Kill";
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Maul) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Fighter) || PlayerControl.LocalPlayer.Is(RoleEnum.Knight) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Mutant) || PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) ||
                    PlayerControl.LocalPlayer.Is(Faction.Impostors) || PlayerControl.LocalPlayer.Is(RoleEnum.Troll) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.WhiteWolf) || PlayerControl.LocalPlayer.Is(RoleEnum.Player) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Crusader) || PlayerControl.LocalPlayer.Is(RoleEnum.Avenger) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Terrorist) || PlayerControl.LocalPlayer.Is(RoleEnum.Hunter) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Vampire) || PlayerControl.LocalPlayer.Is(Faction.Coven) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Infectious) || PlayerControl.LocalPlayer.Is(RoleEnum.Doppelganger);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
            {
                __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Vampire) || PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Mutant) || (PlayerControl.LocalPlayer.Is(RoleEnum.Doctor) && PlayerControl.LocalPlayer.Is(Faction.Madmates))
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Paranoïac) || PlayerControl.LocalPlayer.Is(RoleEnum.Terrorist)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Vulture) || PlayerControl.LocalPlayer.Is(RoleEnum.CovenLeader)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.PotionMaster) || PlayerControl.LocalPlayer.Is(RoleEnum.Infectious)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Doppelganger) || PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Player))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Maul) || PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)
            || PlayerControl.LocalPlayer.Is(RoleEnum.Plumber))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(Faction.Impostors))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }

            bool KillKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Kill");
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if ((KillKey || controller) && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            bool AbilityKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU imp/nk");
            var controller2 = ConsoleJoystick.player.GetButtonDown(49); // they use this for vanilla role ability button normally
            if (role?.ExtraButtons != null && (AbilityKey || controller2) && !PlayerControl.LocalPlayer.Data.IsDead)
                role?.ExtraButtons[0]?.DoClick();

            var controllerRBLB = ConsoleJoystick.player.GetButtonDown(20) || ConsoleJoystick.player.GetButtonDown(22);
            if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") ||
                controllerRBLB) &&
                !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonButton.DoClick();
            }
            else if (Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                     (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") ||
                     controllerRBLB) &&
                     !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer).DisperseButton.DoClick();
            }
            else if (Modifier.GetModifier<Satellite>(PlayerControl.LocalPlayer)?.DetectUsed == false &&
                     (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") ||
                     controllerRBLB) &&
                     !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<Satellite>(PlayerControl.LocalPlayer).DetectButton.DoClick();
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }
                var ghostRole = false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (!haunter.Caught) ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (!phantom.Caught) ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Spirit))
                {
                    var spirit = Role.GetRole<Spirit>(PlayerControl.LocalPlayer);
                    if (!spirit.Caught) ghostRole = true;
                }
                var ghost = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost).Cast<CrewmateGhostRole>();
                HudManager.Instance.AbilityButton.graphic.sprite = ghost.Ability.Image;
                HudManager.Instance.AbilityButton.SetInfiniteUses();
                HudManager.Instance.AbilityButton.buttonLabelText.text = "Haunt";
                HudManager.Instance.AbilityButton.buttonLabelText.SetOutlineColor(Palette.CrewmateBlue);
                HudManager.Instance.AbilityButton.gameObject.SetActive(PlayerControl.LocalPlayer.Data.IsDead && !ghostRole && Utils.ShowDeadBodies && !MeetingHud.Instance);
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        class AbilityButtonDoClickPatch
        {
            static bool Prefix(AbilityButton __instance)
            {
                if (__instance != HudManager.Instance.AbilityButton) return true;
                if (!PlayerControl.LocalPlayer.Data.IsDead) return false;
                if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
                {
                    return false;
                }
                if (Minigame.Instance)
                {
                    if (Minigame.Instance is HauntMenuMinigame)
                    {
                        Minigame.Instance.Close();
                    }
                    return false;
                }
                var ghost = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.CrewmateGhost).Cast<CrewmateGhostRole>();
                Minigame minigame = Object.Instantiate<Minigame>(ghost.HauntMenu);
                minigame.transform.SetParent(DestroyableSingleton<HudManager>.Instance.AbilityButton.transform, false);
                minigame.transform.SetLocalZ(-5f);
                minigame.Begin(null);
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetDisabled();
                return false;
            }
        }
    }
}
