#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceGetAll))]
public class ResourceGetAllEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResourceGetAll myTarget = (ResourceGetAll)target;

        if (GUILayout.Button("Load Spawnables")) {
            myTarget.GetResources();
         }
    }
}

#endif