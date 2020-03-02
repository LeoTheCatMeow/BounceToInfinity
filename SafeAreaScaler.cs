using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaScaler : MonoBehaviour
{
    RectTransform Panel;

    void Start()
    {
        Panel = GetComponent<RectTransform>();
        Rect safeArea = GetSafeArea();
        ApplySafeArea(safeArea);
    }

    private Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    private void ApplySafeArea(Rect r)
    {
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
    }
}
