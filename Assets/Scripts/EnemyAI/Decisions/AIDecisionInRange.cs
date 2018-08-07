using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    [CreateAssetMenu(menuName = "EnemyAI/Decisions/InRange")]
    class AIDecisionInRange : AIDecision
    {
        public int range = 10;
        public override bool Decide(AIStateController controller)
        {
            return ((controller.transform.position - controller.player.transform.position).sqrMagnitude < (range * range));
        }
    }
}
