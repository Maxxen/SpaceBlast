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

        public CyclicalLinkedList()
        {

        }

        public void Remove(T item)
        {
            var node = Find(item);
            if(node != null)
            {
                node.Prev.Next = node.Next;
                node.Next.Prev = node.Prev;

                if (node == First)
                    First = node.Next;
                else if (node == Last)
                    Last = Last.Prev;

                this.Count--;
            }
            //Else throw exception?
        }

        public void RemoveFirst()
        {
            First = First.Next;
            First.Prev = Last;
            Last.Next = First;
            this.Count--;
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

        public void AddFirst(T item)
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
                First.Prev = node;
                node.Prev = Last;
                node.Next = First;
                Last.Next = node;
                First = node;
            }
        }

        public void RotateLeft()
        {
            First = First.Next;
            Last = Last.Next;
        }

        public void Concat(CyclicalLinkedList<T> other)
        {
            Last.Next = other.First;
            other.First.Prev = Last;

            First.Prev = other.Last;
            other.Last.Next = First;

            Count += other.Count;
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

        public void ForEachTriplet( Action<T, T, T> action)
        {
            CyclicalNode<T> current = First;
            do
            {
                action(current.Prev.Value, current.Value, current.Next.Value);
                current = current.Next;
            }
            while (current != First);
        }

        public void ForEachPair( Action<T, T> action)
        {
            CyclicalNode<T> current = First;
            do
            {
                action(current.Value, current.Next.Value);
                current = current.Next;
            }
            while (current != First);
        }

        public void For( Action<T, int> action)
        {
            int index = 0;
            CyclicalNode<T> current = First;
            do
            {
                action(current.Value, index);
                current = current.Next;
                index++;
            }
            while (current != First);
        }

        public CyclicalNode<T> Find(T item)
        {
            CyclicalNode<T> current = First;

            do
            {
                if (current.Value.Equals(item))
                    return current;
                else
                    current = current.Next;
            }
            while (current != First);
            return null;
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
