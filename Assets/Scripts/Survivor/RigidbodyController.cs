using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float acceleration = 1f;

    [Header("References")]
    [SerializeField]
    private Rigidbody rigidbody = null;
    [SerializeField]
    private InputActionAsset controls = null;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        //controls.actionMaps[0].FindActionMap("Survivor").
    }

    private void Update()
    {

        Vector3 asd = new Vector3(horizontalMove, verticalMove);

        //rigidbody.AddForce();
    }

    private void Move(Vector3 moveValues)
    {

    }
}
