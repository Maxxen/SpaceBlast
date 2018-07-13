using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VertexColorNormalsScript : MonoBehaviour {


    public void Generate()
    {
        var mesh = GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        float precision = 0.001f;

        var vertices = mesh.vertices;
        var uvs = mesh.uv;
        var normals = mesh.normals;
        int[] map = new int[vertices.Length];

        List<int> newVerts = new List<int>();  

        for(int v1 = 0; v1 < vertices.Length; v1++)
        {
            var pos = vertices[v1];
            var uv = uvs[v1];
            var n = normals[v1];
            bool duplicate = false;

            for(int v2 = 0; v2 < newVerts.Count; v2++)
            {
                var p2 = newVerts[v2];

                if((vertices[p2] - pos).sqrMagnitude < precision)
                {
                    map[v1] = v2;
                    duplicate = true;
                    break;
                }
            }

            if (!duplicate)
            {
                map[v1] = newVerts.Count;
                newVerts.Add(v1);
            }
        }

        var verts2 = new Vector3[newVerts.Count];
        var normals2 = new Vector3[newVerts.Count];

        for(int i = 0; i < newVerts.Count; i++)
        {
            verts2[i] = vertices[newVerts[i]];
            normals2[i] = normals[newVerts[i]];
        }

        var newTris = mesh.triangles;

        for(int i = 0; i < newTris.Length; i++)
        {
            newTris[i] = map[newTris[i]];
        }

        var newmesh = new Mesh() { vertices = verts2, triangles = newTris, normals = normals2};
        newmesh.RecalculateNormals();
        var smoothNormals = newmesh.normals;
        
        //Encode the "smooth normals" from the second mesh copy into the first mesh's tangents.
        //Only normals and tangents are left intact for skinned mesh renderers, which is why we cant store the smooth normals into vertex color.
        //We need smooth normals in order to produce a uniform and even outline in the shader.

        var newTangent = new Vector4[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {        
            Vector3 v = smoothNormals[map[i]];
            Vector4 tangent = new Vector4(v.x, v.y, v.z, 0);

            newTangent[i] = tangent;
        }

        mesh.tangents = newTangent;
    }
}
