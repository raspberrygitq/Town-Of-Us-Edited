using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs
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
        private static Sprite Dissociate => TownOfUs.Dissociate;
        private static Sprite Rewind => TownOfUs.Rewind;
        private static Sprite Campaign => TownOfUs.CampaignSprite;
        private static Sprite Fortify => TownOfUs.FortifySprite;
        private static Sprite Collect => TownOfUs.CollectSprite;
        private static Sprite Eat => TownOfUs.Eat;
        private static Sprite Fix => TownOfUs.EngineerFix;
        private static Sprite Light => TownOfUs.Light;
        private static Sprite Protect2 => TownOfUs.Protect2Sprite;
        private static Sprite Poison => TownOfUs.PoisonSprite;
        private static Sprite RevealRole => TownOfUs.RevealRoleSprite;
        private static Sprite Jail => TownOfUs.JailSprite;
        private static Sprite ZoomButton => TownOfUs.CapZoomButton;
        private static Sprite DocReviveButton => TownOfUs.DocReviveButton;
        private static Sprite Medic => TownOfUs.MedicSprite;
        private static Sprite Seer => TownOfUs.SeerSprite;
        private static Sprite Douse => TownOfUs.DouseSprite;
        private static Sprite Revive => TownOfUs.ReviveSprite;
        private static Sprite Alert => TownOfUs.AlertSprite;
        private static Sprite Remember => TownOfUs.RememberSprite;
        private static Sprite Track => TownOfUs.TrackSprite;
        private static Sprite Transport => TownOfUs.TransportSprite;
        private static Sprite Mediate => TownOfUs.MediateSprite;
        private static Sprite Vest => TownOfUs.VestSprite;
        private static Sprite Protect => TownOfUs.ProtectSprite;
        private static Sprite Infect => TownOfUs.InfectSprite;
        private static Sprite Trap => TownOfUs.TrapSprite;
        private static Sprite Inspect => TownOfUs.InspectSprite;
        private static Sprite Swoop => TownOfUs.SwoopSprite;
        private static Sprite Observe => TownOfUs.ObserveSprite;
        private static Sprite Stake => TownOfUs.StakeSprite;
        private static Sprite Confess => TownOfUs.ConfessSprite;
        private static Sprite Radiate => TownOfUs.RadiateSprite;
        private static Sprite Hex => TownOfUs.Hex;
        private static Sprite Camp => TownOfUs.CampSprite;
        private static Sprite Watch => TownOfUs.WatchSprite;

        private static Sprite Kill;


        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer) || PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
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
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Informant))
            {
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
                __instance.KillButton.graphic.sprite = Protect;
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lighter))
            {
                __instance.KillButton.graphic.sprite = Light;
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
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                __instance.KillButton.graphic.sprite = Swoop;
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
                __instance.KillButton.graphic.sprite = Protect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.HexMaster))
            {
                __instance.KillButton.graphic.sprite = Hex;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                __instance.KillButton.graphic.sprite = Collect;
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
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Doppelganger))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Player))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Maul))
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
            if (role?.ExtraButtons != null && AbilityKey && !PlayerControl.LocalPlayer.Data.IsDead)
                role?.ExtraButtons[0]?.DoClick();

            if ((KillKey || controller) && PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner) && !PlayerControl.LocalPlayer.Data.IsDead)
                role?.ExtraButtons[0]?.DoClick();

            if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") &&
                !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonButton.DoClick();
            }
            else if (Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                     Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") &&
                     !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer).DisperseButton.DoClick();
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
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
                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && Utils.ShowDeadBodies && !MeetingHud.Instance);
            }
        }
    }
}
