using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class RPGMobileSetup : EditorWindow
{
    [MenuItem("Eldoria/Setup RPG Mobile Player")]
    public static void ShowWindow()
    {
        Setup();
    }

    public static void Setup()
    {
        // 1. Setup Grid and Turn Management
        GameObject gridObj = GameObject.Find("GridManager");
        if (gridObj == null) gridObj = new GameObject("GridManager");
        GridManager gm = gridObj.GetComponent<GridManager>();
        if (gm == null) gm = gridObj.AddComponent<GridManager>();
        gm.width = 16;
        gm.height = 16;
        gm.cellSize = 1f;
        gm.origin = new Vector2(-8, -8);

        GameObject turnObj = GameObject.Find("TurnManager");
        if (turnObj == null) turnObj = new GameObject("TurnManager");
        if (turnObj.GetComponent<TurnManager>() == null) turnObj.AddComponent<TurnManager>();

        // 2. Setup Player
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = new GameObject("Player");
        }
        player.transform.position = new Vector3(0.5f, 0.5f, 0); // Center of a cell near (0,0)

        // Sprite Renderer
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null) sr = player.AddComponent<SpriteRenderer>();
        sr.enabled = true;
        // Search for a default sprite to make it visible
        if (sr.sprite == null)
        {
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = Color.white;
        }

        // Rigidbody2D
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) rb = player.AddComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Isso congela a rotação em Z
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Collider2D
        if (player.GetComponent<Collider2D>() == null)
        {
            player.AddComponent<CircleCollider2D>();
        }

        // PlayerController
        if (player.GetComponent<PlayerController>() == null)
        {
            player.AddComponent<PlayerController>();
        }

        // 2. Setup UI Joystick
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem));
            es.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasObj = GameObject.Find("MobileCanvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("MobileCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas c = canvasObj.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler cs = canvasObj.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
        }

        if (GameObject.Find("Joystick") == null)
        {
            // Background
            GameObject joyBack = new GameObject("Joystick", typeof(RectTransform), typeof(Image), typeof(Joystick));
            joyBack.transform.SetParent(canvasObj.transform, false);
            
            RectTransform rtBack = joyBack.GetComponent<RectTransform>();
            rtBack.anchorMin = new Vector2(0, 0);
            rtBack.anchorMax = new Vector2(0, 0);
            rtBack.pivot = new Vector2(0.5f, 0.5f);
            rtBack.anchoredPosition = new Vector2(250, 250);
            rtBack.sizeDelta = new Vector2(300, 300);
            
            Image imgBack = joyBack.GetComponent<Image>();
            imgBack.color = new Color(1, 1, 1, 0.2f);

            // Handle
            GameObject joyHandle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            joyHandle.transform.SetParent(joyBack.transform, false);
            
            RectTransform rtHandle = joyHandle.GetComponent<RectTransform>();
            rtHandle.sizeDelta = new Vector2(120, 120);
            
            Image imgHandle = joyHandle.GetComponent<Image>();
            imgHandle.color = new Color(1, 1, 1, 0.5f);
        }

        Selection.activeGameObject = player;
        Debug.Log("SISTEMA VERIFICADO: Player configurado com Sprite, Rigidbody2D (Dynamic, Gravity 0, Freeze Z), Collider e Controller.");
    }
}
#endif
