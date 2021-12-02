using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private bool startOn = true;
    [Space]
    [SerializeField, Range(0f,10f)] private float onDuration = 1f;
    [SerializeField, Range(0f,10f)] private float offDuration = 1f;
    [SerializeField, Range(0f,10f)] private float lerpDuration = 1f;
    [Space]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.black;
    [Space]
    [SerializeField] private bool lerpEmission = true;
    [Space]
    [SerializeField] private bool randomOffDuration = false;
    [SerializeField, Range(0f,10f)] private float randomOffVariation = 2f;
    [SerializeField] private bool randomOnDuration = false;
    [SerializeField, Range(0f,10f)] private float randomOnVariation = 2f;
    [Space]
    [SerializeField] private int materialIndex = 1;
    [SerializeField,Tooltip("Uses the given material index if true. " +
        "If false, all materials will be affected")] private bool useMaterialIndex = true;

    private Material[] materials;
    private MeshRenderer meshRenderer;
    private bool isOn = false;

    private void Start()
    {
        //Get all the materials from the mesh renderer on this object
        meshRenderer = GetComponent<MeshRenderer>();

        //If we only want one material to be affected
        if (useMaterialIndex)
        {
            materials = new Material[1];

            //Get that one material based on the number given.
            materials[0] = meshRenderer.materials[materialIndex];
        }
        else
        {
            materials = meshRenderer.materials; //Apply all the materials to the array, essentially creating a new instance of the array.
        }

        ApplyColorChanges(startOn ? onColor : offColor);
        isOn = startOn;

        //Start the blinking
        StartCoroutine(IEBlinkingLight());
    }

    private IEnumerator IEBlinkingLight()
    {
        while (true)
        {
            if (lerpEmission)
            {
                //This is just a reference material used to get the start color.
                Material referenceMat = materials[0];

                //Reset the time to 0 to start a new lerp timer;
                float time = 0;

                //Get the start color from the reference material
                //This color will then be lerped to the new color
                Color startValue = referenceMat.GetColor("_EmissionColor");

                //This while loop will lerp the color
                while (time < lerpDuration)
                {
                    ApplyColorChanges(Color.Lerp(startValue, isOn ? offColor : onColor, time / lerpDuration));
                    time += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                //Instantly apply the new color
                ApplyColorChanges(isOn ? offColor : onColor);
            }

            //Switch this bool, so that the script knows what color to change to.
            isOn = !isOn;

            //Then wait for an amount of time, based on the variable values applied in the editor
            yield return new WaitForSeconds(isOn ? 
                (randomOnDuration ? Mathf.Clamp(Random.Range(onDuration - randomOnVariation, onDuration + randomOnVariation), 0, 10) : onDuration) : 
                (randomOffDuration ? Mathf.Clamp(Random.Range(offDuration - randomOffVariation, offDuration + randomOffVariation), 0, 10) : offDuration));
        }
    }

    //This function simply takes all the materials in the array and applies the new color to each.
    private void ApplyColorChanges(Color newColor)
    {
        foreach (Material mat in materials)
        {
            mat.SetColor("_EmissionColor", newColor);
        }
    }
}
