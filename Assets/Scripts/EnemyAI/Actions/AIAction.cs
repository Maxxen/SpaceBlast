using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    abstract class AIAction : ScriptableObject
    {
        public abstract void StartAction(AIStateController controller);
        public abstract void UpdateAction(AIStateController controller);
    }
}
