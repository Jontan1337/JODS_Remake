using UnityEngine;

public class WeaponHandIK : MonoBehaviour
{
    [SerializeField] private Transform rightHand = null;
    [SerializeField] private Transform leftHand = null;

    [SerializeField] private Transform rightHandTargetPos = null;
    [SerializeField] private Transform leftHandTargetPos = null;

    [SerializeField] private Animator animator = null;
    [SerializeField] private Shoot shoot = null;

    public void SetWeaponPoints(Transform rightHandTarget, Transform leftHandTarget)
    {
        print(animator.GetBoneTransform(HumanBodyBones.RightHand));
        print(animator.GetBoneTransform(HumanBodyBones.LeftHand));
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHandTargetPos = rightHandTarget;
        leftHandTargetPos = leftHandTarget;
    }

    private void LateUpdate()
    {
        if (shoot.hasWeapon)
        {
            rightHand.Translate(rightHandTargetPos.localPosition);
            leftHand.Translate(leftHandTargetPos.localPosition);
        }
    }
}
