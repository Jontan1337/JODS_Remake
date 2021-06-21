using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    public Color particleColor = Color.white;
    public Color ParticleColor
    {
        get => particleColor;
        private set => particleColor = value;
    }

    public void EmitSplash(Color color)
    {

    }
}
