using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ColliderBuilder
{
    struct Edge : IComparable<Edge>
    {
        public Vector2 v1, v2;
        public Edge(Vector2 v1, Vector2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public int CompareTo(Edge other) 
        {
            if (this.v1 == other.v2)
                return 1;
            if (this.v2 == other.v1)
                return -1;

            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
            {
                return false;
            }

            var edge = (Edge)obj;
            return v1.Equals(edge.v1) &&
                   v2.Equals(edge.v2);
        }

        public override int GetHashCode()
        {
            var hashCode = 1763187145;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(v1);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(v2);
            return hashCode;
        }

        public static bool operator == (Edge e1, Edge e2)
        {
            return (e1.v1 == e2.v1) && (e1.v2 == e2.v2);
        }

        public static bool operator != (Edge e1, Edge e2)
        {
            return!(e1 == e2);
        }
    }
}
