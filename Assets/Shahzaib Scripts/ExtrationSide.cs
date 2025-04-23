using PuzzleLevelEditor.Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrationSide : MonoBehaviour
{
    public BlockColor BlockColor;
    public BrickType BrickExtrationColor;
    public float Multipler = 2;
    public void AddBrick()
    {
        float Value = Random.Range(50, 100) * Multipler;
        Debug.Log("Value" + Value);
        UserBricksManager.instance.AddBrickonExtrationPoint(BrickExtrationColor, (int)Value);
    }

    public void UpdateMat(Material material)
    {

        Debug.Log("Change MatA");
        gameObject.transform.GetComponent<MeshRenderer>().material = material;
    }
}
