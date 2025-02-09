using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches
{
    public class DevStatus
    {
        public static bool enabled = true;
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class Title
        {
            public static void Postfix()
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (LobbyBehaviour.Instance && DevFeatures.Players.ContainsKey(player) && enabled)
                    {
                        DevFeatures.Players.TryGetValue(player, out var customId);
                        if (customId == "MainDev" && !player.nameText().text.Contains("Dev"))
                        {
                            player.nameText().text = player.nameText().text + "<size=2>\n(Main Dev)</size>";
                            player.nameText().color = Palette.Orange;
                        }
                        else if (customId == "Dev" && !player.nameText().text.Contains("Dev"))
                        {
                            player.nameText().text = player.nameText().text + "<size=2>\n(Dev)</size>";
                            player.nameText().color = Palette.ImpostorRed;
                        }
                        else if (customId == "Tester" && !player.nameText().text.Contains("Tester"))
                        {
                            player.nameText().text = player.nameText().text + "<size=2>\n(Tester)</size>";
                            player.nameText().color = Color.grey;
                        }
                        else if (customId == "Vip" && !player.nameText().text.Contains("VIP"))
                        {
                            player.nameText().text = player.nameText().text + "<size=2>\n(VIP)</size>";
                            player.nameText().color = Color.yellow;
                        }
                        else if ((customId == "None" || customId == "DevHidden" || customId == "TesterHidden" || customId == "VipHidden") && (player.nameText().text.Contains("Dev") || player.nameText().text.Contains("Tester") || player.nameText().text.Contains("VIP")))
                        {
                            player.nameText().text = player.Data.PlayerName;
                            player.nameText().color = Palette.White;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        public class OnPlayerJoined
        {
            public static void Prefix()
            {
                if (LobbyBehaviour.Instance && PlayerControl.LocalPlayer && enabled)
                {
                    if (AmongUsClient.Instance.AmHost && RpcHandling.Upped.Any())
                    {
                        var uppedPlayers = RpcHandling.Upped.Keys.ToArray().ToList();
                        foreach (var player in uppedPlayers)
                        {
                            if (RpcHandling.Upped.TryGetValue(player, out var role))
                            {
                                Utils.Rpc(CustomRPC.AddUp, player.PlayerId, role);
                            }
                        }
                    }
                    if (PlayerControl.LocalPlayer.IsDev())
                    {
                        if (DevFeatures.localStatus == "MainDev")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "MainDev");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "MainDev");
                        }
                        else if (DevFeatures.localStatus == "DevHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "DevHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "DevHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Dev");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Dev");
                            }
                        }
                    }
                    else if (PlayerControl.LocalPlayer.IsTester())
                    {
                        if (DevFeatures.localStatus == "TesterHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "TesterHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "TesterHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Tester");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Tester");
                            }
                        }
                    }
                    else if (PlayerControl.LocalPlayer.IsVip())
                    {
                        if (DevFeatures.localStatus == "VipHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "VipHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "VipHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Vip");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Vip");
                            }
                        }
                    }
                    else
                    {
                        Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "None");
                        if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                        {
                            DevFeatures.Players.Add(PlayerControl.LocalPlayer, "None");
                        }
                    }
                }
                return;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public class FixRpcSend
        {
            public static void Postfix()
            {
                if (PlayerControl.LocalPlayer && enabled)
                {
                    if (PlayerControl.LocalPlayer.IsDev())
                    {
                        if (DevFeatures.localStatus == "MainDev")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "MainDev");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "MainDev");
                        }
                        else if (DevFeatures.localStatus == "DevHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "DevHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "DevHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Dev");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Dev");
                            }
                        }
                    }
                    else if (PlayerControl.LocalPlayer.IsTester())
                    {
                        if (DevFeatures.localStatus == "TesterHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "TesterHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "TesterHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Tester");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Tester");
                            }
                        }
                    }
                    else if (PlayerControl.LocalPlayer.IsVip())
                    {
                        if (DevFeatures.localStatus == "VipHidden")
                        {
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "VipHidden");
                            }
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "VipHidden");
                        }
                        else
                        {
                            Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "Vip");
                            if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                            {
                                DevFeatures.Players.Add(PlayerControl.LocalPlayer, "Vip");
                            }
                        }
                    }
                    else
                    {
                        Utils.Rpc(CustomRPC.AddDev, PlayerControl.LocalPlayer.PlayerId, "None");
                        if (!DevFeatures.Players.ContainsKey(PlayerControl.LocalPlayer))
                        {
                            DevFeatures.Players.Add(PlayerControl.LocalPlayer, "None");
                        }
                    }
                }
                return;
            }
        }
    }
}