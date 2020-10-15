using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    AudioSource AS;
    bool up;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        up = (Random.Range(0, 2) == 1);
        speed = Random.Range(0.5f, 1f);
        AS.pitch = Random.Range(0.1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!up)
        {
            AS.pitch -= Time.deltaTime * speed;
            if (AS.pitch <= 0.1f) { up = true; }
        }
        if (up)
        {
            AS.pitch += Time.deltaTime * speed;
            if (AS.pitch >= 2) { up = false; }
        }
    }
}
