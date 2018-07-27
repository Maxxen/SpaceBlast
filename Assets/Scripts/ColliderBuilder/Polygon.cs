using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ColliderBuilder
{
    class Polygon : IComparable<Polygon>
    {
        //TODO, both Loop and List should be LinkedList
        //TODO out lists are not necessary.
        public readonly Loop<Vector2> verts;
        Loop<Vector2> earVerts;
        List<Vector2> convexVerts;
        List<Vector2> reflexVerts;

        public bool ClockWiseWindingOrder { get; private set; }

        public Polygon(List<Edge> edges)
        {
            this.verts = EdgeListToVertexLoop(edges);
            this.ClockWiseWindingOrder = true;
        }

        public Polygon(List<Vector2> verts)
        {
            this.verts = (Loop<Vector2>)verts;
            CalculateWindingOrder();
        }
       
        //Sorts in clockwise order
        public Loop<Vector2> EdgeListToVertexLoop(List<Edge> edgeList)
        {
            Loop<Vector2> vertexLoop = new Loop<Vector2>();
            List<Vector2> messyVertexLoop = edgeList.Select((Edge e) => e.v1).ToList();
            messyVertexLoop.Add(edgeList.Last().v2);

            //Here we run along the loop and remove "unecessary" vertices along "straight lines"
            //Look at three vertices at a time, if we come across a bend add the vertex, otherwise just move along.
            vertexLoop.Add(messyVertexLoop[0]);
            var v1 = messyVertexLoop[0];
            for(int i = 1; i < messyVertexLoop.Count - 1; i++)
            {
                var v2 = messyVertexLoop[i];
                var v3 = messyVertexLoop[i + 1];

                if(Vector2.Angle(v1, v2) != Vector2.Angle(v1, v3))
                {
                    vertexLoop.Add(v2);
                }
            }

            return vertexLoop;
        }

        //Needs testing
        public void GetConvexAndReflex(out List<Vector2> convex, out List<Vector2> reflex)
        {
            convex = new List<Vector2>();
            reflex = new List<Vector2>();

            for(int i = 0; i < verts.Count; i++)
            {
                var a = verts[i - 1];
                var b = verts[i];
                var c = verts[i + 1];

                if (IsConvex(a, b, c))
                    convex.Add(b);
                else
                    reflex.Add(b);
            }
        }

        public bool IsConvex(int index)
        {
            var a = verts[index - 1];
            var b = verts[index];
            var c = verts[index + 1];

            return IsConvex(a, b, c);
        }

        public bool IsConvex(Vector2 vert)
        {
            var index = verts.IndexOf(vert);
            return IsConvex(index);
        }

        public bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            return ((b.x - a.x) * (c.y - b.y) - (c.x - b.x) * (b.y - a.y) > 0);
        }

        public void ReverseWindingOrder()
        {
            ClockWiseWindingOrder = !ClockWiseWindingOrder;
            verts.Reverse();
            earVerts.Reverse();
            convexVerts.Reverse();
            reflexVerts.Reverse();
        }

        public void CalculateWindingOrder()
        {
            ClockWiseWindingOrder = ClockwiseWinding(verts);
        }

        private bool ClockwiseWinding(Loop<Vector2> loop)
        {
            int cw = 0;
            int ccw = 0;
            var v1 = loop.First();
            for(int i = 1; i < loop.Count; i++)
            {
                var v2 = loop[i];
                var v3 = loop[i + 1];

                if (ClockwiseWinding(v1, v2, v3))
                    cw++;
                else
                    ccw++;

                v1 = v2;
            }

            return cw > ccw;
        }

        //Needs testing
        private bool ClockwiseWinding(Vector2 a, Vector2 b, Vector2 c)
        {
            var v1 = b - a;
            var v2 = c - a;

            var cross = v1.x * v2.y - v1.y * v2.x;

            return cross >= 0;
        }


        //Point in polygon raycasting algorithm
        //https://en.wikipedia.org/wiki/Point_in_polygon
        //This allows us to sort polygons into a hierarchy based on if they are inside one another with LINQ sort queries
        //Might be slight abuse of IComparable, but whatever
        public int CompareTo(Polygon other)
        {
            Vector2 point = verts.First();
            int intersections = 0;
            List<Vector2> potentialEdges = other.verts.Where((v) => v.x > point.x).ToList();
            float maxDistance = potentialEdges.OrderByDescending((v) => v.x).First().x + 10;
            for(int i = 0; i < potentialEdges.Count - 1; i++)
            {
                Vector2 e1 = potentialEdges[i];
                Vector2 e2 = potentialEdges[i + 1];

                if(Loop<Vector2>.IsIntersecting(point, point + new Vector2(0, maxDistance), e1, e2)){
                    intersections++;
                }
            }

            intersections = intersections % 2;
            //Odd intersections, must be inside
            if (intersections == 1)
                return -1;
            //Even intersection or no intersection, must be bigger
            if (intersections == 0)
                return 1;

            return 0;
        }
    }
}
