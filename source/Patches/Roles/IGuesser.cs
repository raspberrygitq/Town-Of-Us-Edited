using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TownOfUsEdited.Roles
{
    public interface IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; }
    }
}