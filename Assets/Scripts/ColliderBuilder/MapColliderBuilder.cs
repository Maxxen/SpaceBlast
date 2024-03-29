﻿using Assets.Scripts.ColliderBuilder;
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
        List<Polygon> wallPolygons;

        public MapColliderBuilder(List<Edge> inputEdges)
        {
            var edgeLoops = GroupAndSortEdges(inputEdges);
            wallPolygons = edgeLoops.Select((l) => new Polygon(l)).ToList();
        }

        /// <summary>
        /// Sorts and link togheter edges into isolated edge-loops.
        /// Requires that every edge share each of its vertices with another edge in order to ensure that all loops are closed.
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
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
                    if (lookup.TryGetValue(current.v2, out next))
                    {
                        current = next;
                    }
                    else
                    {
                        throw new Exception("Edges does not form a loop!, no connection at: " + current.v2);
                    }

                } while (current != start);

                edgeLoops.Add(currentEdgeLoop);
            }
            return edgeLoops;
        }

        /// <summary>
        /// Creates a "wall" mesh with a given height by "raising" each edge in the wall polygons into quads.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public Mesh GenerateWallMesh(int height)
        {
            List<int> indices = new List<int>();
            List<Vector3> verts = new List<Vector3>();

            foreach (Polygon p in wallPolygons)
            {
                //p.SetWindingOrder(WindingOrder.cw);
                //TODO, dont add duplicate vertices
                p.verts.ForEachPair((p1, p2) =>
                {
                    var index = verts.Count;

                    var v0 = new Vector3(p1.x, -0.5f, p1.y);
                    var v1 = new Vector3(p1.x, height, p1.y);
                    var v2 = new Vector3(p2.x, height, p2.y);
                    var v3 = new Vector3(p2.x, -0.5f, p2.y);

                    verts.Add(v0);
                    verts.Add(v1);
                    verts.Add(v2);
                    verts.Add(v3);

                    //Add triangles in clockwise order
                    if (p.WindingOrder == WindingOrder.cw)
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


                });
            }

            return new Mesh { vertices = verts.ToArray(), triangles = indices.ToArray() };
        }

        /// <summary>
        /// Triangulates a floor mesh between the wall polygon outlines
        /// WARNING: Will alter wall polygon winding orders! 
        /// Make sure to only call this AFTER wall colliders have been created!
        /// </summary>
        /// <returns>The triangulated mesh</returns>
        public Mesh GenerateFloorMesh()
        {
            if(wallPolygons.Count > 1)
            {
                //TODO: sort into a binary tree, should be faster and easier to connect)
                wallPolygons.Sort();
                //Aggregate works as a Fold, we continue to merge smaller polgyons into the 
                //largest polygon until there are none left.

                var floorPolygon = wallPolygons.Aggregate((outer, inner) => ConnectPolygons(outer, inner));
                return floorPolygon.TriangulateByEarClip();
            }
            else
            {
                return wallPolygons[0].TriangulateByEarClip();
            }
        }

        /// <summary>
        /// Finds the ideal vertices on the respective polygons to form a connection between
        /// and then merges them into one by adding a double sided edge between them
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        private Polygon ConnectPolygons(Polygon outer, Polygon inner)
        {
            //Vector2 m = inner.verts.OrderByDescending((v) => v.x).First();
            //Vector2 p = outer.verts.Where((v) => v.x >= m.x).OrderByDescending((v) => (v - m).sqrMagnitude).Last();

            //return CreatePolygonConnection(m, p, inner, outer);

            //Search inner polygon for rightmost vertex M (highest .x value)
            Vector2 M = inner.verts.OrderByDescending((v) => v.x).First();
            //I is the intersection point where the ray from M intersects an edge on the outer loop
            Vector2? I = null;
            //P is the closest vertex to M on the edge where I lies.
            Vector2? P = null;

            outer.verts.ForEachPair((v1, v2) =>
            {
                if( (v1.x >= M.x || v2.x >= M.x) && ((v1.y >= M.y && v2.y <= M.y) || (v1.y <= M.y && v2.y >= M.y)) )
                {
                    Vector2? intersectionPoint = Utils.RayLineIntersection(M, new Vector2(1, 0), v1, v2);
                    if (intersectionPoint != null)
                    {
                        if( I == null)
                        {
                            I = intersectionPoint;                        
                        }
                        else
                        {
                            I = (intersectionPoint.Value.x < I.Value.x) ? intersectionPoint : I;
                        }
                        P = ((v1.x < v2.x) ? v1 : v2);
                    }
                }
            });
            if(I == null || P == null)
            {

                throw new Exception("Polygon is not inside another polygon!");
            }

            if (outer.verts.Contains(I.Value))
            {
                //connect loops with M and I (if I is a vertex on outerloop)
                //Very likely since the tiles are grid-aligned
                return CreatePolygonConnection(M, I.Value, inner, outer);
            }
            else
            {
                //Get reflex and convex vertices
                outer.CalculateConvexAndReflex();

                //Look for reflex outer loop points inside M I P triangle
                var inside = outer.reflex.Where((v) => Utils.IsInTriangle(v, M, I.Value, P.Value));
                if (!inside.Any())
                {
                    //Connect loops with M and P
                    return CreatePolygonConnection(M, P.Value, inner, outer);
                }
                else
                {
                    //Find reflex vert R inside triangle that minimizes the angle between (1,0) and the line {m, v} 
                    //TODO replace Vector2.angle with dot product method
                    var R = inside.OrderByDescending((v) => Vector2.Angle(new Vector2(1, 0), v - M)).Last();
                    //Connect loops with R and P
                    return CreatePolygonConnection(M, R, inner, outer);
                }
            }
            //return CreatePolygonConnection(innerIndex, outerIndex, inner, outer);

            //return CreatePolygonConnection(m, p, inner, outer);
        }

        /// </summary>
        /// Merges two polygons into one by "adding a double sided edge" between the specified indices.
        /// In reality, shifts the vertices to align and concatenates the vertex lists.
        /// <param name="innerIndex"></param>
        /// <param name="outerIndex"></param>
        /// <param name="inner"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        private Polygon CreatePolygonConnection(Vector2 innerVert, Vector2 outerVert, Polygon inner, Polygon outer)
        {

            inner.SetWindingOrder(WindingOrder.cw);
            outer.SetWindingOrder(WindingOrder.cw);
            CyclicalLinkedList<Vector2> firstHalf = inner.verts.Copy();
            CyclicalLinkedList<Vector2> secondHalf = outer.verts.Copy(); 


            //Shift inner polygon vertices to end with desired connection vertex
            while (firstHalf.Last.Value != innerVert)
                firstHalf.RotateLeft();

            //Shift outer polygon vertices to start with desired connection vertex
            while (secondHalf.First.Value != outerVert)
                secondHalf.RotateLeft();

            //Add the duplicate verts in order to make sure edge is two sided.
            firstHalf.AddFirst(innerVert);
            secondHalf.AddLast(outerVert);

            firstHalf.Concat(secondHalf);

            return new Polygon(secondHalf);

            //This is broken, it is also the solution to the problems were having, fix it PLZ
        }
    }
}