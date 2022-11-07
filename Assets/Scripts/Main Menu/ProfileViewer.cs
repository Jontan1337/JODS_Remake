using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileViewer : MonoBehaviour
{
    [SerializeField] private Text usernameText = null;
    [Space]
    [SerializeField] private ProfileStatRow_Survivor[] survivorRows = null;
    [Space]
    [SerializeField] private ProfileStatRow_Master masterRow = null;

    // Start is called before the first frame update
    void Start()
    {
        usernameText.text = WebsocketManager.Instance.playerProfile.data[0].username;
        LoadUserStats();
    }

    private void LoadUserStats()
    {

    }
}
