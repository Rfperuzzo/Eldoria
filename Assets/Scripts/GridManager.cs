using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int width = 16;
    public int height = 16;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;

    private Cell[,] grid;

    public class Cell
    {
        public int x;
        public int y;
        public Vector3 worldPosition;

        public Cell(int x, int y, Vector3 worldPosition)
        {
            this.x = x;
            this.y = y;
            this.worldPosition = worldPosition;
        }
    }

    [Header("Visual Settings")]
    [SerializeField] private Color colorA = new Color(0.9f, 0.9f, 0.9f, 1f);
    [SerializeField] private Color colorB = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Sprite cellSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGrid();
            // In play mode, we ensure the board is visible
            GameObject board = GameObject.Find("Board");
            if (board == null) CreateVisualBoard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Generate Visual Board")]
    public void CreateVisualBoard()
    {
        GameObject boardObj = GameObject.Find("Board");
        if (boardObj != null) 
        {
            if (Application.isPlaying) Destroy(boardObj);
            else DestroyImmediate(boardObj);
        }

        boardObj = new GameObject("Board");
        boardObj.transform.position = Vector3.zero;

        // Ensure we have a sprite
        if (cellSprite == null)
        {
            // Create a simple white 1x1 sprite if none is assigned
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            cellSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cellObj = new GameObject($"Cell_{x}_{y}");
                cellObj.transform.SetParent(boardObj.transform);
                
                // Position at the center of the cell
                Vector3 worldPos = GetWorldPosition(x, y) + new Vector3(cellSize / 2f, cellSize / 2f, 0);
                cellObj.transform.position = worldPos;
                cellObj.transform.localScale = new Vector3(cellSize, cellSize, 1);

                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite = cellSprite;
                sr.color = (x + y) % 2 == 0 ? colorA : colorB;
                sr.sortingOrder = -10; // Behind player (0)
            }
        }
    }

    private void InitializeGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(x, y, GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2, 0));
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (IsWithinBounds(x, y)) return grid[x, y];
        return null;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0) + (Vector3)origin;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - (Vector3)origin;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Cell GetCellFromWorld(Vector3 worldPosition)
    {
        Vector2Int gridPos = GetGridPosition(worldPosition);
        return GetCell(gridPos.x, gridPos.y);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private Vector2Int selectedGridPos = new Vector2Int(-1, -1);
    public void SelectCell(Vector2Int pos)
    {
        if (IsWithinBounds(pos.x, pos.y))
        {
            selectedGridPos = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int x = 0; x <= width; x++)
        {
            Gizmos.DrawLine(GetWorldPosition(x, 0), GetWorldPosition(x, height));
        }
        for (int y = 0; y <= height; y++)
        {
            Gizmos.DrawLine(GetWorldPosition(0, y), GetWorldPosition(width, y));
        }

        if (IsWithinBounds(selectedGridPos.x, selectedGridPos.y))
        {
            Gizmos.color = Color.yellow;
            Vector3 center = GetWorldPosition(selectedGridPos.x, selectedGridPos.y) + new Vector3(cellSize / 2, cellSize / 2, 0);
            Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0.1f));
        }
    }
}
