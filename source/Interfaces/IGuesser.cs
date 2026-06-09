using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUsEdited.Interfaces
{
    public interface IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; }
    }
}