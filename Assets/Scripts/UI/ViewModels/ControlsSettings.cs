using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ViewModels
{
    public class ControlsSettings : MonoBehaviour
    {
        [SerializeField] private Slider mouseSensitivity = null;
        [SerializeField] private Slider mouseAcceleration = null;
        [SerializeField] private Slider mouseMaxAcceleration = null;
        [SerializeField] private Slider mouseEasingSpeed = null;

        private void Awake()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "Lobby")
            {

            }
            Init();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Init()
        {
            if (GameSettings.Instance)
            {
                mouseSensitivity.onValueChanged.AddListener(delegate(float e) { GameSettings.Instance.SetMouseSensitivity(e); });
                mouseAcceleration.onValueChanged.AddListener(delegate(float e) { GameSettings.Instance.SetMouseAcceleration(e); });
                mouseMaxAcceleration.onValueChanged.AddListener(delegate(float e) { GameSettings.Instance.SetMouseMaxAcceleration(e); });
                mouseEasingSpeed.onValueChanged.AddListener(delegate(float e) { GameSettings.Instance.SetMouseEasingSpeed(e); });
            }
        }
        private void Clear()
        {
            if (GameSettings.Instance)
            {
                mouseSensitivity.onValueChanged.RemoveListener(delegate(float e) { GameSettings.Instance.SetMouseSensitivity(e); });
                mouseAcceleration.onValueChanged.RemoveListener(delegate(float e) { GameSettings.Instance.SetMouseAcceleration(e); });
                mouseMaxAcceleration.onValueChanged.RemoveListener(delegate(float e) { GameSettings.Instance.SetMouseMaxAcceleration(e); });
                mouseEasingSpeed.onValueChanged.RemoveListener(delegate(float e) { GameSettings.Instance.SetMouseEasingSpeed(e); });
            }
        }
    }
}