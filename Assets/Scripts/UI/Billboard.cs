using System.Collections;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform billboardText = null;
    private Transform targetCamera = null;
    private void Start()
    {
        // Set target camera to the player running this script.
        billboardText = GetComponent<TextMesh>().transform;
        // TODO: Somehow find the player camera in the scene.
    }

    private void LateUpdate()
    {
        // Text is turned 180 relative to camera.. TODO: Fix.
        billboardText.transform.LookAt(2 * billboardText.position - targetCamera.position);
    }
}
