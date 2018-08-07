using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    [CreateAssetMenu(menuName = "EnemyAI/Decisions/LookingAtPlayer")]
    class AIDecisionLookingAtPlayer : AIDecision
    {
        public float range = 10;
        public float rayRadius = 0.5f;

        public override bool Decide(AIStateController controller)
        {
            RaycastHit hit;
            if (Physics.SphereCast(controller.transform.position + Vector3.up, rayRadius, controller.transform.forward, out hit, range, ~(1<<8)))
            {
                if (hit.collider.CompareTag("Player"))
                    return true;
            }
            return false;
        }
    }
}
