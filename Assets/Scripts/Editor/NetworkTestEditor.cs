using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkTest))]
public class NetworkTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);


        NetworkTest networkTarget = target as NetworkTest;

        var prefabRef = serializedObject.FindProperty("playerPrefab");

        if (GUILayout.Button("Set Player as Survivor"))
        {
            prefabRef.objectReferenceValue = networkTarget.survivorPrefab;
        }
        if (GUILayout.Button("Set Player as Master"))
        {
            prefabRef.objectReferenceValue = networkTarget.masterPrefab;
        }
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(20);
        networkTarget.showNetworkSettings = EditorGUILayout.Toggle("Show Network Settings", networkTarget.showNetworkSettings);
        EditorUtility.SetDirty(target);
        if (networkTarget.showNetworkSettings)
        {
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }        

    }
}
