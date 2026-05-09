using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaJoystickAnchor : MonoBehaviour
{
    [SerializeField] private Vector2 margin = new Vector2(96f, 96f);

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
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(
            safeArea.xMin / scaleFactor + margin.x + rectTransform.sizeDelta.x * rectTransform.pivot.x,
            safeArea.yMin / scaleFactor + margin.y + rectTransform.sizeDelta.y * rectTransform.pivot.y
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
