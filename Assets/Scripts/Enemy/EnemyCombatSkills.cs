using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "EnemySkills")]
    public class EnemyCombatSkills : ScriptableObject
    {
        public int HealthMax { get; private set; }
        public float MovementSpeed { get; private set; }

        public int ShootingDamage { get; private set; }
        public float ShootingSpeed { get; private set; }
        public float ShootingVelocity { get; private set; }

        const int BASE_HEALTH = 100;
        const int HEALTH_PER_LVL = 25;

        const float BASE_HEALTH_REGEN = 1f;
        const float HEALTH_REGEN_PER_LVL = 1.1f;

        const int BASE_ENERGY = 100;
        const int ENERGY_PER_LVL = 25;

        const float BASE_ENERGY_REGEN = 0.2f;
        const float ENERGY_REGEN_PER_LVL = 1.25f;

        const float BASE_MOVEMENT_SPEED = 7f;
        const float MOVEMENT_SPEED_PER_LVL = 1.5f;

        const int BASE_SHOOT_DAMAGE = 10;
        const int SHOOT_DAMAGE_PER_LVL = 10;

        const float BASE_SHOOT_SPEED = 0.8f;
        const float SHOOT_SPEED_PER_LVL = 0.2f;

        const float BASE_BULLET_VELOCITY = 8f;
        const float BULLET_VELOCITY_PER_LVL = 2f;

        int healthSkill = 0;
        int healthRegenSkill = 0;
        int energySkill = 0;
        int energyRegenSkill = 0;
        int movementSpeedSkill = 0;
        int shootingDamageSkill = 0;
        int shootingSpeedSkill = 0;
        int bulletVelocitySkill = 0;


        public void RecalculateStats()
        {
            HealthMax = BASE_HEALTH + (HEALTH_PER_LVL * healthSkill);
            MovementSpeed = Mathf.Log(BASE_MOVEMENT_SPEED, MOVEMENT_SPEED_PER_LVL * (movementSpeedSkill + 1));
            ShootingDamage = BASE_SHOOT_DAMAGE + (SHOOT_DAMAGE_PER_LVL * shootingDamageSkill);
            ShootingSpeed = BASE_SHOOT_SPEED - (SHOOT_SPEED_PER_LVL * shootingSpeedSkill);
            ShootingVelocity = BASE_BULLET_VELOCITY + (BULLET_VELOCITY_PER_LVL * bulletVelocitySkill);
        }
    }
}
