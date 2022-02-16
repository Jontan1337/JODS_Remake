using UnityEngine;
using Mirror;
using System.Collections;

public class HandSway : NetworkBehaviour, IInitializable<SurvivorSetup>
{
    [SerializeField] private float swayVelocityX = 0.05f;
    [SerializeField] private float swayVelocityY = 0.08f;
    [SerializeField] private float swaySmoothing = 5f;
    [SerializeField] private Transform virtualHead = null;

    private SurvivorSetup survivorSetup;
    private Quaternion newRotation;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    Vector3 firstRotationValue = Vector3.zero;
    Vector3 secondRotationValue = Vector3.zero;
    Vector3 rotationDifference = Vector3.zero;

    public bool IsInitialized { get; private set; }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) return;
        if (virtualHead == null) return;

        newRotation = new Quaternion(
            virtualHead.localRotation.x + rotationDifference.x * swayVelocityX,
            0,
            virtualHead.localRotation.y + rotationDifference.y * swayVelocityY,
            1f
        );
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newRotation, Time.deltaTime * swaySmoothing);
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * swaySmoothing);
    }

    private IEnumerator IENextFrame()
    {
        while (true)
        {
            yield return null;
            if (virtualHead != null)
            {
                firstRotationValue = virtualHead.rotation.eulerAngles;
                yield return null;
                secondRotationValue = virtualHead.rotation.eulerAngles;
                rotationDifference = firstRotationValue - secondRotationValue;
            }
        }
    }

    public void Init(SurvivorSetup initializer)
    {
        survivorSetup = initializer;
        survivorSetup.onClientSpawnItem += GetVirtualHead;
    }

    private void GetVirtualHead(GameObject item)
    {
        item.TryGetComponent(out ItemName itemName);
        if (itemName.itemName == ItemNames.VirtualHead)
        {
            virtualHead = item.transform;
            if (hasAuthority)
                StartCoroutine(IENextFrame());
        }
    }
}
