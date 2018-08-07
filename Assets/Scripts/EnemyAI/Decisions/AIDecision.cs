using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Decisions
{
    abstract class AIDecision : ScriptableObject
    {
        public abstract bool Decide(AIStateController controller);
    }
}
