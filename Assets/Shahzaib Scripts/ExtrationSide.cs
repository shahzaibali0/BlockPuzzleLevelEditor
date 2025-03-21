using PuzzleLevelEditor.Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrationSide : MonoBehaviour
{
    public BlockColor BlockColor;

    PickableContainer Block;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PickableContainer>(out PickableContainer component))
        {
            Block = component;

            if (component.ContainerBlock.Color == BlockColor)
            {
                if (!component.OneTime)
                {
                    component.OneTime = true;
                    Debug.Log("Blue Color Side Trigger");
                    Vector3 direction = transform.position - other.transform.position;
                    DetectCollisionSide(direction.normalized);
                }
            }
        }
    }

    void DetectCollisionSide(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if (direction.x > 0)
            {
                Debug.Log("Collided from Right");
                StartCoroutine(Block.ExitFromGrid(Vector3.left, 5, 3));
            }
            else
            {
                StartCoroutine(Block.ExitFromGrid(Vector3.right, 5, 3));

                Debug.Log("Collided from Left");
            }
        }
        else
        {
            if (direction.z > 0)
            {
                Debug.Log("Collided from Front");
                StartCoroutine(Block.ExitFromGrid(Vector3.forward, 5, 3));

            }
            else
            {
                StartCoroutine(Block.ExitFromGrid(Vector3.down, -5, 3));

                Debug.Log("Collided from Back");
            }
        }
    }

}
