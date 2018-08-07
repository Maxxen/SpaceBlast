using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/LookAtPlayer")]
    class AIActionLookAtPlayer : AIAction
    {
        public float turnSpeed = 0.1f;
        public override void StartAction(AIStateController controller)
        {

        }

        public override void UpdateAction(AIStateController controller)
        {
            controller.transform.rotation = 
                Quaternion.Slerp(
                    controller.transform.rotation, 
                    Quaternion.LookRotation(controller.player.transform.position - controller.transform.position), 
                    0.1f
                );
        }
    }
}
