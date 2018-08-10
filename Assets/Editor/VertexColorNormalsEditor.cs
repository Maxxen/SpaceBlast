using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VertexColorNormalsScript))]
public class VertexColorNormalsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VertexColorNormalsScript script = (VertexColorNormalsScript)target;

        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
    }
}
