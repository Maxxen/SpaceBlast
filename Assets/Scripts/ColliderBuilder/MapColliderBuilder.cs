using Assets.Scripts.ColliderBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class MapColliderBuilder
    {
        Polygon floorPolygon;
        List<Polygon> floorEdgeLoops;

        public MapColliderBuilder(List<Edge> inputEdges)
        {
            var edgeLoops = GroupAndSortEdges(inputEdges);
            floorEdgeLoops = edgeLoops.Select((l) => new Polygon(l)).ToList();
            floorPolygon = FoldPolygons(floorEdgeLoops);
            //TriangulateLoop
        }

        public Mesh GenerateWallColliders(int height)
        {
            List<int> indices = new List<int>();
            List<Vector3> verts = new List<Vector3>();

            foreach(Polygon p in floorEdgeLoops)
            {
                //TODO, dont add duplicate vertices
                for(int i = 0; i < p.verts.Count; i++)
                {
                    var index = verts.Count;

                    var p1 = p.verts[i];
                    var p2 = p.verts[i + 1];

                    var v0 = new Vector3(p1.x, 0, p1.y);
                    var v1 = new Vector3(p1.x, height, p1.y);
                    var v2 = new Vector3(p2.x, height, p2.y);
                    var v3 = new Vector3(p2.x, 0, p2.y);

                    verts.Add(v0);
                    verts.Add(v1);
                    verts.Add(v2);
                    verts.Add(v3);

                    //Add triangles in clockwise order
                    if (p.ClockWiseWindingOrder)
                    {
                        indices.Add(index);
                        indices.Add(index + 1);
                        indices.Add(index + 2);
                        indices.Add(index + 2);
                        indices.Add(index + 3);
                        indices.Add(index);
                    }
                    else
                    {
                        indices.Add(index);
                        indices.Add(index + 3);
                        indices.Add(index + 2);
                        indices.Add(index + 2);
                        indices.Add(index + 1);
                        indices.Add(index);
                    }
                }
            }

            return new Mesh { vertices = verts.ToArray(), triangles = indices.ToArray() };
        }

        public void TestSort()
        {
            var randomCircularEdgeList = new List<Edge>();
            randomCircularEdgeList.Add(new Edge(new Vector2(0, 0), new Vector2(1, 0)));
            randomCircularEdgeList.Add(new Edge(new Vector2(-1, 0), new Vector2(0, 0)));
            randomCircularEdgeList.Add(new Edge(new Vector2(1, 0), new Vector2(2, 0)));
            randomCircularEdgeList.Add(new Edge(new Vector2(2, 0), new Vector2(-1, 0)));

            var test1 = GroupAndSortEdges(randomCircularEdgeList);



            //var tripleSquare = new List<Edge>();
            //tripleSquare.Add(new MarchingSquareTile(11).GetEdge(new Vector3(1, 0, 0)).Value);
            //tripleSquare.Add(new MarchingSquareTile(7).GetEdge(new Vector3(0, 0, 0)).Value);
            //tripleSquare.Add(new MarchingSquareTile(14).GetEdge(new Vector3(0, 0, 1)).Value);
            //tripleSquare.Add(new MarchingSquareTile(13).GetEdge(new Vector3(1, 0, 1)).Value);

            //var test2 = GroupAndSortEdges(tripleSquare);

        }

        public List<List<Edge>> GroupAndSortEdges(List<Edge> edges)
        {
            List<List<Edge>> edgeLoops = new List<List<Edge>>();
            var lookup = new Dictionary<Vector2, Edge>(edges.Capacity);
            edges.ForEach((e) => lookup.Add(e.v1, e));

            while (edges.Any())
            {
                Edge start = edges.First();
                Edge current = start;
                Edge next;
                
                List<Edge> currentEdgeLoop = new List<Edge>();

                do
                {
                    edges.Remove(current);
                    currentEdgeLoop.Add(current);
                    if(lookup.TryGetValue(current.v2, out next))
                    {
                        current = next;
                    }
                    else
                    {
                        throw new Exception("Edges does not from a loop!, no connection at: " + current.v2);
                    }

                } while (current != start);

                //while(lookup.TryGetValue(current.v2, out other))
                //{
                //    edges.Remove(other);
                //    currentEdgeLoop.Add(other);
                //    current = other;
                //}

                edgeLoops.Add(currentEdgeLoop);
                
            }

            return edgeLoops;
        }

              
        public Polygon FoldPolygons(List<Polygon> polygons)
        {
            polygons.Sort((p1, p2) => p1.CompareTo(p2));
            return polygons.Aggregate((outer, inner) => ConnectPolygons(outer, inner));
        }


        public Polygon ConnectPolygons(Polygon outer, Polygon inner)
        {        
            //Search inner polygon for rightmost vertex M (highest .x value)
            Vector2 M = inner.verts.OrderByDescending((v) => v.x).First();

            //I is the intersection point where the ray from M intersects an edge on the outer loop
            Vector2 I = Vector2.zero;

            //P is the closest vertex to M on the edge where I intersects
            Vector2 P = Vector2.zero;

            for(int i = 0; i < outer.verts.Count() - 1; i++)
            {
                var v1 = outer.verts[i];
                var v2 = outer.verts[i + 1];

                if((v1.x >= M.x || v2.x >= M.x) && ((v1.y >= M.y && v2.y <= M.y) || (v2.y >= M.y && v1.y <= M.y)))
                {

                    var intersectionPoint = Vector2.zero;
                    if(Loop<Vector2>.RayLineIntersection(M, new Vector2(1, 0), v1, v2, out intersectionPoint))
                    {
                        if(intersectionPoint.x < I.x)
                        {
                            I = intersectionPoint;
                            P = ((v1.x < v2.x) ? v1 : v2);
                        }
                    }
                }
            }

            if (outer.verts.Contains(I))
            {
                //connect loops with M and I (if I is a vertex on outerloop)
                //Very likely since the tiles are grid-aligned
                return CreatePolygonConnection(inner.verts.IndexOf(M), outer.verts.IndexOf(I), inner, outer);
            }
            else
            {
                //Get reflex and convex vertices
                List<Vector2> reflex = new List<Vector2>();
                List<Vector2> convex = new List<Vector2>();

                inner.GetConvexAndReflex(convex, reflex);
 
                //Look for reflex inner loop points inside M I P triangle
                var inside = reflex.Where((v) => IsInTriangle(v, M, I, P));
                if(!inside.Any())
                {
                    //Connect loops with M and P
                    return CreatePolygonConnection(inner.verts.IndexOf(M), outer.verts.IndexOf(P), inner, outer);
                }
                else
                {
                    //Find reflex vert R inside triangle that minimizes the angle between (1,0) and the line {m, v} 
                    //TODO replace Vector2.angle with dot product method
                    var r = inside.OrderByDescending((v) => Vector2.Angle(new Vector2(1, 0), v - M)).Last();

                    //Connect loops with R and P
                    return CreatePolygonConnection(inner.verts.IndexOf(r), outer.verts.IndexOf(P), inner, outer);
                }
            }
        }

        //Needs testing
        private Polygon CreatePolygonConnection(int innerIndex, int outerIndex, Polygon inner, Polygon outer)
        {
            //Make sure inner polygon have reverse winding order
            if (inner.ClockWiseWindingOrder == outer.ClockWiseWindingOrder)
            inner.ReverseWindingOrder();

            //Shift inner polygon vertices to start with desirded connection vertex
            var innerShifted = new List<Vector2>();
            for(int i = innerIndex; i < inner.verts.Count; i++)
            {
                innerShifted.Add(inner.verts[i]);
            }

            var newVerts = new Loop<Vector2>();
            for(int i = 0; i < outer.verts.Count; i++)
            {
                newVerts.Add(outer.verts[i]);
                if(i == outerIndex)
                {
                    newVerts.Concat(innerShifted);
                    newVerts.Add(outer.verts[i]);
                }
            }

            return new Polygon(newVerts); 
        }

        private void TriangulateByEarClip(Polygon polygon)
        {
            List<Vector2> convex = new List<Vector2>();
            List<Vector2> reflex = new List<Vector2>();

            polygon.GetConvexAndReflex(convex, reflex);
            Loop<Vector2> ears = new Loop<Vector2>();

            for(int i = 0; i < polygon.verts.Count; i++)
            {
                var a = polygon.verts[i - 1];
                var b = polygon.verts[i];
                var c = polygon.verts[i + 1];

                if (!reflex.Any((v) => IsInTriangle(v, a, b, c)))
                {
                    ears.Add(b);
                }
            }          
        }

        //https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle 
        private bool IsInTriangle(Vector2 v, Vector2 a, Vector2 b, Vector2 c)
        {
            var s = a.y * c.x - a.x * c.y + (c.y - a.y) * v.x + (a.x - c.x) * v.y;
            var t = a.x * b.y - a.y * b.y + (a.y - b.y) * v.x + (b.x - a.x) * v.y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -b.y * c.x + a.y * (c.x - b.x) + a.x * (b.y - c.y) + b.x * c.y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) <= A;
        }
    }
}