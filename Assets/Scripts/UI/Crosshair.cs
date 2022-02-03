using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private RectTransform rect = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetSize(float size)
    {
        size = Mathf.Clamp(size+30, 50f, 200f);
        rect.sizeDelta = new Vector2(size, size);
    }
}
