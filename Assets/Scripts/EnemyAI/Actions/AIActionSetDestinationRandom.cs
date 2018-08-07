using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/SetDestinationRandom")]
    class AIActionSetDestinationRandom : AIAction
    {
        public int maxDistance = 10;
        public float retryTime = 5;
        System.Random rand = new System.Random();

        public override void StartAction(AIStateController controller)
        {
            CalculateNewPath(controller);
        }

        public override void UpdateAction(AIStateController controller)
        {
            // if we get havent reached our goal by this time, Reset
            if (!ReachedDestination(controller) && controller.TimeSinceStateSwitch() > retryTime)
            {
                controller.TransitionToState(controller.currentState);
            }

            //if (!controller.nav.pathPending)
            //{
            //     if (controller.nav.remainingDistance > controller.nav.stoppingDistance)
            //     {
            //         if ( controller.nav.velocity.sqrMagnitude <= 0.5f)
            //         {
            //            CalculateNewPath(controller);
            //         }
            //     }
            //}
        }

        private void CalculateNewPath(AIStateController controller)
        {
            var result = Vector3.zero;
            bool found = false;
            while (!found)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(RandomPosition(controller), out hit, 1.0f, NavMesh.AllAreas))
                {

                    result = hit.position;
                    found = true;
                }
            }
            controller.nav.SetDestination(RandomPosition(controller));
        }
        private Vector3 RandomPosition(AIStateController controller)
        {
            var circlePoint = UnityEngine.Random.insideUnitCircle * (rand.Next(1, maxDistance));
            return controller.transform.position + new Vector3(circlePoint.x, 0, circlePoint.y);
        }

        private bool ReachedDestination(AIStateController controller)
        {
            return
               !controller.nav.pathPending &&
               controller.nav.remainingDistance <= controller.nav.stoppingDistance &&
               (!controller.nav.hasPath || controller.nav.velocity.sqrMagnitude == 0f);
        }
    }
}
