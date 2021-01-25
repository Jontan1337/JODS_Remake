using UnityEngine;

public class JODSInput : MonoBehaviour
{
    public static Controls Controls { get; private set; }

    private void Awake()
    {
        Controls = new Controls();
    }

    private void OnEnable()
    {
        Controls.Enable();
        print("Enable");
    }

    private void OnDisable()
    {
        Controls.Disable();
        print("Disable");
    }
}
