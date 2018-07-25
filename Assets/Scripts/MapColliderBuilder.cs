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
            loops.ForEach((List<Edge> loop) => vertexLoops.Push(loop.Select((Edge e) => e.v1).ToList()));

            while(vertexLoops.Count() != 1)
            {
                List<Vector2> firstLoop = vertexLoops.Pop();
                List<Vector2> secondLoop = vertexLoops.Pop();

                List<Vector2> connectedLoop;
                //Should be replaced with line-sweep "is inside" algorithm to see if loop is interior. Right now numerical guess will do.
                if (firstLoop.Count > secondLoop.Count)
                    connectedLoop = ConnectEdgeLoops(firstLoop, secondLoop);
                else
                    connectedLoop = ConnectEdgeLoops(secondLoop, firstLoop);

                vertexLoops.Push(connectedLoop);
            }

            return vertexLoops.Pop();
        }
        
        public List<Vector2> ConnectEdgeLoops(List<Vector2> outerLoop, List<Vector2> innerLoop)
        {
            //Search inner polygon for rightmost vertex M (highest .x value)
            Vector2 m = innerLoop.Aggregate((rightMost, vert) => rightMost.x > vert.x ? rightMost : vert);

            //Intersect ray shot from M with edges in outer loop right of M. If ray hits another vert, connect the two
            var potentialEdges = outerLoop.Where((v) => v.x >= m.x).ToList();

            //If there is a vertex on the same y axis it must be visible from m
            var rightmostCandidate = potentialEdges.Where((v) => v.y == m.y).Aggregate((leftMost, vert) => leftMost.x > vert.x ? leftMost : vert);
            if(rightmostCandidate != null)
            {
                Vector2 p = rightmostCandidate;
                //connect m and p
            }

            for(int i = 0; i < potentialEdges.Count() - 1; i++)
            {
                var v1 = potentialEdges[i];
                var v2 = potentialEdges[i + 1];

                if(m.y < v1.y )
            }
            //Else 
        }
        
        public Vector2 RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 LineStart, Vector2 lineEnd)
        {
            
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection);
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