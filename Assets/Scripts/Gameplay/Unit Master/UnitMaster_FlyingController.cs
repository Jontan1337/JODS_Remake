﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMaster_FlyingController : MonoBehaviour
{
    public Camera cam = null;
    private CharacterController cc = null;
    public UnitMaster master;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float gravity = 20f;
    [Space]
    [SerializeField] private float minRotY = -60f;
    [SerializeField] private float maxRotY = 60f;
    [Space]
    [SerializeField] [Range(0.1f, 2f)] private float sensitivity = 1f;
    [Space]
    [SerializeField] private Transform cursorMarker = null;
    [SerializeField] private bool showMarker = false;    

    //Movement variables
    private float m_horizontal; // These variables are used to move the player. 
    private float m_vertical; // They store the player's input values.

    //Camera variables
    private Vector2 rotation; // This variables are used to rotate the player. 

    private void Start()
    {
        cc = GetComponent<CharacterController>();

        //Input stuff

        // Movement Input
        JODSInput.Controls.Master.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());

        // Camera Input
        JODSInput.Controls.Master.Camera.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
    }

    private void Move(Vector2 moveValues)
    {
        m_horizontal = moveValues.x;
        m_vertical = moveValues.y;
    }

    private void Rotate(Vector2 rotateValues)
    {

        rotation.x = rotateValues.x * (sensitivity * 100);
        rotation.y = rotateValues.y * (sensitivity * 100);
    }
    
    private void LateUpdate()
    {
        //Rotation
        //This does not clamp the rotation
        cam.transform.Rotate(Vector3.right * -rotation.y * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.up * rotation.x * Time.deltaTime, Space.World);

    }
    private void Update()
    {
        //Movement
        if (cc.isGrounded)
        {
            moveDirection = transform.TransformDirection(new Vector3(m_horizontal, 0.00f, m_vertical)) * movementSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        cc.Move(moveDirection * Time.deltaTime);
        
        //Raycast Marker
        if (showMarker) //If the master has selected a unit, this bool will be true
        {
            //Call the master's Raycast method, which returns a raycast's hit and a bool if it did hit.
            //This output changes based on what camera controller the master is currently using.
            master.Raycast(out bool didHit, out RaycastHit hit);

            cursorMarker.gameObject.SetActive(didHit);

            if (didHit)
            {
                //Change the position of the marker, to where the raycast hit.
                cursorMarker.position = hit.point;
            }
        }
    }

    #region Marker

    public void ShowMarker(bool active)
    {
        showMarker = active;
        cursorMarker.gameObject.SetActive(active);
    }

    public void ChangeMarker(Mesh markerMesh, Color markerColor)
    {
        //Get the material, and change the colours
        Material mat = cursorMarker.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        mat.color = markerColor;
        mat.SetColor("_EmissionColor", markerColor);

        //Change the mesh
        cursorMarker.gameObject.GetComponent<MeshFilter>().mesh = markerMesh;
    }

    #endregion
}
