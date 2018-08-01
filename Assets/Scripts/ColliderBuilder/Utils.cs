using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ColliderBuilder
{
    static class Utils
    {
        // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle 
        public static bool IsInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;

            b1 = sign(pt, v1, v2) < 0.0f;
            b2 = sign(pt, v2, v3) < 0.0f;
            b3 = sign(pt, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }

        static float sign (Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
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

        public static Vector2? RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 c, Vector2 d)
        {

            Vector2 intersectionPoint = Vector2.zero;
            if(GetLineIntersection(rayOrigin, rayOrigin + (rayDirection * 100000), c, d, out intersectionPoint))
            {
                return intersectionPoint;
            }
            else
            {
                return null;
            }
        }

        //https://www.youtube.com/watch?v=c065KoXooSw
        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
        public static bool GetLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
                       
            Vector2 r = b - a;
            Vector2 s = d - c;

            float dist = (r.x * s.y) - (r.y * s.x);
            float u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / dist;
            float t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / dist;

            intersectionPoint = a + (t * r);

            return (0 <= u && u <= 1 && 0 <= t && t <= 1);
        }
    }
}
