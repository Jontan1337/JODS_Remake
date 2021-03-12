using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurret : MonoBehaviour, IEquippable, IBindable
{
    private PlaceItem place;
    private Coroutine placeHolderActive;

    private void Start()
    {
        place = GetComponent<PlaceItem>();
        placeHolderActive = StartCoroutine(place.PlaceHolderActive());
    }

    public string Name => throw new System.NotImplementedException();

    public GameObject Item => throw new System.NotImplementedException();

    public EquipmentType EquipmentType => throw new System.NotImplementedException();

    public void Bind()
    {
        throw new System.NotImplementedException();
    }

    public void Svr_GiveAuthority(NetworkConnection conn)
    {
        throw new System.NotImplementedException();
    }

    public void Svr_RemoveAuthority()
    {
        throw new System.NotImplementedException();
    }

    public void UnBind()
    {
        throw new System.NotImplementedException();
    }


}
