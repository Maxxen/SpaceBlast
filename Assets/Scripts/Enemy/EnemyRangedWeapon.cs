using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    class EnemyRangedWeapon : MonoBehaviour, IWeapon
    {
        public GameObject bulletPrefab;
        public Transform shootPosition;
        

        static ObjectPool bulletPool;
        EnemyCombatStats stats;
        Animator anim;

        void Start()
        {
            stats = GetComponent<EnemyCombatStats>();
            anim = GetComponent<Animator>();

            if(bulletPool == null)
            {
                bulletPool = new ObjectPool(
                bulletPrefab, 
                (b) => { var bullet = b.GetComponent<Bullet>(); bullet.shooterTag = "Enemy"; bullet.damage = stats.ProjectileDamage; },
                15
                );
            }
        }

        public void Attack(GameObject target)
        {
            anim.SetTrigger("Shoot");
        }

        //This function is called by the animation event in the shoot clip, started by setting the "Shoot" trigger above.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void ShootBullet()
        {
            var bullet = bulletPool.Spawn(shootPosition.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * stats.ProjectileSpeed;
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
