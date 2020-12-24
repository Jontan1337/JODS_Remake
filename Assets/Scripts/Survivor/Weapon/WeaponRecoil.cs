using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public float recoilAmountV = 0f;
    [HideInInspector] public float recoilAmountR = 0f;
    private Transform _transform = null;
    void Start()
    {
        _transform = this.transform;
    }

    public void Recoil()
    {
        _transform.localPosition += new Vector3(0f, 0f, -0.1f * recoilAmountV);
        _transform.localRotation = new Quaternion(_transform.localRotation.x + -0.1f * recoilAmountR, _transform.localRotation.y, _transform.localRotation.z, 1f);
    }
}
