using Assets.Scripts.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    class Mortar : MonoBehaviour
    {
        public float fuseTime = 5f;
        public float diameter = 2f;
        public int damage = 0;

        private float detonationTime;
        private float growSpeed;
        private float rotateSpeed;

        private void Start()
        {
            growSpeed = diameter / fuseTime;
            rotateSpeed = 360 / fuseTime;
        }

        private void OnEnable()
        {
            detonationTime = Time.time + fuseTime;
            this.transform.localScale = Vector3.one;
            this.transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            if(Time.time <= detonationTime)
            {
                this.transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
                this.transform.localScale += Vector3.one * Time.deltaTime * growSpeed;
            }
            else
            {
                //TODO, insert effect or particles here

                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, diameter / 2);
                foreach(Collider c in hitColliders)
                {
                    if (c.CompareTag("Player"))
                    {
                        c.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                    }
                }

                gameObject.SetActive(false);
            }
        }
    }
}
