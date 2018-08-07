using Assets.Scripts.Weapon;
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

        private GameObject target;

        public void Start()
        {
            stats = GetComponent<EnemyCombatStats>();
            anim = GetComponent<Animator>();
            
            if(mortarPool == null)
            {
                mortarPool = new ObjectPool(
                mortarShell, 
                (b) => { var mortar = b.GetComponent<Mortar>(); mortar.fuseTime = 4; mortar.diameter = 2; mortar.damage = stats.ProjectileDamage; },
                10
                );
            }
        }

        public void Attack(GameObject target)
        {
            this.target = target;
            anim.SetTrigger("Shoot");
        }

        //This function is called by the animation event in the shoot clip, started by setting the "Shoot" trigger above.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void FireMortar()
        {
            mortarPool.Spawn(target.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
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
