using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    public Button JoinButton = null;
    public Text Name = null;
    public Text PlayerCount = null;
    public string Address = null;

    public void Setup(string name, int playerCount, string address)
    {
        Name.text = name;
        PlayerCount.text = playerCount.ToString();
        Address = address;
    }
}
