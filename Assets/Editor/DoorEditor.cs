using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorMain))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var doorScript = target as DoorMain;


        EditorGUILayout.LabelField("Door Info", EditorStyles.boldLabel);
        doorScript.open = EditorGUILayout.Toggle("Open", doorScript.open);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Door Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("This bool controls if the door can open both ways.", EditorStyles.centeredGreyMiniLabel);
        doorScript.biDirectional = EditorGUILayout.Toggle("Bi Directional", doorScript.biDirectional);
        doorScript.openRotation = EditorGUILayout.Slider("Open Rotation", doorScript.openRotation, 1, 88f);
        if (doorScript.biDirectional)
        {
            doorScript.openBackRotation = EditorGUILayout.Slider("Open Back Rotation", doorScript.openBackRotation, -1, -88f);
        }
        doorScript.openTime = EditorGUILayout.FloatField("Open Time", doorScript.openTime);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("This bool controls if the door has two hinges.", EditorStyles.centeredGreyMiniLabel);
        doorScript.doubleDoor = EditorGUILayout.Toggle("Double Door", doorScript.doubleDoor);

        doorScript.hinge1 = EditorGUILayout.ObjectField("Hinge 1", doorScript.hinge1, typeof(Transform), true) as Transform;
        if (doorScript.doubleDoor)
        {
            doorScript.hinge2 = EditorGUILayout.ObjectField("Hinge 2", doorScript.hinge2, typeof(Transform), true) as Transform;
        }
    }
}
