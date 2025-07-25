﻿using InnerNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUsEdited.Extensions;
using TownOfUsEdited.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsEdited.Patches;
using static TownOfUsEdited.DisableAbilities;
using TownOfUsEdited.Patches.NeutralRoles;
using TMPro;

namespace TownOfUsEdited.Roles
{
    public class Glitch : Role, IVisualAlteration
    {
        public static Sprite MimicSprite = TownOfUsEdited.MimicSprite;
        public static Sprite HackSprite = TownOfUsEdited.HackSprite;
        public static Sprite LockSprite = TownOfUsEdited.LockSprite;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "The Glitch";
            Color = Patches.Colors.Glitch;
            
            HackButton = null;
            MimicButton = null;
            KillTarget = null;
            HackTarget = null;
            IsUsingMimic = false;
            RoleType = RoleEnum.Glitch;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "Murder, Mimic, Hack... Data Lost";
            TaskText = () => "Murder everyone to win\nFake Tasks:";
            Faction = Faction.NeutralKilling;
            Cooldown = CustomGameOptions.GlitchKillCooldown;
            HackCooldown = CustomGameOptions.HackCooldown;
            MimicCooldown = CustomGameOptions.MimicCooldown;
        }

        public PlayerControl ClosestPlayer;
        public PlayerControl Hacked;
        public KillButton HackButton { get; set; }
        public KillButton MimicButton { get; set; }
        public PlayerControl KillTarget { get; set; }
        public PlayerControl HackTarget { get; set; }
        public TextMeshPro HackText;
        public bool IsUsingMimic { get; set; }
        public bool Enabled;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public float HackCooldown;
        public bool HackcoolingDown => HackCooldown > 0f;
        public float MimicCooldown;
        public bool MimiccoolingDown => MimicCooldown > 0f;

        public PlayerControl MimicTarget { get; set; }
        public bool GlitchWins { get; set; }

        internal override void NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var alivecrewkiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.Coven))) == 1 && (alivecrewkiller.Count <= 0 || !CustomGameOptions.CrewKillersContinue))
            {
                Utils.Rpc(CustomRPC.GlitchWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                PluginSingleton<TownOfUsEdited>.Instance.Log.LogMessage("GAME OVER REASON: Glitch Win");
                return;
            }

            return;
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

        public float HackTimer()
        {
            if (!HackcoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                HackCooldown -= Time.deltaTime;
                return HackCooldown;
            }
            else return HackCooldown;
        }

        public float MimicTimer()
        {
            if (!MimiccoolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                MimicCooldown -= Time.deltaTime;
                return MimicCooldown;
            }
            else return MimicCooldown;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Glitch Edition");
            GlitchWins = true;
            if (AmongUsClient.Instance.AmHost) Utils.EndGame();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var glitchTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            glitchTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = glitchTeam;
        }

        public void Update(HudManager __instance)
        {
            if (HudManager.Instance?.Chat != null)
            {
                foreach (var bubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null &&
                        Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                    {
                        bubble.Cast<ChatBubble>().NameText.color = Color;
                    }
                }
            }

            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            KillButtonHandler.KillButtonUpdate(this, __instance);

            MimicButtonHandler.MimicButtonUpdate(this, __instance);

            HackButtonHandler.HackButtonUpdate(this, __instance);

            if (__instance.KillButton != null && Player.Data.IsDead)
                __instance.KillButton.SetTarget(null);

            if (MimicButton != null && Player.Data.IsDead)
                MimicButton.SetTarget(null);

            if (HackButton != null && Player.Data.IsDead)
                HackButton.SetTarget(null);
        }

        public bool UseAbility(KillButton __instance)
        {
            if (__instance == HackButton)
                HackButtonHandler.HackButtonPress(this);
            else if (__instance == MimicButton)
                MimicButtonHandler.MimicButtonPress(this);
            else if (__instance == HudManager.Instance.KillButton)
                KillButtonHandler.KillButtonPress(this);

            return false;
        }

        public void RpcSetHacked(PlayerControl hacked)
        {
            Utils.Rpc(CustomRPC.SetHacked, Player.PlayerId, hacked.PlayerId);
            SetHacked(hacked);
        }

        public void SetHacked(PlayerControl hacked)
        {
            HackCooldown = CustomGameOptions.HackCooldown;
            Hacked = hacked;
        }

        public void RpcSetMimicked(PlayerControl mimicked)
        {
            Coroutines.Start(AbilityCoroutine.Mimic(this, mimicked));
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (IsUsingMimic)
            {
                appearance = MimicTarget.GetDefaultAppearance();
                var modifiers = Modifier.GetModifiers(MimicTarget);
                var modifier = modifiers.FirstOrDefault(x => x is IVisualAlteration);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public static class AbilityCoroutine
        {
            public static Dictionary<byte, DateTime> tickDictionary = new();

            public static IEnumerator Hack(PlayerControl hackPlayer)
            {
                foreach (var role in GetRoles(RoleEnum.Glitch))
                {
                    var glitch = (Glitch)role;
                    glitch.Hacked = null;
                }
                GameObject[] lockImg = { null, null, null };
                ImportantTextTask hackText;

                if (tickDictionary.ContainsKey(hackPlayer.PlayerId))
                {
                    tickDictionary[hackPlayer.PlayerId] = DateTime.UtcNow;
                    yield break;
                }

                hackText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                hackText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                hackText.Text =
                    $"{"<color=#" + Colors.Glitch.ToHtmlStringRGBA() + ">"}Hacked {hackPlayer.Data.PlayerName} ({CustomGameOptions.HackDuration}s)</color>";
                hackText.Index = hackPlayer.PlayerId;
                tickDictionary.Add(hackPlayer.PlayerId, DateTime.UtcNow);
                PlayerControl.LocalPlayer.myTasks.Insert(0, hackText);

                Coroutines.Start(DisableAbility.StopAbility(CustomGameOptions.HackDuration));

                while (true)
                {
                    if (PlayerControl.LocalPlayer == hackPlayer)
                    {
                        if (HudManager.Instance.KillButton != null)
                        {
                            if (lockImg[0] == null)
                            {
                                lockImg[0] = new GameObject();
                                var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[0].layer = 5;
                            lockImg[0].transform.position =
                                new Vector3(HudManager.Instance.KillButton.transform.position.x,
                                    HudManager.Instance.KillButton.transform.position.y, -50f);
                        }

                        var role = GetRole(PlayerControl.LocalPlayer);
                        if (role?.ExtraButtons.Count > 0)
                        {
                            if (lockImg[1] == null)
                            {
                                lockImg[1] = new GameObject();
                                var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[1].transform.position = new Vector3(
                                role.ExtraButtons[0].transform.position.x,
                                role.ExtraButtons[0].transform.position.y, -50f);
                            lockImg[1].layer = 5;
                        }

                        if (HudManager.Instance.ReportButton != null)
                        {
                            if (lockImg[2] == null)
                            {
                                lockImg[2] = new GameObject();
                                var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[2].transform.position =
                                new Vector3(HudManager.Instance.ReportButton.transform.position.x,
                                    HudManager.Instance.ReportButton.transform.position.y, -50f);
                            lockImg[2].layer = 5;
                            HudManager.Instance.ReportButton.enabled = false;
                            HudManager.Instance.ReportButton.SetActive(false);
                        }
                    }

                    var totalHacktime = (DateTime.UtcNow - tickDictionary[hackPlayer.PlayerId]).TotalMilliseconds /
                                        1000;
                    hackText.Text =
                        $"{"<color=#" + Colors.Glitch.ToHtmlStringRGBA() + ">"}Hacked {hackPlayer.Data.PlayerName} ({CustomGameOptions.HackDuration - Math.Round(totalHacktime)}s)</color>";
                    if (MeetingHud.Instance || totalHacktime > CustomGameOptions.HackDuration || hackPlayer?.Data.IsDead != false || (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started &&
                    AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay))
                    {
                        foreach (var obj in lockImg)
                        {
                            obj?.SetActive(false);
                        }

                        if (PlayerControl.LocalPlayer == hackPlayer)
                        {
                            HudManager.Instance.ReportButton.enabled = true;
                        }

                        tickDictionary.Remove(hackPlayer.PlayerId);
                        PlayerControl.LocalPlayer.myTasks.Remove(hackText);
                        yield break;
                    }

                    yield return null;
                }
            }

            public static IEnumerator Mimic(Glitch __instance, PlayerControl mimicPlayer)
            {
                Utils.Rpc(CustomRPC.SetMimic, PlayerControl.LocalPlayer.PlayerId, mimicPlayer.PlayerId);

                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) yield break;
                if (mimicPlayer == null || mimicPlayer.Data.Disconnected) yield break;

                Utils.Morph(__instance.Player, mimicPlayer);

                var mimicActivation = DateTime.UtcNow;
                var mimicText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                mimicText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                mimicText.Text =
                    $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName} ({CustomGameOptions.MimicDuration}s)</color>";
                PlayerControl.LocalPlayer.myTasks.Insert(0, mimicText);

                while (true)
                {
                    __instance.Enabled = true;
                    __instance.IsUsingMimic = true;
                    __instance.MimicTarget = mimicPlayer;
                    var totalMimickTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;
                    if (__instance.Player.Data.IsDead)
                    {
                        totalMimickTime = CustomGameOptions.MimicDuration;
                    }
                    mimicText.Text =
                        $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName} ({CustomGameOptions.MimicDuration - Math.Round(totalMimickTime)}s)</color>";
                    if (totalMimickTime > CustomGameOptions.MimicDuration ||
                        PlayerControl.LocalPlayer.Data.IsDead ||
                        AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
                    {
                        PlayerControl.LocalPlayer.myTasks.Remove(mimicText);
                        //System.Console.WriteLine("Unsetting mimic");
                        __instance.MimicCooldown = CustomGameOptions.MimicCooldown;
                        __instance.IsUsingMimic = false;
                        __instance.MimicTarget = null;
                        Utils.Unmorph(__instance.Player);

                        Utils.Rpc(CustomRPC.RpcResetAnim, PlayerControl.LocalPlayer.PlayerId);
                        yield break;
                    }

                    Utils.Morph(__instance.Player, mimicPlayer);
                    __instance.MimicButton.SetCoolDown(CustomGameOptions.MimicDuration - (float)totalMimickTime,
                        CustomGameOptions.MimicDuration);

                    yield return null;
                }
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (!__gInstance.Player.Data.IsImpostor() && (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(8) || ConsoleJoystick.player.GetButtonDown(8)))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
                __instance.KillButton.SetCoolDown(__gInstance.KillTimer(),
                    CustomGameOptions.GlitchKillCooldown);

                __instance.KillButton.SetTarget(null);
                __gInstance.KillTarget = null;

                if (__instance.KillButton.isActiveAndEnabled && __gInstance.Player.moveable)
                {
                    if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    else if (__gInstance.Player.IsLover()) Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
                    else Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    __gInstance.KillTarget = __gInstance.ClosestPlayer;
                }

                __gInstance.KillTarget?.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void KillButtonPress(Glitch __gInstance)
            {
                if (__gInstance.KillTarget != null)
                {
                    var interact = Utils.Interact(__gInstance.Player, __gInstance.KillTarget, true);
                    if (interact[4])
                    {
                        return;
                    }
                    else if (interact[0])
                    {
                        __gInstance.Cooldown = CustomGameOptions.GlitchKillCooldown;
                        return;
                    }
                    else if (interact[1])
                    {
                        __gInstance.Cooldown = CustomGameOptions.TempSaveCdReset;
                        return;
                    }
                    else if (interact[3])
                    {
                        return;
                    }
                    return;
                }
            }
        }

        public static class HackButtonHandler
        {
            public static void HackButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.HackButton == null)
                {
                    __gInstance.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.HackButton.gameObject.SetActive(true);
                    __gInstance.HackButton.graphic.enabled = true;
                    __gInstance.HackText = Object.Instantiate(__instance.KillButton.buttonLabelText, __gInstance.HackButton.transform);
                    __gInstance.HackText.gameObject.SetActive(false);
                    __gInstance.ButtonLabels.Add(__gInstance.HackText);
                }

                __gInstance.HackButton.graphic.sprite = HackSprite;

                __gInstance.HackButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
                if (__instance.UseButton != null)
                {
                    __gInstance.HackButton.transform.position = new Vector3(
                        __instance.UseButton.transform.position.x - 2f,
                        __instance.UseButton.transform.position.y + 1f,
                        __instance.ReportButton.transform.position.z);
                }
                else
                {
                    __gInstance.HackButton.transform.position = new Vector3(
                        __instance.PetButton.transform.position.x - 2f,
                        __instance.PetButton.transform.position.y + 1f,
                        __instance.ReportButton.transform.position.z);
                }
                __gInstance.HackButton.SetCoolDown(__gInstance.HackTimer(),
                    CustomGameOptions.HackCooldown);

                __gInstance.HackButton.graphic.SetCooldownNormalizedUvs();

                __gInstance.HackText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));

                __gInstance.HackText.text = "Hack";
                __gInstance.HackText.SetOutlineColor(Colors.Glitch);

                __gInstance.HackButton.SetTarget(null);
                __gInstance.HackTarget = null;

                if (__gInstance.HackButton.isActiveAndEnabled && __gInstance.Player.moveable)
                {
                    PlayerControl closestPlayer = null;
                    Utils.SetTarget(
                        ref closestPlayer,
                        __gInstance.HackButton
                    );
                    __gInstance.HackTarget = closestPlayer;
                }

                var labelrender = __gInstance.HackText;
                if (__gInstance.HackTarget != null)
                {
                    labelrender.color = Palette.EnabledColor;
                    labelrender.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    labelrender.color = Palette.DisabledClear;
                    labelrender.material.SetFloat("_Desat", 1f);
                }

                if (__gInstance.HackTarget != null)
                {
                    __gInstance.HackTarget.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
                    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU hack") || ConsoleJoystick.player.GetButtonDown(21)) // controller RT
                    {
                        __gInstance.HackButton.DoClick();
                    }
                }
            }

            public static void HackButtonPress(Glitch __gInstance)
            {
                if (__gInstance.HackTarget != null)
                {
                    if (__gInstance.Player.inVent) return;
                    var interact = Utils.Interact(__gInstance.Player, __gInstance.HackTarget);
                    if (interact[4])
                    {
                        __gInstance.RpcSetHacked(__gInstance.HackTarget);
                    }
                    if (interact[0])
                    {
                        __gInstance.HackCooldown = CustomGameOptions.HackCooldown;
                        return;
                    }
                    else if (interact[1])
                    {
                        __gInstance.HackCooldown = CustomGameOptions.TempSaveCdReset;
                        return;
                    }
                    else if (interact[3])
                    {
                        return;
                    }
                    return;
                }
            }
        }

        public static class MimicButtonHandler
        {
            public static void MimicButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.MimicButton == null)
                {
                    __gInstance.MimicButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.MimicButton.gameObject.SetActive(true);
                    __gInstance.MimicButton.graphic.enabled = true;
                }

                __gInstance.MimicButton.graphic.sprite = MimicSprite;

                __gInstance.MimicButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started ||
                    AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay));
                if (__instance.UseButton != null)
                {
                    __gInstance.MimicButton.transform.position = new Vector3(
                        __instance.UseButton.transform.position.x - 1f,
                        __instance.UseButton.transform.position.y + 1f,
                        __instance.ReportButton.transform.position.z);
                }
                else
                {
                    __gInstance.MimicButton.transform.position = new Vector3(
                        __instance.PetButton.transform.position.x - 1f,
                        __instance.PetButton.transform.position.y + 1f,
                        __instance.ReportButton.transform.position.z);
                }

                if (__gInstance.IsUsingMimic)
                {
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MimicButton.graphic.color = Palette.EnabledColor;
                }
                else if (__gInstance.Player.moveable)
                {
                    __gInstance.MimicButton.isCoolingDown = false;
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MimicButton.graphic.color = Palette.EnabledColor;
                    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic") || ConsoleJoystick.player.GetButtonDown(24)) // controller LT
                    {
                        __gInstance.MimicButton.DoClick();
                    }
                }
                else
                {
                    __gInstance.MimicButton.isCoolingDown = true;
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.MimicButton.graphic.color = Palette.DisabledClear;
                }

                if (!__gInstance.IsUsingMimic)
                {
                    __gInstance.MimicButton.SetCoolDown(__gInstance.MimicTimer(),
                        CustomGameOptions.MimicCooldown);
                }
                __gInstance.MimicButton.graphic.SetCooldownNormalizedUvs();
            }

            public static void MimicButtonPress(Glitch __gInstance)
            {
                List<byte> mimicTargets = new List<byte>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != __gInstance.Player && !player.Data.Disconnected)
                    {
                        if (!player.Data.IsDead) mimicTargets.Add(player.PlayerId);
                        else
                        {
                            foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            {
                                if (body.ParentId == player.PlayerId) mimicTargets.Add(player.PlayerId);
                            }
                        }
                    }
                }
                byte[] mimictargetIDs = mimicTargets.ToArray();
                var pk = new PlayerMenu((x) =>
                {
                    __gInstance.RpcSetMimicked(x);
                }, (y) =>
                {
                    return mimictargetIDs.Contains(y.PlayerId);
                });
                Coroutines.Start(pk.Open(0f, true));
            }
        }
    }
}