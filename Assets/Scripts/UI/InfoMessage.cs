using System.Collections;
using TMPro;
using UnityEngine;

public class InfoMessage : MonoBehaviour
{
    [SerializeField] private GameObject textObject = null;
    [SerializeField] private Transform MessageList = null;
    [SerializeField] private float displayTime = 2f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void NewMessage(string messageText)
    {
        DisplayMessage(messageText);
    }

    private void DisplayMessage(string messageText)
    {
        GameObject newTextObject = Instantiate(textObject, MessageList);
        Destroy(newTextObject, displayTime);
        newTextObject.GetComponent<TextMeshProUGUI>().text += $"{messageText}";
    }
}
