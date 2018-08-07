using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    class EnemyArtilleryWeapon : MonoBehaviour, IWeapon
    {
        public GameObject mortarShell;

        static ObjectPool mortarPool;
        EnemyCombatStats stats;
        Animator anim;

        public void Start()
        {
            stats = GetComponent<EnemyCombatStats>();
            anim = GetComponent<Animator>();
            
            if(mortarPool == null)
            {
                mortarPool = new ObjectPool(
                mortarShell, 
                (b) => { var mortar = b.GetComponent<Mortar>(); mortar.shooterTag = "Enemy"; mortar.damage = stats.ProjectileDamage; },
                15
                );
            }
        }

        public void Attack(GameObject target)
        {
            mortarPool.Spawn(target.transform.position, Quaternion.identity);
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
