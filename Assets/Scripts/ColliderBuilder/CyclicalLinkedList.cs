using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ColliderBuilder
{
    //Inspired by https://navaneethkn.wordpress.com/2009/08/18/circular-linked-list/#more-197
    class CyclicalNode<T>
    {
        public T Value { get; set; }
        public CyclicalNode<T> Next { get; set; }
        public CyclicalNode<T> Prev { get; set; }

        public CyclicalNode(T item)
        {
            Value = item;
        }
    }
    class CyclicalLinkedList <T> : IEnumerable<T>
    {
        public int Count { get; private set; }
        public CyclicalNode<T> First { get; private set; }
        public CyclicalNode<T> Last { get; private set; }

        public CyclicalLinkedList(ICollection<T> collection)
        {
            foreach(T elem in collection)
            {
                this.AddLast(elem);
            }
        }

        public void AddLast(T item)
        {
            this.Count++;
            if(First == null)
            {
                First = new CyclicalNode<T>(item);
                Last = First;
                First.Next = Last;
                Last.Prev = First;
            }
            else
            {
                var node = new CyclicalNode<T>(item);
                Last.Next = node;
                node.Prev = Last;
                node.Next = First;

                Last = node;
                First.Prev = Last;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            CyclicalNode<T> current = First;
            do
            {
                yield return current.Value;
                current = current.Next;
            }
            while (current != First);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ForEachTriple( Action<T, T, T> action)
        {
            CyclicalNode<T> current = First;
            do
            {
                action(current.Prev.Value, current.Value, current.Next.Value);
                current = current.Next;
            }
            while (current != First);
        }

        public int IndexOf(T item)
        {
            int index = 0;
            CyclicalNode<T> current = First;
            do
            {
                if (current.Value.Equals(item))
                {
                    return index;
                }
                else
                {
                    current = current.Next;
                    index++;
                }
            }
            while (current != First);
            return -1;
        }
    }
}
