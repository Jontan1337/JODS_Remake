using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spectator : NetworkBehaviour
{
    float rotY = 0;
    float rotX = 0;
    void Update()
    {
        if (hasAuthority == false)
        {
            return;
        }
        var hor = Input.GetAxis("Horizontal") * 5;
        var ver = Input.GetAxis("Vertical") * 5;
        rotY += Input.GetAxis("Mouse Y") * 2;
        rotX += Input.GetAxis("Mouse X") * 2;

        transform.position += transform.forward * ver * Time.deltaTime;
        transform.position += transform.right * hor * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, -75f, 75f);
        //camera.transform.Rotate(-rotY, 0f, 0f);

        transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
    }
}
