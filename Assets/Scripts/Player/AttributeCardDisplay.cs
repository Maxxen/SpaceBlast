using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class AttributeCardDisplay : MonoBehaviour 
    {
        public GameController gameController;
        public AttributeCard card;
        public Text nameText;
        public Image image;

        public void OnEnable()
        {
            image.sprite = card.image;
            nameText.text = card.description;
        }

        public void LevelUp()
        {
            gameController.LevelUpPlayer(card.attribute);
        }
    }
}
