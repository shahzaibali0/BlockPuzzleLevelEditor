
using PuzzleLevelEditor.Container.Block;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class PickableContainer : MonoBehaviour
{

    [SerializeField, ReadOnly] private ContainerBlock _containerBlock;
    private bool _OneTime = false;

    public bool OneTime

    {
        get { return _OneTime; }
        set { _OneTime = value; }
    }
    public ContainerBlock ContainerBlock

    {
        get
        {
            return _containerBlock;
        }

        set { _containerBlock = value; }
    }
    private void Start()
    {
        _containerBlock = GetComponent<ContainerBlock>();
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
        if (_containerBlock.CanProceedInDirection(direction))
        {
            transform.position += moveVector;

        }


        return new Vector3(transform.position.x, 0, transform.position.z);
    }

    public Vector3 Move(RaycastDirections direction, Vector3 value)
    {
        if (_containerBlock.CanProceedInDirection(direction))
        {
            transform.position = value;

        }
        return new Vector3(transform.position.x, 0, transform.position.z);
    }


    public IEnumerator ExitFromGrid(Vector3 direction, float moveDistance, float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction.normalized * moveDistance);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Normalize time (0 to 1)
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //transform.position = targetPosition; // Ensure it reaches exactly
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
