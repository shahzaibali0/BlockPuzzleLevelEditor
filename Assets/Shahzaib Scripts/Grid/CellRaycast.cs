using System.Collections.Generic;
using PuzzleLevelEditor.BorderLogic;
using PuzzleLevelEditor.GridItem;
using Sirenix.OdinInspector;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class CellRaycast : MonoBehaviour
{

    public List<Transform> objects; // List of objects to check
    public List<ObjectDirection> classifiedObjects = new List<ObjectDirection>(); // Store results
    RaycastHit hit;
    private void Start()
    {
    }

    [Button(ButtonSizes.Medium)]
    private void ClassifyObjects()
    {
        classifiedObjects.Clear(); // Clear list before checking

        foreach (Transform obj in objects)
        {
            RaycastDirections detectedDirection = GetObjectDirection(obj);
            classifiedObjects.Add(new ObjectDirection(detectedDirection, obj));
        }

        // Debug to verify
        foreach (var entry in classifiedObjects)
        {
            //Debug.Log($"Object: {entry.obj.name} is in {entry.direction} direction.");
        }
    }

    private RaycastDirections GetObjectDirection(Transform obj)
    {


        // Convert world position to LOCAL SPACE relative to this object
        Vector3 localPos1 = obj.forward;
        // Compare with LOCAL AXES
        float dotRight = Vector3.Dot(localPos1, Vector3.right);    // LOCAL right
        float dotLeft = Vector3.Dot(localPos1, -Vector3.right);   // LOCAL left
        float dotFront = Vector3.Dot(localPos1, Vector3.forward); // LOCAL front
        float dotBack = Vector3.Dot(localPos1, -Vector3.forward); // LOCAL back
        float dotDown = Vector3.Dot(localPos1, -Vector3.up);      // LOCAL down

        // Determine the closest direction
        if (dotRight > dotLeft && dotRight > dotFront && dotRight > dotBack && dotRight > dotDown)
            return RaycastDirections.Right;
        if (dotLeft > dotRight && dotLeft > dotFront && dotLeft > dotBack && dotLeft > dotDown)
            return RaycastDirections.Left;
        if (dotFront > dotRight && dotFront > dotLeft && dotFront > dotBack && dotFront > dotDown)
            return RaycastDirections.Front;
        if (dotBack > dotRight && dotBack > dotLeft && dotBack > dotFront && dotBack > dotDown)
            return RaycastDirections.Down;

        // If none of the above, it must be downward
        return RaycastDirections.Down;  // Or define a new "Down" enum if needed

    }

    Transform Obj;

    public Transform GetRayData(RaycastDirections raycastDirections)
    {
        ClassifyObjects();


        foreach (var item in classifiedObjects)
        {
            if (item.direction == raycastDirections)
            {
                return Obj = item.obj;
            }
            else
            {

            }
        }

        return null;
    }

}

[System.Serializable]
public class ObjectDirection
{
    public RaycastDirections direction;
    public Transform obj;

    public ObjectDirection(RaycastDirections dir, Transform objectTransform)
    {
        direction = dir;
        obj = objectTransform;
    }
}
