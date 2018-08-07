using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    /// <summary>
    /// Sets the nav-agents destination to the players current position, but doesnt update it afterwards.
    /// </summary>
    [CreateAssetMenu(menuName = "EnemyAI/Actions/SetDestinationPlayer")]
    class AIActionSetDestinationPlayer : AIAction
    {
        public override void StartAction(AIStateController controller)
        {
            controller.nav.SetDestination(controller.player.transform.position);
        }
        public override void UpdateAction(AIStateController controller)
        {
            
        }
    }
}
