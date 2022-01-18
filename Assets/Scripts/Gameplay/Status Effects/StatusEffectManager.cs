using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class StatusEffectManager : NetworkBehaviour
{

    private Dictionary<StatusEffectSO, StatusEffect> currentEffects = new Dictionary<StatusEffectSO, StatusEffect>();
    private Dictionary<Sprite, Image> statusEffectVisuals = new Dictionary<Sprite, Image>();
    [Header("Current Status Effects")]
    public List<string> statusEffects = new List<string>();

    private Coroutine effectEnumerator;
    bool isActive;
    private bool playerObject = false;

    [Header("UI References")]
    [SerializeField] private Image[] imageReferenceList = null;

    private void Start()
    {
        if (!isServer) return;

        playerObject = GetComponent<ActiveSClass>();

        if (playerObject)
        {
            foreach(Image i in imageReferenceList)
            {
                i.enabled = false;
            }
        }

        isActive = false;
    }

    [Server]
    public void Svr_RemoveStatusEffect(StatusEffectSO effect)
    {
        if (currentEffects.ContainsKey(effect))
        {
            Debug.Log("(SVR) " + effect.name + " removed");
            currentEffects.Remove(effect);
        }
        if (statusEffects.Contains(effect.name))
        {
            statusEffects.Remove(effect.name);
        }
    }

    private void RemoveVisuals(StatusEffectSO effect)
    {
        Sprite effectVisual = effect.uIImage;

        //If the effect has a visual element
        if (effectVisual && imageReferenceList.Length > 0)
        {
            GameObject imgRef = statusEffectVisuals[effectVisual].gameObject;

            Rpc_EnableVisual(GetComponent<NetworkIdentity>().connectionToClient, imgRef);
        }
    }

    private IEnumerator StatusEffectEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            //Go through each of the current active effects, and call their Tick method
            foreach (var effect in currentEffects.Values.ToList())
            {
                //Call Tick, which does an effect over time and reduces the duration of the effect
                effect.Tick();

                //If the duration of this effect has reached 0, then stop the effect and remove it.
                if (effect.isFinished)
                {
                    //Remove the effect from the list of current active effects
                    currentEffects.Remove(effect.effect);
                    statusEffects.Remove(effect.effect.name);

                    RemoveVisuals(effect.effect);
                }
            }
            if (currentEffects.Count == 0)
            {
                isActive = false;
                StopCoroutine(effectEnumerator);
            }
        }
    }

    [Server]
    public void Svr_ApplyStatusEffect(StatusEffect newEffect, int? amount = null)
    {
        //If the status effect is already in the list, then activate the effect
        if (currentEffects.ContainsKey(newEffect.effect))
        {
            //If this effect can stack in any way, it will stack when activated again.
            currentEffects[newEffect.effect].Activate(amount);
        }
        else
        {
            Debug.Log("(SVR) New Status Effect : " + newEffect.effect.name);
            statusEffects.Add(newEffect.effect.name);

            //Add the effect to the dictionary
            currentEffects.Add(newEffect.effect, newEffect);

            //Activate the effect
            newEffect.Activate(amount);

            //If the coroutine is not currently running, then activate it
            //The coroutine will stop itself when there are no more active effects
            if (!isActive)
            {
                effectEnumerator = StartCoroutine(StatusEffectEnumerator());
                isActive = true;
            }

            Sprite effectVisual = newEffect.effect.uIImage;

            //If the effect has a visual element
            if (effectVisual && imageReferenceList.Length > 0)
            {
                foreach(Image img in imageReferenceList)
                {
                    if (!statusEffectVisuals.ContainsValue(img))
                    {
                        img.sprite = effectVisual;
                        statusEffectVisuals.Add(effectVisual, img);
                        break;
                    }
                }

                Rpc_EnableVisual(GetComponent<NetworkIdentity>().connectionToClient, statusEffectVisuals[effectVisual].gameObject);
            }
        }
    }

    [TargetRpc]
    private void Rpc_EnableVisual(NetworkConnection conn, GameObject imgRef)
    {
        Image newImg = imgRef.GetComponent<Image>();

        bool enable = newImg.sprite != null;

        foreach (Image img in imageReferenceList)
        {
            //Assign the sprite to an unused image
            if (enable && !img.enabled)
            {
                img.sprite = newImg.sprite;
                break;
            }
            else if (!enable && img.enabled && img.sprite == newImg.sprite)
            {
                img.sprite = null;
            }
            img.enabled = enable;
        }     
    }
}
