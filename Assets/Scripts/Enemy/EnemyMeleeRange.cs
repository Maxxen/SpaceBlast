using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    class EnemyMeleeRange : MonoBehaviour
    {
        public bool IsInRange { get { return isInRange; } }
        bool isInRange = false;
        public GameObject targer;
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isInRange = true;
                targer = other.gameObject;
            
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isInRange = false;
                targer = null;
            }
        }
    }
}
