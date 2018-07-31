using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ColliderBuilder
{
    enum WindingOrder { cw, ccw }
    class Polygon : IComparable<Polygon>
    {
        //TODO, both Loop and List should be LinkedList
        public readonly CyclicalLinkedList<Vector2> verts;
        public CyclicalLinkedList<Vector2> ears;
        public Loop<Vector2> reflex;
        public Loop<Vector2> convex;

        public WindingOrder WindingOrder { get; private set; }

        public Polygon(List<Edge> edges)
        {
            this.verts = new CyclicalLinkedList<Vector2>(EdgeListToVertexLoop(edges));
            RecalculateWindingOrder();
        }

        public Polygon(CyclicalLinkedList<Vector2> verts)
        {
            this.verts = verts;
            RecalculateWindingOrder();
        }

        //Sorts in clockwise order
        public LinkedLoop<Vector2> EdgeListToVertexLoop(List<Edge> edgeList)
        {
            LinkedLoop<Vector2> vertexLoop = new LinkedLoop<Vector2>();
            List<Vector2> messyVertexLoop = edgeList.Select((Edge e) => e.v1).ToList();
            messyVertexLoop.Add(edgeList.Last().v2);

            // Here we run along the loop and remove "unecessary" vertices along "straight lines"
            // Look at three vertices at a time, if we come across a bend add the vertex, otherwise just move along.
            vertexLoop.AddLast(messyVertexLoop[0]);

            var v1 = messyVertexLoop[0];
            for(int i = 1; i < messyVertexLoop.Count - 1; i++)
            {
                var v2 = messyVertexLoop[i];
                var v3 = messyVertexLoop[i + 1];

                // If the dot product of the lines (v1,v2) and (v2,v3) is 1, they are parallel, 
                // which means that v2 is on the straight line (v1, v3)
                // thus we dont need to add v2 to preserve the shape of the polygon
                if(Vector2.Dot(v1 - v2, v2 - v3) != 1)
                {
                    vertexLoop.AddLast(v2);
                }
                v1 = v2;
            }

            return vertexLoop;
        }


        public void ReverseWindingOrder()
        {
            WindingOrder = WindingOrder == WindingOrder.cw ? WindingOrder.ccw : WindingOrder.cw;
            verts.Reverse();
            //earVerts.Reverse();
            //convexVerts.Reverse();
            //reflexVerts.Reverse();
        }

        public void SetWindingOrder(WindingOrder order)
        {
            if(order == WindingOrder)
            {
                return;
            }
            else
            {
                ReverseWindingOrder();
            }
        }

        private WindingOrder RecalculateWindingOrder()
        {
            int cw = 0;
            int ccw = 0;

            verts.ForEachTriplet((v1, v2, v3) => 
                {
                    if (ClockwiseWinding(v1, v2, v3))
                        cw++;
                    else
                        ccw++;
                }
            );

            return cw > ccw ? WindingOrder.cw : WindingOrder.ccw;
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
            float maxDistance = potentialEdges.OrderByDescending((v) => v.x).FirstOrDefault().x + 10;
            for(int i = 0; i < potentialEdges.Count - 1; i++)
            {
                Vector2 e1 = potentialEdges[i];
                Vector2 e2 = potentialEdges[i + 1];

                if(MapColliderBuilder.IsIntersecting(point, point + new Vector2(0, maxDistance), e1, e2)){
                    intersections++;
                }
            }

            intersections = intersections % 2;
            //Odd intersections, must be inside
            if (intersections == 1)
                return 1;
            //Even intersection or no intersection, must be bigger
            if (intersections == 0)
                return -1;

            return 0;
        }


        // https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
        public Mesh TriangulateByEarClip()
        {
            List<int> indices = new List<int>();
            List<Vector2> originialVerts = new List<Vector2>(verts);

            CalculateConvexAndReflex();
            CalculateEars();

            while(verts.Count > 3 && ears.Count > 0)
            {
                ClipEar(indices, originialVerts);
            }

            if (verts.Count == 3)
            {

                var vert = verts.First;

                indices.Add(originialVerts.IndexOf(vert.Value));
                indices.Add(originialVerts.IndexOf(vert.Next.Value));
                indices.Add(originialVerts.IndexOf(vert.Next.Next.Value));
            }

            var meshTris = indices.ToArray();
            var meshVerts = originialVerts.ConvertAll((v) => new Vector3(v.x, 0, v.y)).ToArray();

            return new Mesh() { vertices = meshVerts, triangles = meshTris };
        }

        void ClipEar(List<int> indices, List<Vector2> originialVerts)
        {
            var ear = ears.First.Value;
            var earNode = verts.Find(ear);

            var prev = earNode.Prev.Value;
            var next = earNode.Next.Value;
            //var prev = verts[verts.IndexOf(ear) - 1];
            //var next = verts[verts.IndexOf(ear) + 1];

            indices.Add(originialVerts.IndexOf(ear));
            indices.Add(originialVerts.IndexOf(next));
            indices.Add(originialVerts.IndexOf(prev));

            ears.RemoveFirst();
            verts.Remove(ear);
            //verts.RemoveAt(verts.IndexOf(ear));

            UpdateVertex(prev);
            UpdateVertex(next);

        }

        void UpdateVertex(Vector2 vertex)
        {        
            if (reflex.Contains(vertex))
            {
                if (IsConvex(vertex))
                {
                    reflex.Remove(vertex);
                    convex.Add(vertex);
                }
            }

            if (convex.Contains(vertex))
            {
                bool wasEar = ears.Contains(vertex);
                bool isNowEar = IsEar(vertex);

                if (wasEar && !isNowEar)
                {
                    ears.Remove(vertex);
                }
                else if (!wasEar && isNowEar)
                {
                    ears.AddFirst(vertex);
                }
                //else, still ear
            }
        }


        public bool IsConvex(Vector2 vertex)
        {
            var node = verts.Find(vertex);

            var prev = node.Prev.Value;
            var next = node.Next.Value;
            return IsConvex(prev, vertex, next);
        }

        public bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            //return ((b.x - a.x) * (c.y - b.y) - (c.x - b.x) * (b.y - a.y) <= 0);

            Vector2 d1 = (c - a).normalized;
            Vector2 d2 = (b - c).normalized;
            Vector2 n2 = new Vector2(-d2.y, d2.x);

            return (Vector2.Dot(d1, n2) <= 0f);
        }

        public void CalculateConvexAndReflex()
        {
            reflex = new Loop<Vector2>();
            convex = new Loop<Vector2>();

            verts.ForEachTriplet((prev, current, next) => 
                {
                    if (IsConvex(prev, current, next))
                        convex.Add(current);
                    else
                        reflex.Add(current);
                }
            );
        }

        bool IsEar (Vector2 vertex)
        {
            var node = verts.Find(vertex);
            var prev = node.Prev.Value;
            var next = node.Next.Value;

            return IsEar(prev, vertex, next);
        }


        bool IsEar(Vector2 a, Vector2 b, Vector2 c)
        {
            foreach(Vector2 v in reflex)
            {
                if(v == a || v == b || v == c)
                {
                    continue;
                }
                if (IsInTriangle(v, a, b, c))
                    return false;
            }
            return true;
            //return !reflex.Any((v) => IsInTriangle(v, a, b, c) && !(v == a || v == b || v == c));
        }

        //Construct list with all ear vertices.
        public void CalculateEars()
        {
            ears = new CyclicalLinkedList<Vector2>();
            
            verts.ForEachTriplet((prev, current, next) =>
                {
                    if (convex.Contains(current))
                    {
                        if(IsEar(prev, current, next))
                        {
                            ears.AddLast(current);
                        }
                    }
                }
            );
        }

        // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle 
        public bool IsInTriangle(Vector2 v, Vector2 a, Vector2 b, Vector2 c)
        {
            return PointInTriangle(v, a, b, c);
            //var s = a.y * c.x - a.x * c.y + (c.y - a.y) * v.x + (a.x - c.x) * v.y;
            //var t = a.x * b.y - a.y * b.y + (a.y - b.y) * v.x + (b.x - a.x) * v.y;

            //if ((s < 0) != (t < 0))
            //    return false;

            //var A = -b.y * c.x + a.y * (c.x - b.x) + a.x * (b.y - c.y) + b.x * c.y;
            //if (A < 0.0)
            //{
            //    s = -s;
            //    t = -t;
            //    A = -A;
            //}
            //return s > 0 && t > 0 && (s + t) <= A;
        }
        float sign (Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        bool PointInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;

            b1 = sign(pt, v1, v2) < 0.0f;
            b2 = sign(pt, v2, v3) < 0.0f;
            b3 = sign(pt, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }

    }
}
