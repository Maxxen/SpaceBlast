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
        List<Edge> inputEdges = new List<Edge>();

        public MapColliderBuilder()
        {
            
        }

        public List<List<Edge>> SortEdgesIntoLoops(List<Edge> edges)
        {
            List<Edge> workingSet = edges;
            List<List<Edge>> loops = new List<List<Edge>>();
            int edgesLeft = workingSet.Count();
            while (edgesLeft != 0)
            {
                List<Edge> loop = new List<Edge>();


                Edge start = workingSet[0];
                Edge current = start;
                //Sort the edges in connective order by finding their corresponding links, shared vertices
                do // TODO, use linq sort to order edges, probably safer
                {
                    loop.Add(current);
                    workingSet.Remove(current);
                    edgesLeft -= 1;
                    current = workingSet.Find((Edge other) => other.v1 == current.v2);
                } while (current != start);

                loops.Add(loop);
            }
            return loops;
        }

        public List<Vector2> MergeEdgeLoops(List<List<Edge>> loops)
        {
            Stack<List<Vector2>> vertexLoops = new Stack<List<Vector2>>();
            loops.ForEach((List<Edge> loop) => vertexLoops.Push(EdgeLoopToVertexLoop(loop)));

            while(vertexLoops.Count() != 1)
            {
                List<Vector2> firstLoop = vertexLoops.Pop();
                List<Vector2> secondLoop = vertexLoops.Pop();

                List<Vector2> connectedLoop;
                //Should be replaced with line-sweep "is inside" algorithm to see if loop is interior. Right now numerical guess will do.
                //TODO, ORDER NESTED HOLES BY THEIR CONVEX HULL SPAN, IE CONNECT BASED ON LOOP HIERARCHY, NOT RANDOMLY LIKE NOW
                if (firstLoop.Count > secondLoop.Count)
                    connectedLoop = ConnectEdgeLoops(firstLoop, secondLoop);
                else
                    connectedLoop = ConnectEdgeLoops(secondLoop, firstLoop);

                vertexLoops.Push(connectedLoop);
            }

            return vertexLoops.Pop();
        }

        //Should just return span, comparsion can be done in other method
        private bool IsEdgeLoopInside(List<Vector2> outer, List<Vector2> inner)
        {
            //Calculate the two furthers points on the convex hull created by the edge loops
            //In other words, the furthest point from the mean, and the furthest point from that point
            //Inneficient and innacurate but simple method, calculate the max span of the edge loops, the larger span is the outer edgeloop
            Vector2 outerMean = Vector2.zero;
            outer.ForEach((v) => outerMean += v);
            outerMean /= outer.Count();

            Vector2 innerMean = Vector2.zero;
            inner.ForEach((v) => innerMean += v);
            innerMean /= inner.Count();

            Vector2 outerV1 = outer.OrderByDescending((v) => Vector2.Distance(v, outerMean)).First();
            Vector2 innerV1 = inner.OrderByDescending((v) => Vector2.Distance(v, innerMean)).First();

            Vector2 outerV2 = outer.OrderByDescending((v) => Vector2.Distance(v, outerV1)).First();
            Vector2 innerV2 = inner.OrderByDescending((v) => Vector2.Distance(v, innerV1)).First(); 
                
            return (Vector2.Distance(outerV1, outerV2) > Vector2.Distance(innerV1, innerV2));
        }

        public List<Vector2> EdgeLoopToVertexLoop(List<Edge> loop)
        {
            List<Vector2> vertexLoop = new List<Vector2>();
            List<Vector2> messyVertexLoop = loop.Select((Edge e) => e.v1).ToList();

            //Here we run along the loop and remove "unecessary" vertices along "straight lines"
            //Look at three vertices at a time, if we come across a bend add the vertex, otherwise just move along.
            vertexLoop.Add(messyVertexLoop[0]);
            for(int i = 0; i < messyVertexLoop.Count - 2; i++)
            {
                var v1 = messyVertexLoop[i];
                var v2 = messyVertexLoop[i + 1];
                var v3 = messyVertexLoop[i + 2];

                if(Vector2.Angle(v1, v2) != Vector2.Angle(v1, v3))
                {
                    vertexLoop.Add(v2);
                }
            }

            return vertexLoop;
        }
        
        public List<Vector2> ConnectEdgeLoops(List<Vector2> outerLoop, List<Vector2> innerLoop)
        {
            //Search inner polygon for rightmost vertex M (highest .x value)
            Vector2 m = innerLoop.OrderByDescending((v) => v.x).First();

            var potentialEdges = outerLoop.Where((v) => v.x >= m.x).ToList();

            Vector2 intersectionPoint = Vector2.zero;
            Vector2 closestVertexToIntersectionPoint = Vector2.zero;

            for(int i = 0; i < potentialEdges.Count() - 1; i++)
            {
                var v1 = potentialEdges[i];
                var v2 = potentialEdges[i + 1];

                var intersection = RayLineIntersection(m, new Vector2(1, 0), v1, v2);
                if(intersection != null)
                {
                    if(intersection.Value.x < intersectionPoint.x)
                    {
                        intersectionPoint = intersection.Value;
                        closestVertexToIntersectionPoint = ((v1.x < v2.x) ? v1 : v2);
                    }                
                }
            }

            //Make triangle between Intersectionpoint, m and closestVertexToIntersectionPoint
            //If any outerloop points are inside, then use them instead as m and repeat.
        }
        
        public Vector2? RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 c, Vector2 d)
        {
            //Investigate max distance further
            return LineSegmentIntersection(rayOrigin, rayOrigin + (rayDirection * (Math.Max(c.x - rayOrigin.x, d.x - rayOrigin.x)) * 2), c, d);
        }

        //https://www.youtube.com/watch?v=c065KoXooSw
        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
        Vector2? LineSegmentIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Vector2 r = b - a;
            Vector2 s = d - c;

            float dist = (r.x * s.y) - (r.y * s.x);
            float u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / dist;
            float t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / dist;

            if(u < 0 || u > 1 || t < 0 || t > 1)
            {
                return null;
            }
            else
            {
                return a + (t * r);
            }
        }

    }

    struct Edge : IComparable
    {
        public Vector2 v1, v2;
        public Edge(Vector2 v1, Vector2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public int CompareTo(object obj) // TODO, Use linq sort to order edges
        {
            if (obj == null)
            {
                throw new ArgumentException();
            }
            else
            {
                Edge other = (Edge)obj;
                if (this.v1 == other.v2)
                    return 1;
                if (this.v2 == other.v1)
                    return -1;

                return 0;
            }
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