using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

[System.Serializable]
public class CameraPosition
{
    public string name;
    public Transform position;
}
public class MenuCamera : MonoBehaviour
{
    [SerializeField] private List<CameraPosition> cameraPositions = new List<CameraPosition>();

    [Header("Post Processing")]
    private PostProcessingProfile PP;
    void Start()
    {
        GetComponent<PostProcessingBehaviour>().enabled = true;
        PP = GetComponent<PostProcessingBehaviour>().profile;
        ColorGradingModel.Settings col = PP.colorGrading.settings;
        col.basic.hueShift = 0;
        PP.colorGrading.settings = col;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ColorGradingModel.Settings col = PP.colorGrading.settings;
            col.basic.hueShift = Random.Range(-180, 180);
            PP.colorGrading.settings = col;
        }
    }


    #region Camera Position Changes

    public void ChangePosition(string position)
    {
        bool positionAvailable = false;
        foreach(var cameraPosition in cameraPositions)
        {
            if (cameraPosition.name == position)
            {
                //TODO : Start coroutine which smoothly moves the camera to the new position.
                positionAvailable = true;
                break;
            }
        }
        if (!positionAvailable)
        {
            Debug.LogWarning($"No position with the name: ({position}) could be found. Make sure the names match.");
        }
    }

    #endregion
}
