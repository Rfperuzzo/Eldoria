using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Mode")]
    [SerializeField] private bool useGridMovement = false;

    [Header("Free Movement Settings")]
    [SerializeField] private float freeMoveSpeed = 5f;
    [SerializeField] private Vector2 movementBoundsMin = new Vector2(-7.5f, -6.0f);
    [SerializeField] private Vector2 movementBoundsMax = new Vector2(7.5f, 7.5f);

    [Header("Grid Movement Settings")]
    [SerializeField] private float gridMoveTime = 0.2f; // Time to move to the next cell
    
    private Rigidbody2D rb;
    private Vector2Int gridPosition;
    private bool isMovingInGrid = false;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateRigidbodyState();
    }

    private void UpdateRigidbodyState()
    {
        if (rb == null) return;

        if (useGridMovement)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        rb.simulated = true;
    }

    private void Start()
    {
        if (useGridMovement && GridManager.Instance != null)
        {
            SnapToGrid();
        }
    }

    private void SnapToGrid()
    {
        gridPosition = GridManager.Instance.GetGridPosition(transform.position);
        transform.position = GridManager.Instance.GetWorldPosition(gridPosition.x, gridPosition.y) + new Vector3(GridManager.Instance.cellSize / 2, GridManager.Instance.cellSize / 2, 0);
    }

    private void Update()
    {
        if (useGridMovement)
        {
            HandleGridMovement();
        }
        else
        {
            HandleFreeMovement();
        }
    }

    private void HandleFreeMovement()
    {
        // 1. Get Joystick Input (Mobile)
        Vector2 joystickInput = Joystick.Input;

        // 2. Get Keyboard Input (WASD/Arrows)
        Vector2 keyboardInput = Vector2.zero;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float x = 0;
            float y = 0;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1;
            keyboardInput = new Vector2(x, y);
        }

        Vector2 combined = joystickInput + keyboardInput;
        moveInput = combined.sqrMagnitude > 1 ? combined.normalized : combined;
    }

    private void FixedUpdate()
    {
        if (!useGridMovement && rb != null)
        {
            Vector2 velocity = moveInput * freeMoveSpeed;
            Vector2 position = ClampToMovementBounds(rb.position);

            if (rb.position != position)
            {
                rb.position = position;
            }

            Vector2 targetPosition = ClampToMovementBounds(position + velocity * Time.fixedDeltaTime);
            velocity = (targetPosition - position) / Time.fixedDeltaTime;

            rb.linearVelocity = velocity;
        }
    }

    private Vector2 ClampToMovementBounds(Vector2 position)
    {
        return new Vector2(
            Mathf.Clamp(position.x, movementBoundsMin.x, movementBoundsMax.x),
            Mathf.Clamp(position.y, movementBoundsMin.y, movementBoundsMax.y)
        );
    }

    private void HandleGridMovement()
    {
        if (isMovingInGrid) return;
        if (TurnManager.Instance != null && !TurnManager.Instance.IsPlayerTurn()) return;

        // Visual feedback for current cell selection
        if (GridManager.Instance != null)
        {
            GridManager.Instance.SelectCell(gridPosition);
        }

        Vector2 input = GetGridInput();
        
        if (input.sqrMagnitude > 0.01f)
        {
            Vector2Int direction = Vector2Int.zero;
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                direction.x = input.x > 0 ? 1 : -1;
            }
            else
            {
                direction.y = input.y > 0 ? 1 : -1;
            }

            TryGridMove(direction);
        }
    }

    private Vector2 GetGridInput()
    {
        Vector2 joystickInput = Joystick.Input;
        Vector2 keyboardInput = Vector2.zero;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) keyboardInput.y = 1;
            else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame) keyboardInput.y = -1;
            else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame) keyboardInput.x = -1;
            else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame) keyboardInput.x = 1;
        }

        if (joystickInput.sqrMagnitude < 0.25f) joystickInput = Vector2.zero;
        else joystickInput = joystickInput.normalized;

        return keyboardInput.sqrMagnitude > 0 ? keyboardInput : joystickInput;
    }

    private void TryGridMove(Vector2Int direction)
    {
        Vector2Int targetPosition = gridPosition + direction;
        if (GridManager.Instance != null && GridManager.Instance.IsWithinBounds(targetPosition.x, targetPosition.y))
        {
            StartCoroutine(GridMoveRoutine(targetPosition));
        }
    }

    private System.Collections.IEnumerator GridMoveRoutine(Vector2Int targetGridPos)
    {
        isMovingInGrid = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = GridManager.Instance.GetWorldPosition(targetGridPos.x, targetGridPos.y) + new Vector3(GridManager.Instance.cellSize / 2, GridManager.Instance.cellSize / 2, 0);
        
        float elapsed = 0;
        while (elapsed < gridMoveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / gridMoveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        gridPosition = targetGridPos;
        isMovingInGrid = false;

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.EndPlayerTurn();
        }
    }

    // Context menu to toggle easily from editor
    [ContextMenu("Switch to Free Movement")]
    public void EnableFreeMovement() { useGridMovement = false; UpdateRigidbodyState(); }

    [ContextMenu("Switch to Grid Movement")]
    public void EnableGridMovement() { useGridMovement = true; UpdateRigidbodyState(); SnapToGrid(); }
}
