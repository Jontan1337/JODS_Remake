using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour
{
    [SyncVar] public bool isOpen;
    [SyncVar] public bool invert;
    [SyncVar] public bool locked;
    public bool pushDoor;
    public bool onlyPushWhenClosed;
    public float openRot = 90f;
    public float timeToOpen = 3f;
    private float openTime = 0f;
    public float closeAfterTime = 0f; //0 is never
    public float selfCloseTime = 50f;
    private float closeTime = 0f;
    private float closedRot = 0f;
    public GameObject door1;
    public GameObject door2;
    private Vector3 forw;

    [Header("Audio")]
    public AudioClip[] openClips;
    public AudioClip[] closeClips;
    private AudioSource AS;

    private bool isLerping;
    private float timeLerped;

    Quaternion open;
    Quaternion iopen;
    Quaternion closed;

    void Start()
    {
        closedRot = transform.eulerAngles.y;
        open = Quaternion.Euler(0, openRot + closedRot, 0);
        iopen = Quaternion.Euler(0, -openRot + closedRot, 0);
        closed = Quaternion.Euler(0, closedRot, 0);
        forw = transform.forward;
        AS = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (isLerping)
        {
            float timeFromStart = Time.time - timeLerped;
            float timeDone = timeFromStart / openTime;

            if (isOpen)
            {
                if (door1) { door1.transform.rotation = Quaternion.Lerp(door1.transform.rotation, invert ? open : iopen, timeDone); }
                if (door2) { door2.transform.rotation = Quaternion.Lerp(door2.transform.rotation, invert ? iopen : open, timeDone); }
            }
            else
            {
                if (door1) { door1.transform.rotation = Quaternion.Lerp(door1.transform.rotation, closed, timeDone); }
                if (door2) { door2.transform.rotation = Quaternion.Lerp(door2.transform.rotation, closed, timeDone); }
            }
            if (timeDone >= 1.0f)
            {
                isLerping = false;
            }
        }
        if (closeTime > 0)
        {
            closeTime -= Time.fixedDeltaTime;
        }
        else
        {
            if (closeAfterTime != 0f)
            {
                if (isOpen)
                {
                    openTime = selfCloseTime * 10;
                    isOpen = false;
                    isLerping = true;
                    timeLerped = Time.time;
                }
            }
        }
    }

    public void Open()
    {
        if (!locked)
        {
            openTime = timeToOpen;
            isOpen = !isOpen;
            isLerping = true;
            timeLerped = Time.time;
            if (closeAfterTime != 0f)
            {
                closeTime = closeAfterTime;
            }
        }
    }

    [ClientRpc]
    public void RpcOpen(float x, float z)
    {
        if (!locked)
        {
            if (forw.x != 0)
            {
                if (forw.x > 0)
                {
                    invert = (x > 0);
                }
                else if (forw.x < 0)
                {
                    invert = (x < 0);
                }
            }
            else
            {
                if (forw.z > 0)
                {
                    invert = (z > 0);
                }
                else if (forw.z < 0)
                {
                    invert = (z < 0);
                }
            }

            AS.clip = openClips[Random.Range(0, openClips.Length)];
            AS.Play();

            openTime = timeToOpen;
            isOpen = !isOpen;
            isLerping = true;
            timeLerped = Time.time;
            if (closeAfterTime != 0f)
            {
                closeTime = closeAfterTime;
            }
        }
        //Debug.Log("Player forward x : " + x.ToString("N1") + " Door forward x : " + forw.x.ToString("N1") + " | Player forward z : " + z.ToString("N1") + " Door forward z : " + forw.z.ToString("N1"));
    }

    [ClientRpc]
    public void RpcPush(float x, float z)
    {
        if (!locked)
        {
            if (!isOpen)
            {
                bool canPush = true;
                if (onlyPushWhenClosed)
                {
                    // ------------- TO DO:  PLAYER CAN ONLY PUSH DOOR IF IT HAS CLOSED COMPLETELY. NEED TO BE ABLE TO PUSH WHEN THE DOOR IS HALF CLOSED

                    if (door1.transform.rotation != closed)
                    {
                        canPush = false;
                    }
                    if (door2.transform.rotation != closed)
                    {
                        canPush = false;
                    }
                }
                if (canPush)
                {
                    if (forw.x != 0)
                    {
                        if (forw.x > 0)
                        {
                            invert = (x > 0);
                        }
                        else if (forw.x < 0)
                        {
                            invert = (x < 0);
                        }
                    }
                    else
                    {
                        if (forw.z > 0)
                        {
                            invert = (z > 0);
                        }
                        else if (forw.z < 0)
                        {
                            invert = (z < 0);
                        }
                    }

                    AS.clip = openClips[Random.Range(0, openClips.Length)];
                    AS.Play();

                    openTime = timeToOpen;
                    isOpen = !isOpen;
                    isLerping = true;
                    timeLerped = Time.time;
                    if (closeAfterTime != 0f)
                    {
                        closeTime = closeAfterTime;
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void RpcLockDoor(bool open)
    {
        if (!locked)
        {
            if (open && !isOpen)
            {
                isOpen = !isOpen;
            }
            else if (!open && isOpen)
            {
                isOpen = !isOpen;
            }
            openTime = timeToOpen;
            locked = true;
            isLerping = true;
            timeLerped = Time.time;
            Invoke("RpcUnlockDoor", 10f);
        }
    }

    [ClientRpc]
    public void RpcUnlockDoor()
    {
        locked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zombie")
        {
            if (!isOpen && pushDoor)
            {
                Open();
            }
        }
    }
}
