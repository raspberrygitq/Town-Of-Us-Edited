using HarmonyLib;
using UnityEngine;

namespace TownOfUsEdited
{
    [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerUpdate
    {
        public static void Postfix(VersionShower __instance)
        {
            var text = __instance.text;
            text.text += " - <color=#00FF00FF><color=#EE9D01>Town Of Us</color><b><color=#AA00FF> Edited</color></b> v" + TownOfUsEdited.VersionString + "</color>" + TownOfUsEdited.VersionTag;
            text.transform.localPosition += new Vector3(-0.8f, -0.08f, 0f);

            if (GameObject.Find("RightPanel"))
            {
                text.transform.SetParent(GameObject.Find("RightPanel").transform);

                var aspect = text.gameObject.AddComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.Top;
                aspect.DistanceFromEdge = new Vector3(-0.2f, 4.5f, 8f);

                aspect.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    aspect.AdjustPosition();
                })));

                return;
            }
        }
    }
}
