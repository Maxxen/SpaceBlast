using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public enum EnemyAttribute
    {
        MaxHealth,
        MovementSpeed,
        Damage
    }

    [CreateAssetMenu(fileName = "EnemyAttributes")]
    public class EnemyAttributes : ScriptableObject
    {
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public int Damage { get; private set; }
        public int ScoreReward { get; private set; }

        [SerializeField]
        int BASE_HEALTH = 100;
        [SerializeField]
        int HEALTH_PER_LVL = 25;

        [SerializeField]
        float BASE_MOVEMENT_SPEED = 3f;
        [SerializeField]
        float BASE_MOVEMENT_SPEED_PER_LVL = 1.5f;

        [SerializeField]
        int BASE_DAMAGE = 10;
        [SerializeField]
        int BASE_DAMAGE_PER_LVL = 1;

        [SerializeField]
        int attribute_Health = 0;
        [SerializeField]
        int attribute_MovementSpeed = 0;
        [SerializeField]
        int attribute_Damage = 0;


        [SerializeField]
        int scoreReward = 100;

        private void OnEnable()
        {
            RecalculateStats();
        }

        public void LevelUpAttribute(EnemyAttribute attribute)
        {
            switch (attribute)
            {
                case EnemyAttribute.MaxHealth:
                    attribute_Health++;
                    break;
                case EnemyAttribute.MovementSpeed:
                    attribute_MovementSpeed++;
                    break;
                case EnemyAttribute.Damage:
                    attribute_Damage++;
                    break;
                default:
                    break;
            }

            RecalculateStats();
        }

        public void RecalculateStats()
        {
            MaxHealth = (BASE_HEALTH + (HEALTH_PER_LVL * attribute_Health));
            MovementSpeed = (BASE_MOVEMENT_SPEED + (BASE_MOVEMENT_SPEED_PER_LVL * attribute_MovementSpeed));
            Damage = (BASE_DAMAGE + (BASE_DAMAGE_PER_LVL * attribute_Damage));

            ScoreReward = scoreReward;
        }

        public void ResetStats()
        {
            attribute_Damage = 0;
            attribute_Health = 0;
            attribute_MovementSpeed = 0;
            RecalculateStats();
        }
    }
}
