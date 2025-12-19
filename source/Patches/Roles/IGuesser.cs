using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Roles
{
    public interface IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; }
    }
}