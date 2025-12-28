using HarmonyLib;
using System.Linq;

namespace TownOfUsEdited.Patches
{
    public class PatchChat
    {
        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class FixEmojis
        {
            public static void Postfix(ChatBubble __instance, ref string chatText)
            {
                if (asset == null) asset = AssetLoader.LoadSpriteAsset(AssetLoader.bundles.FirstOrDefault(x => x.Key == "touebundle").Value, "hehEmojiAsset.asset");
                __instance.TextArea.spriteAsset = asset;
                __instance.TextArea.text = EmojiFormat(chatText);
            }
        }

        public static TMPro.TMP_SpriteAsset asset;
        public static string EmojiFormat(string text)
        {
            string finalText = text;
            if (finalText.ToLower().Contains(":heh:")) finalText = text.Replace(":heh:", "<sprite=0>");
            return finalText;
        }
    }
}