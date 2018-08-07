using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    [CreateAssetMenu(menuName = "EnemyAI/Decisions/TimePassed")]
    class AIDecisionTimePassed : AIDecision
    {
        public float time;
        public override bool Decide(AIStateController controller)
        {
            return time <= controller.TimeSinceStateSwitch();
        }
    }
}
