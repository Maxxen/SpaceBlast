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

                    var v0 = new Vector3(p1.x, 0, p1.y);
                    var v1 = new Vector3(p1.x, height, p1.y);
                    var v2 = new Vector3(p2.x, height, p2.y);
                    var v3 = new Vector3(p2.x, 0, p2.y);

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
            Vector2 m = inner.verts.OrderByDescending((v) => v.x).First();
            Vector2 p = outer.verts.Where((v) => v.x >= m.x).OrderByDescending((v) => (v - m).sqrMagnitude).Last();

            return CreatePolygonConnection(m, p, inner, outer);
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
            //Make sure inner polygon have reverse winding order
            if (inner.WindingOrder == outer.WindingOrder)
                inner.ReverseWindingOrder();

            //Shift inner polygon vertices to start with desired connection vertex
            while (inner.verts.First.Value != innerVert)
                inner.verts.RotateLeft();

            //Shift outer polygon vertices to end with desired connection vertex
            while (outer.verts.Last.Value != outerVert)
                outer.verts.RotateLeft();

            //Add the duplicate verts in order to make sure edge is two sided.
            outer.verts.AddFirst(outer.verts.Last.Value);
            inner.verts.AddFirst(inner.verts.Last.Value);

            outer.verts.Concat(inner.verts);

            return new Polygon(outer.verts);
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

        public bool RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
            //Investigate max distance further
            return GetLineIntersection(rayOrigin, rayOrigin + (rayDirection * (Math.Max(c.x - rayOrigin.x, d.x - rayOrigin.x)) * 2), c, d, out intersectionPoint);
        }

        //https://www.youtube.com/watch?v=c065KoXooSw
        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
        bool GetLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersectionPoint)
        {
                       
            Vector2 r = b - a;
            Vector2 s = d - c;

            float dist = (r.x * s.y) - (r.y * s.x);
            float u = ((c.x - a.x) * r.y - (c.y - a.y) * r.x) / dist;
            float t = ((c.x - a.x) * s.y - (c.y - a.y) * s.x) / dist;

            intersectionPoint = a + (t * r);

            return (0 < u && u < 1 && 0 < t && t < 1);
        }
    }
}