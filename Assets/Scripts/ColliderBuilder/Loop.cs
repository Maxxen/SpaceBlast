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
                //perform the index wrapping
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                return base[index];
            }
            set
            {
                //perform the index wrapping
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                base[index] = value;
            }
        }


        public static bool IsIntersecting(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector2 r = b - a;
            Vector2 s = d - c;

            float dist = (r.x * s.y) - (r.y * s.x);
            float u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / dist;
            float t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / dist;

            return (0 < u && u < 1 && 0 < t && t < 1);
        }

        public static bool RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
            //Investigate max distance further
            intersectionPoint = Vector2.zero;
            return GetLineIntersection(rayOrigin, rayOrigin + (rayDirection * (Math.Max(c.x - rayOrigin.x, d.x - rayOrigin.x)) * 2), c, d, out intersectionPoint);
        }

        //https://www.youtube.com/watch?v=c065KoXooSw
        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
        static bool GetLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
            Vector2 r = b - a;
            Vector2 s = d - c;

            float dist = (r.x * s.y) - (r.y * s.x);
            float u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / dist;
            float t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / dist;

            intersectionPoint = a + (t * r);

            if(u < 0 || u > 1 || t < 0 || t > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
