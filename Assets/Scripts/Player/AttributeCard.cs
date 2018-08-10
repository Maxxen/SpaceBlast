using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerAttributeCard")]
    public class AttributeCard : ScriptableObject
    {
        public PlayerAttribute attribute;
        public Sprite image;
        public string description;
    }
}
