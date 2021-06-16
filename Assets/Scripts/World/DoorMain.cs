using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

public class DoorMain : NetworkBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool biDirectional = false; //Can the door swing both ways? Or only open in one direction.

    public float openRotation = 88;
    public float openBackRotation = -88;

    public float openTime = 1f;

    public bool doubleDoor = false;
    public Transform hinge1 = null;
    public Transform hinge2 = null;

    public bool open = false;



    public bool IsInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public string ObjectName => throw new System.NotImplementedException();

    [Server]
    public void Svr_Interact(GameObject interacter)
    {
        ChangeState(CheckAngle(interacter.transform));
    }

    private void ChangeState(bool front)
    {
        if (isMoving)
        {
            StopCoroutine(changeCo);
            isMoving = false;
        }
        changeCo = IEChange(front);
        StartCoroutine(changeCo);
    }

    private IEnumerator changeCo;
    private bool isMoving;
    IEnumerator IEChange(bool front)
    {
        isMoving = true;

        open = !open;

        float time = openTime;
        float elapsedTime = 0.0f;

        if (doubleDoor)
        {
            Quaternion startingRotation1 = hinge1.localRotation;
            Quaternion startingRotation2 = hinge2.localRotation;

            Quaternion targetRotation1 = Quaternion.Euler(new Vector3(0, open ? (biDirectional ? (front ? openRotation : openBackRotation) : openRotation) : 0, 0));
            Quaternion targetRotation2 = Quaternion.Euler(new Vector3(0, open ? (biDirectional ? (front ? -openRotation : -openBackRotation) : -openRotation) : 0, 0));

                print(new Vector3(0, hinge2.localEulerAngles.y + (open ? openRotation : 0), 0));

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                hinge1.localRotation = Quaternion.Slerp(startingRotation1, targetRotation1, (elapsedTime / time));
                hinge2.localRotation = Quaternion.Slerp(startingRotation2, targetRotation2, (elapsedTime / time));

                yield return new WaitForEndOfFrame();

            }
        }
        else
        {
            Quaternion startingRotation1 = hinge1.localRotation;

            Quaternion targetRotation1 = Quaternion.Euler(new Vector3(0,
            open ? (biDirectional ? (front ? openRotation : openBackRotation) : openRotation) : 0, 0));

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                hinge1.localRotation = Quaternion.Slerp(startingRotation1, targetRotation1, (elapsedTime / time));

                yield return new WaitForEndOfFrame();

            }
        }
    }

    private bool CheckAngle(Transform interactor)
    {
        Vector3 debugPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.DrawRay(debugPos, transform.forward * 2, Color.cyan, 2f, false);

        Vector3 dir = interactor.position - transform.position;
        float angle = Vector3.Angle(dir, transform.forward);

        bool front = angle <= 90;

        Debug.DrawLine(debugPos, interactor.position, front ? Color.green : Color.magenta,2f, false);

        return front;
    }
}





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
        doorScript.openRotation = EditorGUILayout.Slider("Open Rotation",doorScript.openRotation, 1, 88f);
        if (doorScript.biDirectional)
        {
            doorScript.openBackRotation = EditorGUILayout.Slider("Open Back Rotation",doorScript.openBackRotation, -1, -88f);
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