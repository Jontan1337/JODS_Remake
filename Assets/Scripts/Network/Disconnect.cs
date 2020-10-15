using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Disconnect : NetworkBehaviour
{
    public void DisconnectPlayer()
    {
        // If host/server then stop server.
        if (isServer)
        {
            print("Shutdown");
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }

        //if (isServer)
        //{
        //    NetworkManager.singleton.StopHost();
        //}
        //else
        //{
        //    NetworkManager.singleton.StopClient();
        //}

        //SceneManager.LoadScene(0);
    }
}
