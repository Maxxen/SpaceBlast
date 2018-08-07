using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/StopMoving")]
    class AIActionStopMoving : AIAction
    {
        public override void StartAction(AIStateController controller)
        {
            controller.nav.ResetPath();
        }

        public override void UpdateAction(AIStateController controller)
        {

        }
    }
}
