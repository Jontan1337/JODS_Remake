using Mirror;
using UnityEngine;

public interface IPlaceable
{  

    Transform Owner { get; set; }
    [Server]
    void Svr_OnPlaced();
}
