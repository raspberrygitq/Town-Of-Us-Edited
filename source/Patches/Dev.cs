using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches
{
    public class DevFeatures
    {
        public static bool dev = false;
        public static bool error = false;
        public static bool system = false;
        public static bool noaccess = false;
        public static bool enabled = true;
        public static Dictionary<PlayerControl, string> Players = new Dictionary<PlayerControl, string>();
        public static string localStatus;
        public static string lastStatus;
        public static bool isRandom = false;
        public class SavedOutfit
        {
            public static int ColorId;
            public static string HatId;
            public static string PetId;
            public static string SkinId;
            public static string VisorId;
            public static string PlayerName;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
        public static class UpdateDevClient
        {
            public static void Postfix()
            {
                if (EOSManager.Instance.loginFlowFinished)
                {
                    if (localStatus.IsNullOrWhiteSpace())
                    {
                        if ((EOSManager.Instance.FriendCode == "peaktipple#8186" || EOSManager.Instance.FriendCode == "netpowered#1463"
                        || EOSManager.Instance.FriendCode == "leftwax#7574") && TownOfUs.HideDevStatus.Value)
                        {
                            if (EOSManager.Instance.FriendCode == "peaktipple#8186") lastStatus = "MainDev";
                            else if (EOSManager.Instance.FriendCode == "netpowered#1463"
                            || EOSManager.Instance.FriendCode == "leftwax#7574") lastStatus = "Dev";
                            localStatus = "DevHidden";
                        }
                        else if (EOSManager.Instance.FriendCode == "peaktipple#8186")
                        {
                            localStatus = "MainDev";
                        }
                        else if (EOSManager.Instance.FriendCode == "netpowered#1463"
                        || EOSManager.Instance.FriendCode == "leftwax#7574")
                        {
                            localStatus = "Dev";
                        }
                        return;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public class SpecialChat
        {
            public static bool IsCorrect(string s)
            {
                foreach (char c in s)
                {
                    if (!Char.IsLetterOrDigit(c))
                    return false;
                }
                return true;
            }
            public static string GetRandomName()
            {
                var RandomWordList = new List<string> { "Ivrinet", "Ampide", "Sindek", "Nullarado", "Peaktipple", "Frenv", "Imen", "Mins", "Waitim", "Inosu"
                , "Amoni", "Zitu", "Ivenom", "Nowa", "Unyk", "Uhdes", "Ilmin", "Woh", "Sluh", "Inoj"
                , "Wayui", "Nomeq", "Patser", "Elper", "Emanos", "Amina", "Sha", "Kuni", "Ani", "Nikuma"
                , "Canobi", "Anome", "Nika", "Emster", "Badigo", "Nomeva", "Susami", "Nami", "Mano", "Fista"
                , "Wenitzu", "Maya", "Impro", "Sumen", "Matin", "Alpha", "Omega", "Fin", "Kisto", "Canuo"
                , "Enoma", "Nemeqo", "Istena", "Ameni", "Onema", "Manaq", "Onima", "Numati", "Nometa", "Omeni" };
                var random = new System.Random();
                int index = random.Next(RandomWordList.Count);
                var name = RandomWordList[index];
                RandomWordList.RemoveAt(index);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                return "Azerty";
            }
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer , ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat)
                    return true;

                if (!enabled)
                    return true;
                
                if (chatText.ToLower().StartsWith("/dmsg "))
                {
                    if (sourcePlayer.IsDev())
                    {
                        var message = chatText[6..];
                        dev = true;
                        Commands.SpecialChat.ForceAddChat(sourcePlayer, message);
                        return false;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/checkstatus "))
                {
                    if (sourcePlayer.IsDev() && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        try
                        {
                            var text = Convert.ToByte(chatText[13..]);
                            if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.PlayerId == text) && text != sourcePlayer.PlayerId)
                            {
                                Utils.Rpc(CustomRPC.CheckStatus, sourcePlayer.PlayerId, text);
                                chatText = $"Requesting for {Utils.PlayerById(text).Data.PlayerName}'s status infos...";
                                system = true;
                            }
                            else if (text == sourcePlayer.PlayerId)
                            {
                                DevFeatures.Players.TryGetValue(sourcePlayer, out var status);
                                chatText = $"{sourcePlayer.Data.PlayerName}'s status Infos:\nlocal status: {DevFeatures.localStatus}\nPlayers List status: {status}";
                                system = true;
                            }
                            else
                            {
                                chatText = "Make sure this player id does exist. Type \"/id\" to see all players ids.";
                                error = true; 
                            }
                        }
                        catch (FormatException)
                        {
                            chatText = "Make sure to write the id correctly. Use only numbers and don't use letters.";
                            error = true; 
                        }
                        return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/getcode "))
                {
                    if (sourcePlayer.IsDev() && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        try
                        {
                            var text = Convert.ToByte(chatText[9..]);
                            if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.PlayerId == text) && text != sourcePlayer.PlayerId)
                            {
                                Utils.Rpc(CustomRPC.GetCode, sourcePlayer.PlayerId, text);
                                chatText = $"Requesting for {Utils.PlayerById(text).Data.PlayerName}'s Friend Code infos...";
                                system = true;
                            }
                            else if (text == sourcePlayer.PlayerId)
                            {
                                chatText = $"{sourcePlayer.Data.PlayerName}'s Friend Code Infos:\nFriend Code: {EOSManager.Instance.FriendCode}";
                                system = true;
                            }
                            else
                            {
                                chatText = "Make sure this player id does exist. Type \"/id\" to see all players ids.";
                                error = true; 
                            }
                        }
                        catch (FormatException)
                        {
                            chatText = "Make sure to write the id correctly. Use only numbers and don't use letters.";
                            error = true; 
                        }
                        return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/status"))
                {
                    if (sourcePlayer.IsDev() && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        if (localStatus != "DevHidden")
                        {
                            chatText = "Dev status hidden.";
                            system = true;
                            lastStatus = localStatus;
                            localStatus = "DevHidden";
                            if (Players.ContainsKey(sourcePlayer))
                            {
                                Players.Remove(sourcePlayer);
                            }
                            Players.Add(sourcePlayer, "DevHidden");
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "DevHidden");
                        }
                        else
                        {
                            chatText = "Dev status shown.";
                            system = true;
                            localStatus = lastStatus;
                            if (Players.ContainsKey(sourcePlayer))
                            {
                                Players.Remove(sourcePlayer);
                            }
                            Players.Add(sourcePlayer, localStatus);
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, localStatus);
                        }
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/default"))
                {
                    if (sourcePlayer.IsDev() && LobbyBehaviour.Instance && isRandom)
                    {
                        if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            chatText = "Outfit set back to normal.";
                            system = true;
                            isRandom = false;
                            sourcePlayer.SetName(DataManager.Player.Customization.Name);
			                sourcePlayer.SetColor((int)DataManager.Player.Customization.Color);
                            sourcePlayer.CmdCheckName(DataManager.Player.Customization.Name);
			                sourcePlayer.CmdCheckColor(DataManager.Player.Customization.Color);
			                sourcePlayer.RpcSetPet(DataManager.Player.Customization.Pet);
			                sourcePlayer.RpcSetHat(DataManager.Player.Customization.Hat);
			                sourcePlayer.RpcSetSkin(DataManager.Player.Customization.Skin);
			                if (DestroyableSingleton<HatManager>.Instance.GetHatById(DataManager.Player.Customization.Hat).BlocksVisors)
			                {
				                DataManager.Player.Customization.Visor = "visor_EmptyVisor";
			                }
			                sourcePlayer.RpcSetVisor(DataManager.Player.Customization.Visor);
                        }
                        sourcePlayer.cosmetics.colorBlindText.color = Color.white;
                        if (GameData.Instance.GetHost() == sourcePlayer.Data)
                        {
                            DestroyableSingleton<HostInfoPanel>.Instance.content.SetActive(true);
                        }
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId && !sourcePlayer.IsDev())
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId && !LobbyBehaviour.Instance)
                    {
                        chatText = "You can only use this command in lobby!";
                        error = true;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId && !isRandom)
                    {
                        chatText = "Your outfit is already set to default!";
                        error = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/randomise"))
                {
                    if (sourcePlayer.IsDev() && LobbyBehaviour.Instance)
                    {
                        if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            chatText = "Outfit randomised successfully.";
                            system = true;
                            isRandom = true;
                            sourcePlayer.SetName(GetRandomName());
			                sourcePlayer.SetColor((int)UnityEngine.Random.Range(0, Palette.PlayerColors.Length));
                            sourcePlayer.CmdCheckName(sourcePlayer.GetDefaultOutfit().PlayerName);
			                sourcePlayer.CmdCheckColor((byte)sourcePlayer.GetDefaultOutfit().ColorId);
			                sourcePlayer.RpcSetPet(HatManager.Instance.allPets[UnityEngine.Random.Range(0, HatManager.Instance.allPets.Count)].ProdId);
			                sourcePlayer.RpcSetHat(HatManager.Instance.allHats[UnityEngine.Random.Range(0, HatManager.Instance.allHats.Count)].ProdId);
			                sourcePlayer.RpcSetSkin(HatManager.Instance.allSkins[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId);
                            sourcePlayer.RpcSetVisor(HatManager.Instance.allVisors[UnityEngine.Random.Range(0, HatManager.Instance.allVisors.Count)].ProdId);
                            SavedOutfit.ColorId = sourcePlayer.GetDefaultOutfit().ColorId;
                            SavedOutfit.HatId = sourcePlayer.GetDefaultOutfit().HatId;
                            SavedOutfit.SkinId = sourcePlayer.GetDefaultOutfit().SkinId;
                            SavedOutfit.PetId = sourcePlayer.GetDefaultOutfit().PetId;
                            SavedOutfit.VisorId = sourcePlayer.GetDefaultOutfit().VisorId;
                            SavedOutfit.PlayerName = sourcePlayer.GetDefaultOutfit().PlayerName;
                        }
                        sourcePlayer.cosmetics.colorBlindText.color = Color.white;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId && !sourcePlayer.IsDev())
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId && !LobbyBehaviour.Instance)
                    {
                        chatText = "You can only use this command in lobby!";
                        error = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/fly"))
                {
                    if (sourcePlayer.IsDev() && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        if (LobbyBehaviour.Instance)
                        {
                            chatText = "Colliders successfully updated.";
                            system = true;
                            if (sourcePlayer.Collider.enabled)
                            {
                                sourcePlayer.Collider.enabled = false;
                            }
                            else
                            {
                                sourcePlayer.Collider.enabled = true;
                            }
                        }
                        else if(sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            chatText = "You can't use that command unless you are in lobby!";
                            error = true;
                        }
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                if (chatText.ToLower().StartsWith("/dev"))
                {
                    if (sourcePlayer.IsDev() && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "Current devs & friendcodes:\n\n-le killer (Main Dev): peaktipple#8186\n-whichTwix: netpowered#1463\n-Windyways: leftwax#7574\n-Det: (no code)\n-Gun: (no code)";
                        system = true;
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        chatText = "You don't have access to this command!";
                        noaccess = true;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public class DevChatFixName
        {
            public static bool Prefix(ChatBubble __instance)
            {
                if (dev)
                {
                    __instance.NameText.text = "DEV MESSAGE";
                    __instance.NameText.color = Palette.ImpostorRed;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = false;
			        __instance.Background.color = Palette.White;
                    __instance.votedMark.enabled = false;
                    dev = false;
                    return false;
                }
                else if (error)
                {
                    __instance.NameText.text = "ERROR";
                    __instance.NameText.color = Palette.ImpostorRed;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = true;
			        __instance.Background.color = Palette.White;
                    __instance.votedMark.enabled = false;
                    error = false;
                    return false;
                }
                else if (system)
                {
                    __instance.NameText.text = "SYSTEM MESSAGE";
                    __instance.NameText.color = Palette.CrewmateBlue;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = false;
			        __instance.Background.color = Palette.White;
                    __instance.votedMark.enabled = false;
                    system = false;
                    return false;
                }
                else if (noaccess)
                {
                    __instance.NameText.text = "NO ACCESS";
                    __instance.NameText.color = Palette.Blue;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = true;
			        __instance.Background.color = Palette.White;
                    __instance.votedMark.enabled = false;
                    noaccess = false;
                    return false;
                }
                else return true;
            }
        }
    }
}