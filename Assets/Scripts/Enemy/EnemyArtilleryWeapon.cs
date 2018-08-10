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
        EnemyHealth health;
        GameObject target;

        public void Start()
        {
            health = GetComponent<EnemyHealth>();
            target = GetComponent<EnemyAI.AIStateController>().player;
            
            if(mortarPool == null)
            {
                mortarPool = new ObjectPool(
                mortarShell, 
                (b) => { var mortar = b.GetComponent<Mortar>(); mortar.fuseTime = 4; mortar.diameter = 2; mortar.damage = health.stats.Damage; },
                10
                );
            }
        }

        //This function is called as an animation event by a "Attack" animation clip.
        //It is absolutely ridiculous but alas the "Unity way" to do it.
        public void Attack()
        {
            mortarPool.Spawn(target.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        }
    }
}
