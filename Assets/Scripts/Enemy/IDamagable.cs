using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Enemy
{
    interface IDamageable
    {
        void TakeDamage(int damage);
    }
}
