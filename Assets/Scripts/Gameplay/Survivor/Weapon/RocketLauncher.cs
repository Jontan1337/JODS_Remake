using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class RocketLauncher : NetworkBehaviour, IBindable, IEquippable
{
    private GameObject rocketSpawnPos;
    public GameObject rocket;
    private int rocketSpeed = 5000;
    private AuthorityController authController = null;
    public string Name => throw new System.NotImplementedException();
    public string ObjectName => gameObject.name;
    public GameObject Item => gameObject;
    public EquipmentType EquipmentType => EquipmentType.None;

    private void Start()
    {
        authController = GetComponent<AuthorityController>();
        rocketSpawnPos = new GameObject("rocketSpawnPos");
        rocketSpawnPos.transform.SetParent(transform);
        rocketSpawnPos.transform.position = new Vector3(0.5f, 2, 0.5f);
    }

    public void Bind()
	{
        JODSInput.Controls.Survivor.LMB.performed += OnShoot;
    }

    public void UnBind()
    {
        JODSInput.Controls.Survivor.LMB.performed -= OnShoot;
    }

    void OnShoot(InputAction.CallbackContext context)
	{
        CmdRocketLaunch();
	}

    [Command]
    void CmdRocketLaunch()
    {
        GameObject currentRocket = Instantiate(rocket, new Vector3(rocketSpawnPos.transform.position.x, rocketSpawnPos.transform.position.y, rocketSpawnPos.transform.position.z), transform.GetComponent<LookController>().playerCamera.transform.rotation);
        NetworkServer.Spawn(currentRocket);
        RpcRocketLaunch(currentRocket);
    }

    [ClientRpc]
    void RpcRocketLaunch(GameObject currentRocket)
    {
        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        rb.AddForce(transform.GetComponent<LookController>().playerCamera.transform.forward * rocketSpeed);
    }
}
