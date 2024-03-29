﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitBodyPart))]
public class UnitBodyPartEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as UnitBodyPart;

        script.bodyPart = (BodyParts)EditorGUILayout.EnumPopup("Body Part", script.bodyPart);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Dismemberment", EditorStyles.boldLabel);

        serializedObject.FindProperty("detachable").boolValue = EditorGUILayout.Toggle("Detachable", script.detachable);

        if (script.detachable)
        {
            serializedObject.FindProperty("onlyDetachOnDeath").boolValue = EditorGUILayout.Toggle("Only Detach On Death", script.onlyDetachOnDeath);
            EditorGUILayout.Space();
            serializedObject.FindProperty("attachedPart").objectReferenceValue = EditorGUILayout.ObjectField("Attached Part", script.attachedPart, typeof(GameObject), true) as GameObject;
            serializedObject.FindProperty("partTransform").objectReferenceValue = EditorGUILayout.ObjectField("Part Transform", script.partTransform, typeof(Transform), true) as Transform;
            EditorGUILayout.Space();
            serializedObject.FindProperty("bodyBloodEmitter").objectReferenceValue = EditorGUILayout.ObjectField("Body Blood Emitter", script.bodyBloodEmitter, typeof(GameObject), true) as GameObject;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
