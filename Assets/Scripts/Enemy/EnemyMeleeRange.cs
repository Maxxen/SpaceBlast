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
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                isInRange = true;
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                isInRange = false;
        }
    }
}
