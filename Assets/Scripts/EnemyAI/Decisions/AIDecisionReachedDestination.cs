using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    [CreateAssetMenu(menuName = "EnemyAI/Decisions/StandingStill")]
    class AIDecisionReachedDestination : AIDecision
    {
        public override bool Decide(AIStateController controller)
        {
            return
               !controller.nav.pathPending &&
               controller.nav.remainingDistance <= controller.nav.stoppingDistance &&
               (!controller.nav.hasPath || controller.nav.velocity.sqrMagnitude == 0f);

        }
    }
}
