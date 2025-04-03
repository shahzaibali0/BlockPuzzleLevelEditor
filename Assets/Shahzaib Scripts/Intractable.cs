using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IntractableSystem : MonoBehaviour
{
    public Camera Camera;
    public float CellLength = 0.5f;
    public PickableContainer grabbedObject;
    private float objectDistance;
    private Vector3 grabOffset;
    private bool SavePos = true;
    private Vector3 LastgrabObjPosition;
    private Vector3 lastMousePosition;
    private Vector3 CurrentGrabPos;
    public Rigidbody grabbedRb;
    private bool isSnapping = false;

    [Header("Grid Settings")]
    public Vector2Int gridSize = new Vector2Int(5, 4);
    public Vector2 gridWorldSize = new Vector2(2f, 1.5f);
    private float cellSize = 0.5f;
    [Header("Drag Settings")]
    public LayerMask groundLayer;
    private MovementConstraint movementConstraint;
    [Button(ButtonSizes.Medium)]
    private void Start()
    {
        gridSize.x = (int)LevelManager.Instance.LevelInfo.WidthBlocks;
        gridSize.y = (int)LevelManager.Instance.LevelInfo.HeightBlocks;
    }


    bool IsExtratPoint;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsExtratPoint = false;
            TryGrabObject();
        }

        if (grabbedRb != null && Input.GetMouseButton(0))
        {
            MoveObjectWithMouse();
        }


        if (Input.GetMouseButtonUp(0) && grabbedRb != null)
        {

            grabbedObject.ContainerBlock.StartEmitRayCastFromAllSides((bool result) =>
            {
                IsExtratPoint = result;
            }); ;
            if (!IsExtratPoint)
            {
                StartSnapping();
            }

        }
    }

    private void TryGrabObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out grabbedRb))
            {

                grabbedObject = hit.collider.GetComponent<PickableContainer>();
                movementConstraint = grabbedObject.ContainerBlock.MovementConstraint;
                grabbedRb.isKinematic = false; // Enable physics movement
                grabbedRb.useGravity = false;  // Disable gravity while moving

                objectDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
                grabOffset = grabbedRb.position - hit.point;

            }
        }
    }
    public float Speed;
  

    private void MoveObjectWithMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objectDistance));

        Vector3 targetPos = grabbedRb.position;
        switch (movementConstraint)
        {
            case MovementConstraint.None:
                targetPos = new Vector3(worldPoint.x + grabOffset.x, grabbedRb.position.y, worldPoint.z + grabOffset.z);
                break;
            case MovementConstraint.Horizontal:
                targetPos = new Vector3(worldPoint.x + grabOffset.x, grabbedRb.position.y, grabbedRb.position.z);
                break;
            case MovementConstraint.Vertical:
                targetPos = new Vector3(grabbedRb.position.x, grabbedRb.position.y, worldPoint.z + grabOffset.z);
                break;
        }

        Vector3 direction = targetPos - grabbedRb.position;


        float speed_x = direction.x * Speed;
        float speed_y = direction.z * Speed;


        //Vector3 velocity = direction.normalized * speed;
        Vector3 velocity = new Vector3(speed_x, 0, speed_y);

        //Debug.Log($"Velocity Components - X: {velocity.x.ToString("F2")}, " +
        //          $"Y: {velocity.y.ToString("F2")}, " +
        //          $"Z: {velocity.z.ToString("F2")} | " +
        //          $"Resultant Magnitude: {velocity.magnitude.ToString("F2")}");

        grabbedRb.velocity = velocity;
    }

    private void StartSnapping()
    {
        isSnapping = true;
    }

    private void MoveObjectWithMouse__()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objectDistance));

        // Determine target position based on movement constraint
        Vector3 targetPos = grabbedRb.position;

        switch (movementConstraint)
        {
            case MovementConstraint.None:
                targetPos = new Vector3(worldPoint.x + grabOffset.x, grabbedRb.position.y, worldPoint.z + grabOffset.z);
                break;

            case MovementConstraint.Horizontal:
                targetPos = new Vector3(worldPoint.x + grabOffset.x, grabbedRb.position.y, grabbedRb.position.z); // Only move in X
                break;

            case MovementConstraint.Vertical:
                targetPos = new Vector3(grabbedRb.position.x, grabbedRb.position.y, worldPoint.z + grabOffset.z); // Only move in Z
                break;
        }

        Vector3 direction = targetPos - grabbedRb.position;
        float speed = direction.magnitude * Speed; // Adjust speed for smoother movement

        grabbedRb.velocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        if (isSnapping && grabbedRb != null)
        {
            Vector3 snappedPos = GetUniversalSnappedPosition(grabbedRb.position);
            Vector3 direction = snappedPos - grabbedRb.position;
            float snapSpeed = 5f;

            if (direction.magnitude > 0.01f)
            {
                grabbedRb.velocity = direction.normalized * snapSpeed;
            }
            else
            {
                grabbedRb.velocity = Vector3.zero;
            }

            if (direction.magnitude < 0.05f)
            {
                grabbedRb.velocity = Vector3.zero;
                grabbedRb.useGravity = true;
                grabbedRb.isKinematic = true;

                // Snap precisely to final position
                grabbedRb.position = snappedPos;


                //IsExtratPoint = grabbedObject.ContainerBlock.EmitRayCastFromAllSides();
                IsExtratPoint = false;
                grabbedRb = null;
                isSnapping = false;
                grabbedObject = null;
            }
        }
    }


    private Vector3 GetUniversalSnappedPosition(Vector3 worldPosition)
    {
        // Ensure grid is valid
        if (gridSize.x < 1 || gridSize.y < 1) return worldPosition;

        // Ensure cell sizes match fixed cell size
        float cellSizeX = cellSize;
        float cellSizeZ = cellSize;

        // Center grid
        float gridWidth = (gridSize.x - 1) * cellSizeX;
        float gridHeight = (gridSize.y - 1) * cellSizeZ;
        float minX = -gridWidth / 2f;
        float minZ = -gridHeight / 2f;

        // Snap position correctly
        float snappedX = Mathf.Round((worldPosition.x - minX) / cellSizeX) * cellSizeX + minX;
        float snappedZ = Mathf.Round((worldPosition.z - minZ) / cellSizeZ) * cellSizeZ + minZ;

        return new Vector3(snappedX, worldPosition.y, snappedZ);
    }




    private Vector3 GetSnappedPosition_Old(Vector3 currentPos)
    {
        float snappedX = GetSnappedCoordinate(currentPos.x);
        float snappedZ = GetSnappedCoordinate(currentPos.z);
        return new Vector3(snappedX, 0, snappedZ);
    }

    private float GetSnappedCoordinate(float value)
    {
        float baseValue = Mathf.Floor(value);
        float remainder = value - baseValue;
        return remainder >= 0.5f ? baseValue + 0.75f : baseValue + 0.25f;
    }



    private Vector3 GetSnappedPosition(Vector3 worldPosition)
    {
        // Calculate total grid size
        float gridWidth = (gridSize.x - 1) * cellSize;
        float gridDepth = (gridSize.y - 1) * cellSize;

        // Center the grid around (0,0,0)
        float minX = -gridWidth / 2f;
        float minZ = -gridDepth / 2f;

        // Calculate grid coordinates
        float gridX = Mathf.Round((worldPosition.x - minX) / cellSize) * cellSize + minX;
        float gridZ = Mathf.Round((worldPosition.z - minZ) / cellSize) * cellSize + minZ;

        return new Vector3(gridX, worldPosition.y, gridZ);
    }

    #region Movement based On transform

    private void FixedUpdate__Old()
    {
        if (isSnapping && grabbedRb != null)
        {
            Vector3 snappedPos = GetSnappedPosition(grabbedRb.position);
            Vector3 direction = snappedPos - grabbedRb.position;
            float snapSpeed = 5f; // Speed of snapping

            grabbedRb.velocity = direction.normalized * snapSpeed;

            if (direction.magnitude < 0.05f)
            {
                grabbedRb.velocity = Vector3.zero;
                grabbedRb.useGravity = true;
                grabbedRb.isKinematic = true;
                //grabbedRb.position = snappedPos;


                grabbedObject.transform.position = new Vector3(snappedPos.x, 0, snappedPos.z);
                grabbedRb = null;
                isSnapping = false;
                grabbedObject = null;

            }
        }
    }
    private void Update_Old()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (grabbedObject == null)
            {

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.TryGetComponent<PickableContainer>(out PickableContainer component))
                    {
                        if (SavePos)
                        {
                            SavePos = false;
                            LastgrabObjPosition = hit.collider.transform.position;
                        }

                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                        grabbedObject = component;

                        objectDistance = Vector3.Distance(Camera.transform.position, hit.point);

                        grabOffset = grabbedObject.transform.position - hit.point;


                    }
                    else
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                    }
                }
                else
                {

                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.black);
                }
            }
        }

        if (grabbedObject != null && Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPoint = Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objectDistance));

            CurrentGrabPos = new Vector3(worldPoint.x + grabOffset.x, 0, worldPoint.z + grabOffset.z);
            //grabbedObject.transform.position = CurrentGrabPos;


            DetectMouseMovementDirection(Input.mousePosition, CurrentGrabPos);

        }



        if (Input.GetMouseButtonUp(0))
        {
            SavePos = true;
            //if (grabbedObject != null)
            //    grabbedObject.transform.position = LastgrabObjPosition;

            if (grabbedObject != null)
            {
                Debug.Log("Last grabbed obj postion" + grabbedObject.transform.position);
                grabbedObject.transform.position = GetSnappedPosition(grabbedObject.transform.position);
            }
            grabbedObject = null;
        }

    }
    private void DetectMouseMovementDirection(Vector3 currentMousePosition, Vector3 GrabbedObjPos)
    {
        Vector3 delta = currentMousePosition - lastMousePosition;
        //if (delta.magnitude > movementThreshold)
        //{
        if (grabbedObject != null)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {

                if (delta.x > 0)
                {

                    Debug.Log("Right Direction");

                    LastgrabObjPosition = grabbedObject.Move(RaycastDirections.Right, GrabbedObjPos);
                }
                else
                {
                    Debug.Log("Left Direction");

                    LastgrabObjPosition = grabbedObject.Move(RaycastDirections.Left, GrabbedObjPos);

                }
            }
            else
            {
                if (delta.y > 0)
                {

                    Debug.Log("Front Direction");

                    LastgrabObjPosition = grabbedObject.Move(RaycastDirections.Front, GrabbedObjPos);

                }
                else
                {

                    Debug.Log("Down Direction");

                    LastgrabObjPosition = grabbedObject.Move(RaycastDirections.Down, GrabbedObjPos);

                }
            }
        }
        lastMousePosition = currentMousePosition;
    }


    #endregion
}


//}
