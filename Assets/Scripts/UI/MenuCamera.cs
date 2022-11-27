using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraPosition
{
    public string name;
    public Transform position;
}
public class MenuCamera : MonoBehaviour
{
    public static MenuCamera instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private List<CameraPosition> cameraPositions = new List<CameraPosition>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region Camera Position Changes

    private Coroutine MoveCamera;
    public bool moving = false;
    public void ChangePosition(string position)
    {
        bool positionAvailable = false;
        foreach(var cameraPosition in cameraPositions)
        {
            if (cameraPosition.name == position)
            {
                if (cameraPosition.position == null)
                {
                    Debug.LogWarning($"Position with the name: ({position}) had no transform reference. ");
                    continue;
                }
                if (moving) StopCoroutine(MoveCamera);
                MoveCamera = StartCoroutine(MoveToPosition(cameraPosition.position,1f));
                positionAvailable = true;
                break;
            }
        }
        if (!positionAvailable)
        {
            Debug.LogWarning($"No position with the name: ({position}) could be found. Make sure the names match.");
        }
    }

    private IEnumerator MoveToPosition(Transform to, float time)
    {
        moving = true;
        //references
        Transform from = transform;
        float t = 0f;

        //This while loop will smoothly transition the camera's position and rotation to the "to" values.
        while (t < time)
        {
            transform.position = Vector3.Lerp(from.position, to.position, t / time);
            transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        //After the smooth transition,
        //set the position and rotation as the exact "to" position/rotation.
        transform.position = to.position;
        transform.rotation = to.rotation;
        moving = false;
    }

    #endregion
}
