using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.EnemyAI.Decisions;
using Assets.Scripts.EnemyAI.States;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scripts.EnemyAI
{
    [System.Serializable]
    enum TransitionCondition
    {
        AllTrue,
        AnyTrue,
    }

    [System.Serializable]
    enum DecisionResult
    {
        IsTrue,
        IsFalse
    }
    static class DecisionResultExtensions
    {
        public static bool ToBoolean(this DecisionResult value)
        {
            return value == DecisionResult.IsTrue;
        }
    }

    [System.Serializable]
    struct AIDecisionQuery
    {
        public AIDecision decision;
        public DecisionResult result;
    }

    [System.Serializable]
    class Transition
    {
        public TransitionCondition condition = TransitionCondition.AnyTrue;
        public AIDecisionQuery[] decisions;
        public AIState nextState;

        public void Decide(AIStateController controller)
        {
            var switchState = false;
            if(condition == TransitionCondition.AllTrue)
            {
                switchState = decisions.All((d) => d.result.ToBoolean() == d.decision.Decide(controller));
            }
            else if(condition == TransitionCondition.AnyTrue)
            {
                switchState = decisions.Any((d) => d.result.ToBoolean() == d.decision.Decide(controller));
            }

            if (switchState)
            {
                controller.TransitionToState(nextState);
            }           
        }
    }
}
