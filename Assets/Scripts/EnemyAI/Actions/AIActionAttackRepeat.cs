using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EnemyAI.Actions
{
    [CreateAssetMenu(menuName = "EnemyAI/Actions/AttackRepeat")]
    class AIActionAttackRepeat : AIAction
    {
        public override void StartAction(AIStateController controller)
        {
            
        }

        public override void UpdateAction(AIStateController controller)
        {
            if(controller.attack.CanAttack())
                controller.attack.Attack(controller.player);
        }
    }
}
