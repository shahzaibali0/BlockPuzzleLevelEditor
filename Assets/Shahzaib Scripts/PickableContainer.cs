
using PuzzleLevelEditor.Container.Block;
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class PickableContainer : MonoBehaviour
{

    [SerializeField, ReadOnly] private ContainerBlock _containerBlock;
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
