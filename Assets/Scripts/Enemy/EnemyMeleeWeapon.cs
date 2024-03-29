﻿using Assets.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    class EnemyMeleeWeapon : MonoBehaviour, IWeapon
    {
        public EnemyMeleeRange range;

        EnemyStats health;
        Animator anim;

        private void Start()
        {
            health = GetComponent<EnemyStats>();
            anim = GetComponent<Animator>();
        }

        //This function is called as an animation event by a "Attack" animation clip.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void Attack()
        {
            if (range.IsInRange)
            {
                range.targer.GetComponent<IDamageable>().TakeDamage(health.attributes.Damage);
            }
        }
    }
}
