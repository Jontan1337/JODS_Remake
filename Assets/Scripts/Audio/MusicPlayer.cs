using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] music;
    private AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.clip = music[Random.Range(0, music.Length)];
        AS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AS.isPlaying)
        {
            AS.clip = music[Random.Range(0, music.Length)];
            AS.Play();
        }   
    }
}
