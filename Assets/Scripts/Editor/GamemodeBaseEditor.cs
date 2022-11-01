using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gamemode_Survival))]
public class GamemodeBaseEditor : Editor
{    
    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);
        DrawDefaultInspector();
        GUILayout.Space(20);

        Gamemode_Survival networkTarget = target as Gamemode_Survival;

        if (GUILayout.Button("Force End Game"))
        {
            networkTarget.SurvivorsAlive = 0;
        }        
    }
}
