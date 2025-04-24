using PuzzleLevelEditor.Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrationSide : MonoBehaviour
{
    public BlockColor BlockColor;
    public BrickType BrickExtrationColor;
    public float BricksValue = 2;
    public void AddBrick()
    {
        Debug.Log("Current Value " + BricksValue);
        UserBricksManager.instance.AddBrickonExtrationPoint(BrickExtrationColor, (int)BricksValue);
    }

    public void UpdateMat(Material material)
    {

        Debug.Log("Change MatA");
        gameObject.transform.GetComponent<MeshRenderer>().material = material;
    }
}
