using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    [CreateAssetMenu(menuName = "EnemyAI/Decisions/Look")]
    class AIDecisionLook : AIDecision
    {
        public int lookRange = 10;
        public override bool Decide(AIStateController controller)
        {
            return Look(controller);
        }

        private bool Look(AIStateController controller)
        {
            RaycastHit hit;
            if (Physics.Raycast(controller.transform.position + Vector3.up, controller.player.transform.position - controller.transform.position, out hit, lookRange, ~(1<<8)))
            {
                if (hit.collider.gameObject == controller.player)
                    return true;
            }
            return false;
        }
    }
}
