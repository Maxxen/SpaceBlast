using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/FollowPlayer")]
    class AIActionChase : AIAction
    {
        public override void StartAction(AIStateController controller)
        {
            
        }

        public override void UpdateAction(AIStateController controller)
        {
            controller.nav.SetDestination(controller.player.transform.position);
        }
    }
}
