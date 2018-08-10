using Assets.Scripts.Weapon;
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
        EnemyHealth health;

        void Start()
        {
            health = GetComponent<EnemyHealth>();

            if(bulletPool == null)
            {
                bulletPool = new ObjectPool(
                bulletPrefab, 
                (b) => { var bullet = b.GetComponent<Bullet>(); bullet.shooterTag = "Enemy"; bullet.damage = health.stats.Damage; },
                15
                );
            }
        }

        //This function is called as an animation event by a "Attack" animation clip.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void Attack()
        {
            var bullet = bulletPool.Spawn(shootPosition.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 8;
        }
    }
}
