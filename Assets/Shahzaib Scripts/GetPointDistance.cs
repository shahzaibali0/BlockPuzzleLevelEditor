using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPointDistance : MonoBehaviour
{
    public Transform Left, Right, Up, Down;

    [Button(ButtonSizes.Medium)]
    void CalculateDistances()
    {
        Vector3 center = transform.position; // Assuming this script is on the center object

        float distanceLeft = Vector3.Distance(Left.position , Right.position);
        float distanceRight = Vector3.Distance( Right.position, Left.position);
        float distanceUp = Vector3.Distance(Up.position ,Down.position);
        float distanceDown = Vector3.Distance(Down.position , Up.position);

        Debug.Log($"Left: {distanceLeft}, Right: {distanceRight}, Up: {distanceUp}, Down: {distanceDown}");
    }

}
