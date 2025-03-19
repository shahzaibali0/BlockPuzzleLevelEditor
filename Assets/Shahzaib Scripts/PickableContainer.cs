
using PuzzleLevelEditor.Container.Block;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class PickableContainer : MonoBehaviour
{

    [SerializeField, ReadOnly] private ContainerBlock containerBlock;

    private void Start()
    {
        containerBlock = GetComponent<ContainerBlock>();
    }

    public Vector3 Move(RaycastDirections direction, float value)
    {
        Vector3 moveVector = Vector3.zero;

        value = Mathf.Abs(value);

        switch (direction)
        {
            case RaycastDirections.Left:
                moveVector = Vector3.left * value;
                break;
            case RaycastDirections.Right:
                moveVector = Vector3.right * value;
                break;
            case RaycastDirections.Front:
                moveVector = Vector3.forward * value;
                break;
            case RaycastDirections.Down:
                moveVector = Vector3.back * value;
                break;
        }
        Debug.Log("Move Vector" + moveVector);
        if (containerBlock.CanProceedInDirection(direction))
        {
            transform.position += moveVector;

        }


        return new Vector3(transform.position.x, 0, transform.position.z);
    }

    public Vector3 Move(RaycastDirections direction, Vector3 value)
    {


        if (containerBlock.CanProceedInDirection(direction))
        {
            transform.position = value;

        }


        return new Vector3(transform.position.x, 0, transform.position.z);
    }

    public Vector3 Move(RaycastDirections direction, float value, Action callback = null)
    {
        if (!containerBlock.CanProceedInDirection(direction) || isMoving)
            return transform.position;

        Vector3 moveVector = Vector3.zero;
        value = Mathf.Abs(value);

        switch (direction)
        {
            case RaycastDirections.Left: moveVector = Vector3.left * value; break;
            case RaycastDirections.Right: moveVector = Vector3.right * value; break;
            case RaycastDirections.Front: moveVector = Vector3.forward * value; break;
            case RaycastDirections.Down: moveVector = Vector3.back * value; break;
        }

        Vector3 targetPosition = transform.position + moveVector;

        // Stop previous movement coroutine and start a new one
        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetPosition, 0.2f));

        return targetPosition;
    }

    private bool isMoving = false;

    private IEnumerator SmoothMove(Vector3 target, float duration)
    {
        isMoving = true; // Prevent multiple movements at the same time
        Vector3 start = transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure exact position
        isMoving = false; // Allow new movement
    }


}

[Serializable]
public enum RaycastDirections
{
    Right,
    Left,
    Front,
    Down,
}
