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

    private void ShowUserStats(WSResponse<UserStats> response)
    {
        //Extract the data from the JSON response and insert them into rows
        //For each survivor in the Classes array
        for (int i = 0; i < response.data[0].classes.Length; i++)
        {
            survivorRows[i].classText.text = ((SurvivorType)response.data[0].classes[i].classType).ToString();
            survivorRows[i].playerHighscoreText.text = response.data[0].classes[i].highscore.ToString();
            survivorRows[i].killsText.text = response.data[0].classes[i].kills.ToString();
            survivorRows[i].specialUsed.text = response.data[0].classes[i].specialUsed.ToString();
            survivorRows[i].deathsText.text = response.data[0].classes[i].deaths.ToString();
        }

        //Master data
        masterRow.classText.text = "Zombie Master"; //This is sort of temporary, this should also be a variable like the above classType.
        masterRow.playerHighscoreText.text = response.data[0].masterStats.highscore.ToString();
        masterRow.unitsPlacedText.text = response.data[0].masterStats.unitsPlaced.ToString();
        masterRow.upgradesText.text = response.data[0].masterStats.unitUpgrades.ToString();
    }
}
