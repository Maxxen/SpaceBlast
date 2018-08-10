using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    enum PlayerAttribute
    {
        MaxHealth,
        HealthRegen,
        MaxEnergy,
        EnergyRegen,
        MovementSpeed,
        AttackDamage,
        AttackSpeed,
        BulletVelocity
    }

    [CreateAssetMenu(fileName = "PlayerAttributes")]
    public class PlayerAttributes : ScriptableObject
    {
        public int MaxHealth { get; private set; }
        public float HealthRegen { get; private set; }
        public int MaxEnergy { get; private set; }
        public float EnergyRegen { get; private set; }

        public int AttackDamage { get; private set; }
        public float AttackSpeed { get; private set; }
        public float BulletVelocity { get; private set; }

        public float MovementSpeed { get; private set; }

        public float AttackEnergyCost { get; private set; }

        [SerializeField]
        int BASE_HEALTH = 100;
        [SerializeField]
        int HEALTH_PER_LVL = 25;

        [SerializeField]
        float BASE_HEALTH_REGEN = 1f;
        [SerializeField]
        float HEALTH_REGEN_PER_LVL = 1.1f;

        [SerializeField]
        int BASE_ENERGY = 100;
        [SerializeField]
        int ENERGY_PER_LVL = 25;

        [SerializeField]
        float BASE_ENERGY_REGEN = 0.2f;
        [SerializeField]
        float ENERGY_REGEN_PER_LVL = 1.25f;

        [SerializeField]
        float BASE_MOVEMENT_SPEED = 7f;
        [SerializeField]
        float MOVEMENT_SPEED_PER_LVL = 1.5f;

        //Shoot energy cost depends on damage and shoot speed.
        [SerializeField]
        float BASE_SHOOT_COST = 10f;
        
        [SerializeField]
        int BASE_SHOOT_DAMAGE = 10;
        [SerializeField]
        int SHOOT_DAMAGE_PER_LVL = 10;

        [SerializeField]
        float BASE_SHOOT_SPEED = 1f;
        [SerializeField]
        float SHOOT_SPEED_PER_LVL = 1.1f;

        [SerializeField]
        float BASE_BULLET_VELOCITY = 8f;
        [SerializeField]
        float BULLET_VELOCITY_PER_LVL = 2f;

        int attribute_health = 0;
        int attribute_healthRegen = 0;
        int attribute_energy = 0;
        int attribute_energyRegen = 0;
        int attribute_movementSpeed = 0;
        int attribute_attackDamage = 0;
        int attribute_attackSpeed = 0;
        int attribute_bulletVelocity = 0;

        public void RecalculateStats()
        {
            MaxHealth = BASE_HEALTH + (HEALTH_PER_LVL * attribute_health);
            HealthRegen = BASE_HEALTH_REGEN * (HEALTH_REGEN_PER_LVL * attribute_healthRegen);
            MaxEnergy = BASE_ENERGY + (ENERGY_PER_LVL * attribute_energy);
            EnergyRegen = BASE_ENERGY_REGEN + (ENERGY_PER_LVL * attribute_energyRegen);
            MovementSpeed = Mathf.Log(BASE_MOVEMENT_SPEED, MOVEMENT_SPEED_PER_LVL * (attribute_movementSpeed + 1));
            AttackDamage = BASE_SHOOT_DAMAGE + (SHOOT_DAMAGE_PER_LVL * attribute_attackDamage);
            AttackSpeed = BASE_SHOOT_SPEED * (Mathf.Pow(SHOOT_SPEED_PER_LVL, attribute_attackSpeed));
            BulletVelocity = BASE_BULLET_VELOCITY + (BULLET_VELOCITY_PER_LVL * attribute_attackSpeed);

            AttackEnergyCost = (BASE_SHOOT_COST / (BASE_SHOOT_DAMAGE + BASE_SHOOT_SPEED + BASE_BULLET_VELOCITY)) * (AttackDamage + AttackSpeed + BulletVelocity);
        }

        public void ResetStats()
        {
            attribute_health = 0;
            attribute_healthRegen = 0;
            attribute_energy = 0;
            attribute_energyRegen = 0;
            attribute_movementSpeed = 0;
            attribute_attackDamage = 0;
            attribute_attackSpeed = 0;
            attribute_bulletVelocity = 0;

            RecalculateStats();
        }
    }
}
