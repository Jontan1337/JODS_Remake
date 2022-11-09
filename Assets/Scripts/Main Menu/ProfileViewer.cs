using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ProfileViewer : MonoBehaviour
{
    [SerializeField] private Text usernameText = null;
    [Space]
    [SerializeField] private ProfileStatRow_Survivor[] survivorRows = null;
    [Space]
    [SerializeField] private ProfileStatRow_Master masterRow = null;

    private void Awake()
    {
        WebsocketManager.onGetStats += ShowUserStats;
    }

    // Start is called before the first frame update
    void Start()
    {
        usernameText.text = WebsocketManager.Instance.playerProfile.data[0].username;

    }
    public void LoadPanel()
    {
        RequestUserStats();
    }
    private async Task RequestUserStats()
    {
        var getRequest = new WSRequests()
        {
            type = RequestType.getStats.ToString(),
        };

        await WebsocketManager.Instance.Send(getRequest);
    }

    private void ShowUserStats(WSResponse<WSRequests> response)
    {
        Debug.LogWarning(response.data[0]);
    }
}
