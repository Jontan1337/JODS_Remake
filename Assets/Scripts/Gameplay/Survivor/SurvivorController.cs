using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorController : MonoBehaviour
{
    Rigidbody rb;

    float minRotY = -75f;
    float maxRotY = 75F;
    float lookSpeed = 3.0f;
    public float rotY;
    public float rotX;
    public float speed;
    public new Camera camera;
    public Vector3 moveDirection = Vector3.zero;
    private float horizontal;
    private float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


        //Camera controls
        rotY += Input.GetAxis("Mouse Y") * lookSpeed;
        rotX += Input.GetAxis("Mouse X") * lookSpeed;
        rotY = Mathf.Clamp(rotY, minRotY, maxRotY);
        camera.transform.rotation = Quaternion.Euler(-rotY, rotX, 0f);
        transform.rotation = Quaternion.Euler(0f, rotX, 0f);

        //Movement
        moveDirection = new Vector3(horizontal, 0.0f, vertical);
        moveDirection = transform.TransformDirection(moveDirection);

        //Fixes the issue of moving faster if going forwards and sideways.
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }
        moveDirection = moveDirection * speed;
        rb.MovePosition(transform.position + moveDirection);
    }
}
