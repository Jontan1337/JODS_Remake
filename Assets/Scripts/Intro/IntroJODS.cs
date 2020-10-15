using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroJODS : MonoBehaviour
{
    public bool isPlaying;
    public GameObject[] objects;
    public GameObject player;
    public VideoPlayer vp;
    public Camera c;
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
        if (isPlaying)
        {
            Material m = player.GetComponent<Material>();
            foreach (GameObject o in objects)
            {
                o.SetActive(false);
            }
            player.SetActive(true);
            Destroy(test, 1f);
        }
        else if (!isPlaying)
        {
            foreach(GameObject o in objects)
            {
                o.SetActive(true);
            }
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            isPlaying = Input.anyKeyDown ? false : true;
            foreach (GameObject o in objects)
            {
                o.SetActive(false);
            }
            if (vp.isPlaying)
            {
                Color newC = c.backgroundColor;
                ColorUtility.TryParseHtmlString("#070707", out newC);
                c.backgroundColor = newC;
            }
        }
        else if (!isPlaying)
        {
            foreach (GameObject o in objects)
            {
                o.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
