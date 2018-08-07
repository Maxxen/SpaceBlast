using Assets.Scripts.EnemyAI.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.States
{

    [CreateAssetMenu (menuName ="EnemyAI/State")]
    class AIState : ScriptableObject
    {
        public AIAction[] actions;
        public Transition[] transitions;

        public Color sceneGizmoColor = Color.gray;
        public string animationTrigger = "";
        public void UpdateState (AIStateController controller)
        {
            DoActions(controller);
            CheckTranstitions(controller);
        }

        private void DoActions(AIStateController controller)
        {
            foreach(AIAction action in actions)
            {
                if(action != null)
                {
                    action.UpdateAction(controller);
                }
            }
        }


        private void CheckTranstitions(AIStateController controller)
        {
            foreach(Transition transition in transitions)
            {
                transition.Decide(controller);
            }
        }
    }
}
