using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitBodyPart))]
public class UnitBodyPartEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as UnitBodyPart;

        EditorGUILayout.EnumPopup("Body Part", script.bodyPart);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Dismemberment", EditorStyles.boldLabel);

        serializedObject.FindProperty("detachable").boolValue = EditorGUILayout.Toggle("Detachable", script.detachable);

        if (script.detachable)
        {
            script.attachedPart = EditorGUILayout.ObjectField("Attached Part", script.attachedPart, typeof(GameObject), true) as GameObject;
            script.detachedPart = EditorGUILayout.ObjectField("Detached Part", script.detachedPart, typeof(GameObject), true) as GameObject;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
