using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;
    [SerializeField, Range(0f, 2f)] 
    private float maxPitch = 1.05f;
    [SerializeField, Range(0f, 2f)]
    private float minPitch = 0.95f;
    [SerializeField]
    private bool randomPitch;
    [SerializeField]
    private bool looping;
    [SerializeField]
    private bool playOnce;
    [SerializeField]
    private bool playOnAwake;
    [SerializeField]
    private bool playAmountOfTimes;
    [SerializeField]
    private int amountOfPlays = 2;

    private AudioSource AS;

    public bool RandomPitch { get => randomPitch; }
    public bool Looping { get => looping; }
    public bool PlayOnce { get => playOnce; }
    public bool PlayOnAwake { get => playOnAwake; }
    public bool PlayAmountOfTimes { get => playAmountOfTimes; }
    public int AmountOfPlays { get => amountOfPlays; }

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
