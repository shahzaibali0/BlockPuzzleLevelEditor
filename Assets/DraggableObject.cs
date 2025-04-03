using UnityEngine;

public class DragHandler : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 15f;
    public float snapSpeed = 20f;
    public LayerMask draggableLayer;

    [Header("Grid Settings")]
    public float cellSize = 1f;
    public Vector2Int gridSize = new Vector2Int(10, 10);

    private Camera mainCamera;
    private Rigidbody currentDraggedObject;
    private float objectDistance;
    private bool isSnapping;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleSnapping();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        else if (Input.GetMouseButton(0) && currentDraggedObject != null)
        {
            ContinueDrag();
        }
        else if (Input.GetMouseButtonUp(0) && currentDraggedObject != null)
        {
            ReleaseDrag();
        }
    }

    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayer))
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
            {
                currentDraggedObject = rb;
                objectDistance = Vector3.Distance(mainCamera.transform.position, hit.point);
                rb.isKinematic = false;
                rb.useGravity = false;
            }
        }
    }

    private void ContinueDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objectDistance));

        Vector3 direction = (worldPoint - currentDraggedObject.position);
        currentDraggedObject.velocity = direction.normalized * (dragSpeed * direction.magnitude);
    }

    private void ReleaseDrag()
    {
        isSnapping = true;
    }

    private void HandleSnapping()
    {
        if (!isSnapping || currentDraggedObject == null) return;

        Vector3 snappedPos = GetSnappedPosition(currentDraggedObject.position);
        float distance = Vector3.Distance(currentDraggedObject.position, snappedPos);

        if (distance < 0.05f)
        {
            CompleteSnap(snappedPos);
        }
        else
        {
            Vector3 snapDirection = (snappedPos - currentDraggedObject.position).normalized;
            currentDraggedObject.velocity = snapDirection * snapSpeed;
        }
    }

    private Vector3 GetSnappedPosition(Vector3 worldPosition)
    {
        float gridWidth = (gridSize.x - 1) * cellSize;
        float gridDepth = (gridSize.y - 1) * cellSize;
        float minX = -gridWidth / 2f;
        float minZ = -gridDepth / 2f;

        float snappedX = Mathf.Round((worldPosition.x - minX) / cellSize) * cellSize + minX;
        float snappedZ = Mathf.Round((worldPosition.z - minZ) / cellSize) * cellSize + minZ;

        return new Vector3(snappedX, worldPosition.y, snappedZ);
    }

    private void CompleteSnap(Vector3 snappedPos)
    {
        currentDraggedObject.position = snappedPos;
        currentDraggedObject.velocity = Vector3.zero;
        currentDraggedObject.isKinematic = true;
        currentDraggedObject.useGravity = true;
        currentDraggedObject = null;
        isSnapping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector3 pos = GetSnappedPosition(new Vector3(x * cellSize, 0, z * cellSize));
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.9f);
            }
        }
    }
}