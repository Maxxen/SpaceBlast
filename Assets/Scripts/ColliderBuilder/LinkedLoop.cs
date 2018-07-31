using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ColliderBuilder
{
    class LinkedLoop<T> : LinkedList<T>
    {
        public T this[int index]
        {
            get
            {
                //perform the index wrapping
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                //find the proper node
                LinkedListNode<T> node = First;
                for (int i = 0; i < index; i++)
                    node = node.Next;

                return node.Value;
            }
        }

        public LinkedLoop()
        {

        }

        public LinkedLoop(IEnumerable<T> col) :base(col)
        {
        }

        public void IterateTriple(Action<T, T, T> action)
        {
            var node = First;
            while (node != Last)
            {
                action(node.Previous.Value, node.Value, node.Next.Value);
                node = node.Next;
            }
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Equals(item))
                    return i;

            return -1;
        }
    }
}
