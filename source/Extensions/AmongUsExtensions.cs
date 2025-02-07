using System.Collections.Generic;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using System;
using TownOfUs.Patches;
using System.Linq.Expressions;
using Il2CppInterop.Runtime.InteropTypes;

namespace TownOfUs.Extensions
{
    public static class AmongUsExtensions
    {
        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (var keyValuePair in self)
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }

            return result;
        }

        public static KeyValuePair<byte, int> MaxPair(this byte[] self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            for (byte i = 0; i < self.Length; i++)
                if (self[i] > result.Value)
                {
                    result = new KeyValuePair<byte, int>(i, self[i]);
                    tie = false;
                }
                else if (self[i] == result.Value)
                {
                    tie = true;
                }

            return result;
        }

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player)
        {
            return new VisualAppearance();
        }

        public static bool TryGetAppearance(this PlayerControl player, IVisualAlteration modifier, out VisualAppearance appearance)
        {
            if (modifier != null)
                return modifier.TryGetModifiedAppearance(out appearance);

            appearance = player.GetDefaultAppearance();
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player)
        {
            if (player.TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (player.TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return player.GetDefaultAppearance();
        }
        public static bool IsImpostor(this NetworkedPlayerInfo playerinfo)
        {
            var player = Utils.PlayerByData(playerinfo);
            var playerRole = Role.GetRole(player);
            if (playerRole == null) return false;

            return playerRole.Faction == Faction.Impostors;
        }

        public static bool coolingDown(this PlayerControl player)
        {
            var role = Role.GetRole(player);
            if (role == null) return true;

            return role.KillCooldown > 0f;
        }

        public static bool IsDev(this PlayerControl player)
        {
            if (player == PlayerControl.LocalPlayer) return DevFeatures.localStatus == "Dev" || DevFeatures.localStatus == "MainDev" || DevFeatures.localStatus == "DevHidden";
            else if (DevFeatures.Players.ContainsKey(player) && DevFeatures.Players.TryGetValue(player, out var customId)) return customId == "Dev" || customId == "MainDev" || customId == "DevHidden";
            return false;
        }

        public static bool IsTester(this PlayerControl player)
        {
            if (player == PlayerControl.LocalPlayer) return DevFeatures.localStatus == "Tester" || DevFeatures.localStatus == "TesterHidden";
            else if (DevFeatures.Players.ContainsKey(player) && DevFeatures.Players.TryGetValue(player, out var customId)) return customId == "Tester" || customId == "TesterHidden";
            return false;
        }

        public static bool IsVip(this PlayerControl player)
        {
            if (player == PlayerControl.LocalPlayer) return DevFeatures.localStatus == "Vip" || DevFeatures.localStatus == "VipHidden";
            else if (DevFeatures.Players.ContainsKey(player) && DevFeatures.Players.TryGetValue(player, out var customId)) return customId == "Vip" || customId == "VipHidden";
            return false;
        }

        public static NetworkedPlayerInfo.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl)
        {
            return playerControl.Data.DefaultOutfit;
        }

        public static NetworkedPlayerInfo.PlayerOutfit GetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            return playerControl.Data.Outfits[(PlayerOutfitType)CustomOutfitType];
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType, NetworkedPlayerInfo.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }
        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            var outfitType = (PlayerOutfitType)CustomOutfitType;
            if (!playerControl.Data.Outfits.ContainsKey(outfitType))
            {
                return;
            }
            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            if (!playerControl.Data.IsDead) playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            if (!playerControl.Data.IsDead) playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            playerControl.cosmetics.colorBlindText.color = Color.white;
            if (PlayerControl.LocalPlayer.Data.IsImpostor() && playerControl.Data.IsImpostor()) playerControl.nameText().color = Patches.Colors.Impostor;
        }


        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl)
        {
            return (CustomPlayerOutfitType)playerControl.CurrentOutfitType;
        }

        public static bool IsNullOrDestroyed(this System.Object obj)
        {

            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

            return false;
        }
        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
        }

        private static class CastExtension<T> where T : Il2CppObjectBase
        {
            public static Func<IntPtr, T> Cast;
            static CastExtension()
            {
                var constructor = typeof(T).GetConstructor(new[] { typeof(IntPtr) });
                var ptr = Expression.Parameter(typeof(IntPtr));
                var create = Expression.New(constructor!, ptr);
                var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
                Cast = lambda.Compile();
            }
        }

        public static T Caster<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
        {
            if (obj is T casted) return casted;
            return obj.Pointer.Caster<T>();
        }

        public static T Caster<T>(this IntPtr ptr) where T : Il2CppObjectBase
        {
            return CastExtension<T>.Cast(ptr);
        }

        public static TMPro.TextMeshPro nameText(this PlayerControl p) => p?.cosmetics?.nameText;

        public static TMPro.TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static UnityEngine.SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;
    }
}