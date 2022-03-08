using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float minSize, maxSize;

    private RectTransform rect = null;

    public void Init()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetSize(float size)
    {
        size = Mathf.Clamp(4f*size, 4*minSize, 200f+maxSize);
        rect.sizeDelta = new Vector2(size, size);
    }
}
