using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool doubleDoor = false;
    public Transform hinge1 = null;
    public Transform hinge2 = null;




    public bool IsInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public string ObjectName => throw new System.NotImplementedException();

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        throw new System.NotImplementedException();
    }
}

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var doorScript = target as Door;

        doorScript.doubleDoor = EditorGUILayout.Toggle("Double Door", doorScript.doubleDoor);

        doorScript.hinge1 = EditorGUILayout.ObjectField("Hinge 1", doorScript.hinge1, typeof(Transform), true) as Transform;
        if (doorScript.doubleDoor)
        {
            doorScript.hinge2 = EditorGUILayout.ObjectField("Hinge 2", doorScript.hinge2, typeof(Transform), true) as Transform;
        }
    }
}