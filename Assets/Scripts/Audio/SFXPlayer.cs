using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    [Range (0f,2f)] public float maxPitch = 1.05f;
    [Range(0f, 2f)] public float minPitch = 0.95f;
    public bool randomPitch;
    public bool looping;
    public bool playOnce;
    public bool playOnAwake;
    public bool playAmountOfTimes;
    public int amountOfPlays = 2;

    private AudioSource AS;

    private void Start()
    {
        if (!GetComponent<AudioSource>())
        {
            var a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
        }
        AS = GetComponent<AudioSource>();
        if (playOnAwake)
        {
            PlaySFX();
        }
    }
    void Update()
    {
        if (playAmountOfTimes && amountOfPlays == 0)
        {
            Destroy(gameObject);
        }
        if (looping)
        {
            if (!AS.isPlaying)
            {
                PlaySFX();
            }
        }
    }
    public void PlaySFX()
    {
        if (playAmountOfTimes)
        {
            amountOfPlays--;
        }
        if (randomPitch)
        {
            AS.pitch = Random.Range(minPitch, maxPitch);
        }
        if (clips.Length > 0)
        {
            AS.clip = clips[Random.Range(0, clips.Length)];
        }
        AS.PlayOneShot(AS.clip);
        if (playOnce)
        {
            Destroy(gameObject, AS.clip.length);
        }
    }
}
