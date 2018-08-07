using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Enemy
{
    interface IWeapon
    {
        void Attack(UnityEngine.GameObject target);
        bool CanAttack();
    }
}
