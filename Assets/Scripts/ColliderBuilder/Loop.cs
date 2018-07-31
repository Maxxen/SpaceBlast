using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ColliderBuilder
{
    class Loop<T> : List<T>
    {
        public new T this[int index]
        {
            get
            {
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                return base[index];
            }
            set
            {
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                base[index] = value;
            }
        }

        public new void RemoveAt(int index)
        {
            Remove(this[index]);
        }
    }
}
