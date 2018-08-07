using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/AttackOnce")]
    class AIActionAttackOnce : AIAction
    {
        public override void StartAction(AIStateController controller)
        {
            if (controller.attack.CanAttack())
            {
                controller.attack.Attack(controller.player);
            }
        }

        public override void UpdateAction(AIStateController controller)
        {
            
        }
    }
}
