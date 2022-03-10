using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Option_AdvancedBool : MonoBehaviour
{
    private enum SettingType
    {
        Bloom, DOF, Vignette, MotionBlur
    }
    private Volume postProcessingVolume = null;
    private VolumeProfile volumeProfile = null;
    [SerializeField] private SettingType settingType = SettingType.Bloom;
    private Bloom bloom;
    private DepthOfField dof;
    private Vignette vignette;
    private MotionBlur motionBlur;

    private void Start()
    {
        postProcessingVolume = PostProcessingManager.Instance.GetVolume;
        if (!postProcessingVolume) 
        {
            Debug.LogError("No Post Processing Manager found, is the Global Post Processing Volume in the level?");
            return;
        }
        volumeProfile = postProcessingVolume.profile;
        volumeProfile.TryGet(out bloom);
        volumeProfile.TryGet(out dof);
        volumeProfile.TryGet(out vignette);
        volumeProfile.TryGet(out motionBlur);
    }
        
    public void ChangeSetting(bool enable)
    {
        switch (settingType)
        {
            case SettingType.Bloom:
                bloom.active = enable;
                break;
            case SettingType.DOF:
                dof.active = enable;
                break;
            case SettingType.Vignette:
                vignette.active = enable;
                break;
            case SettingType.MotionBlur:
                motionBlur.active = enable;
                break;
        }
    }
}
