﻿using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsEdited.NeutralRoles.SoulCollectorMod
{
    public class Soul : MonoBehaviour
    {
        public PlayerControl DeadPlayer = null;
    }

    [HarmonyPatch]
    public static class SoulExtensions
    {
        public static void ClearSouls(this List<GameObject> obj)
        {
            foreach (GameObject t in obj)
            {
                UnityEngine.Object.Destroy(t);
            }
            obj.Clear();
        }

        public static GameObject CreateSoul(this Vector3 location, PlayerControl victim)
        {
            GameObject soul = new GameObject("Soul");
            soul.transform.position = location;
            soul.layer = LayerMask.NameToLayer("Players");
            SpriteRenderer render = soul.AddComponent<SpriteRenderer>();
            render.sprite = TownOfUsEdited.SoulSprite;
            Vector3 scale = render.transform.localScale;
            scale.x *= 0.5f;
            scale.y *= 0.5f;
            render.transform.localScale = scale;
            BoxCollider2D splatCollider = soul.AddComponent<BoxCollider2D>();
            splatCollider.size = new Vector2(render.size.x, render.size.y);
            var scene = soul.AddComponent<Soul>();
            scene.DeadPlayer = victim;
            return soul;
        }
    }
}
