using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkTest))]
public class NetworkTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NetworkTest networkTarget = target as NetworkTest;
        if (GUILayout.Button("Set Player as Survivor"))
        {
            networkTarget.playerPrefab = networkTarget.survivorPrefab;
        }
        if (GUILayout.Button("Set Player as Master"))
        {
            networkTarget.playerPrefab = networkTarget.masterPrefab;
        }

        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}
