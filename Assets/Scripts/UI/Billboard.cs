using System.Collections;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform billboardText = null;
    private Transform targetCamera = null;
    private void Start()
    {
        // Set target camera to the player running this script.
        StartCoroutine(GetCameraInScene());
        billboardText = GetComponent<TextMesh>().transform;
    }
    private IEnumerator GetCameraInScene()
    {
        yield return new WaitForSeconds(0.1f);
        targetCamera = FindObjectOfType<Camera>().transform;
    }

    private void LateUpdate()
    {
        // Text is turned 180 relative to camera.. TODO: Fix.
        if (targetCamera)
            billboardText.transform.LookAt(2 * billboardText.position - targetCamera.position);
    }
}
