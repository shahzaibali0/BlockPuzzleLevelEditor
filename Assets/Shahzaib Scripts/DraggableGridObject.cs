using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DraggableGridObject : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize = new Vector2Int(5, 4);
    public Vector2 gridWorldSize = new Vector2(2f, 1.5f);

    [Header("Drag Settings")]
    public float liftHeight = 0.5f;
    public LayerMask groundLayer;

    private Vector3 offset;
    private float originalY;
    private bool isDragging = false;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        originalY = transform.position.y;
    }

    private void OnMouseDown()
    {
        // Calculate offset from mouse to object center
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            offset = transform.position - hit.point;
            offset.y = 0; // Only care about XZ plane
        }

        isDragging = true;
        originalY = transform.position.y;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Follow mouse with offset while dragging
            Vector3 newPosition = hit.point + offset;
            newPosition.y = originalY + liftHeight; // Lift while dragging
            transform.position = newPosition;
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        // Snap to grid when released
        SnapToGrid();
        isDragging = false;
    }

    private void SnapToGrid()
    {
        Vector3 snappedPosition = GetSnappedPosition(transform.position);
        snappedPosition.y = originalY; // Maintain original height
        transform.position = snappedPosition;
    }

    private Vector3 GetSnappedPosition(Vector3 worldPosition)
    {
        // Calculate cell sizes
        float cellSizeX = gridWorldSize.x / Mathf.Max(1, gridSize.x - 1);
        float cellSizeZ = gridWorldSize.y / Mathf.Max(1, gridSize.y - 1);

        // Calculate grid origin (center-based)
        float gridOffsetX = -gridWorldSize.x * 0.5f;
        float gridOffsetZ = -gridWorldSize.y * 0.5f;

        // Convert world position to grid coordinates
        float gridX = (worldPosition.x - gridOffsetX) / cellSizeX;
        float gridZ = (worldPosition.z - gridOffsetZ) / cellSizeZ;

        // Snap to nearest grid point
        float snappedX = Mathf.Round(gridX) * cellSizeX + gridOffsetX;
        float snappedZ = Mathf.Round(gridZ) * cellSizeZ + gridOffsetZ;

        return new Vector3(snappedX, worldPosition.y, snappedZ);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize grid in editor when selected
        Gizmos.color = Color.cyan;
        float cellSizeX = gridWorldSize.x / Mathf.Max(1, gridSize.x - 1);
        float cellSizeZ = gridWorldSize.y / Mathf.Max(1, gridSize.y - 1);
        Vector3 origin = new Vector3(-gridWorldSize.x / 2f, 0, -gridWorldSize.y / 2f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector3 point = origin + new Vector3(x * cellSizeX, 0, z * cellSizeZ);
                Gizmos.DrawWireCube(point, new Vector3(0.1f, 0.01f, 0.1f));
            }
        }
    }
}