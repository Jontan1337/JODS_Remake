using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoadName = null;

    public void OnButtonClick()
    {
        SceneManager.LoadScene(sceneToLoadName);
    }
}
