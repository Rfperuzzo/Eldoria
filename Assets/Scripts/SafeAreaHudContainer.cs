using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaHudContainer : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;
    private float lastScaleFactor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        Apply();
    }

    private void Update()
    {
        Rect safeArea = Screen.safeArea;
        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        float scaleFactor = GetScaleFactor();

        if (safeArea != lastSafeArea || screenSize != lastScreenSize || !Mathf.Approximately(scaleFactor, lastScaleFactor))
        {
            Apply();
        }
    }

    private void Apply()
    {
        Rect safeArea = Screen.safeArea;
        float scaleFactor = GetScaleFactor();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = new Vector2(safeArea.xMin / scaleFactor, safeArea.yMin / scaleFactor);
        rectTransform.offsetMax = new Vector2(
            -(Screen.width - safeArea.xMax) / scaleFactor,
            -(Screen.height - safeArea.yMax) / scaleFactor
        );

        lastSafeArea = safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastScaleFactor = scaleFactor;
    }

    private float GetScaleFactor()
    {
        return canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
    }
}
