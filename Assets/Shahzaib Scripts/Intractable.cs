using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class IntractableSystem : MonoBehaviour
{
    public Camera Camera;
    public float CellLength = 0.5f;
    private PickableContainer grabbedObject;
    private float objectDistance;
    private Vector3 grabOffset;
    private bool SavePos = true;
    private Vector3 LastgrabObjPosition;
    private Vector3 lastMousePosition;
    private Vector3 CurrentGrabPos;
    private void Update()
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

    private Vector3 GetSnappedPosition(Vector3 currentPos)
    {
        float snappedX = GetSnappedCoordinate(currentPos.x);
        float snappedZ = GetSnappedCoordinate(currentPos.z);

        return new Vector3(snappedX, 0, snappedZ);
    }

    private float GetSnappedCoordinate(float value)
    {
        float baseValue = Mathf.Floor(value); // Get the integer base (e.g., -1, 0, 1)
        float remainder = value - baseValue;  // Get the decimal part (e.g., 0.6 from 1.6)

        // If remainder is closer to 0.75, snap to 0.75; if closer to 0.25, snap to 0.25
        if (remainder >= 0.5f) return baseValue + 0.75f;
        else return baseValue + 0.25f;
    }

    private float movementThreshold = 10f;

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
    }


//}
