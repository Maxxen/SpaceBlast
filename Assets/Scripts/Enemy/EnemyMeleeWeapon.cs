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

        EnemyCombatStats stats;
        Animator anim;

        private void Start()
        {
            stats = GetComponent<EnemyCombatStats>();
            anim = GetComponent<Animator>();
        }

        public void Attack(GameObject target)
        {
            anim.SetTrigger("Attack");
        }

        //This function is called by the animation event in the shoot clip, started by setting the "Shoot" trigger above.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void DealDamage()
        {
            if (range.IsInRange)
            {
                Debug.Log("Damage");
            }
        }

        float nextAttack;
        public bool CanAttack()
        {
            if(Time.time > nextAttack)
            {
                nextAttack = Time.time + stats.AttackSpeed;
                return true;
            }
            return false;            
        }
    }
}
