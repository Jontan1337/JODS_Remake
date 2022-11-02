using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Text userNameText = null;
    [SerializeField] private Text messageText = null;

    public void SetMessage(string userNameString, string messageString)
    {
        userNameText.text = userNameString + ": ";
        messageText.text = messageString;

        fadeCo = StartCoroutine(FadeText());
    }

    private void Awake()
    {
        ChatManager.OnOpenChat += OnOpenChatReceived;
        ChatManager.OnCloseChat += OnCloseChatReceived;
    }

    public void OnOpenChatReceived()
    {
        if (fading)
        {
            StopCoroutine(fadeCo);
            fading = false;

            Color mColor = userNameText.color;  //reference to the color component of the text
            Color uColor = messageText.color;   //reference to the color component of the text

            mColor.a = 1; // set the colour's alpha value 
            uColor.a = 1; // set the colour's alpha value 

            userNameText.color = uColor;// apply new colour to the text's colour
            messageText.color = mColor; // apply new colour to the text's colour
        }
    }

    public void OnCloseChatReceived()
    {
        if (!fading)
        {
            fadeCo = StartCoroutine(FadeText());
        }
    }

    bool fading = false;
    Coroutine fadeCo;
    private IEnumerator FadeText()
    {
        fading = true;

        //Wait 5 seconds
        yield return new WaitForSeconds(5f);

        //Then we start the fading process

        float timeToFade = 5f;  //float variable for how long the fade takes
        Color mColor = userNameText.color;  //reference to the color component of the text
        Color uColor = messageText.color;   //reference to the color component of the text

        while (timeToFade > 0)
        {
            timeToFade -= Time.deltaTime; //the float counts down in real time

            mColor.a = timeToFade / 5f; // set the colour's alpha value 
            uColor.a = timeToFade / 5f; // set the colour's alpha value 

            userNameText.color = uColor;// apply new colour to the text's colour
            messageText.color = mColor; // apply new colour to the text's colour

            yield return null;
        }

        fading = false;
    }
}
