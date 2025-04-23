using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{

    public List<PickableContainer> container = new List<PickableContainer>();
    public List<ExtrationSide> ExtrationSides = new List<ExtrationSide>();

    public Action BlockPass;
    public float WidthBlocks, HeightBlocks;
    private void OnEnable()
    {
        BlockPass += AllBlockspassed;


    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.25f);
        SetExtrationsSides();
    }

    public List<BrickType> brickTypes = new List<BrickType>();

    public void SetExtrationsSides()
    {
        DataManager.Instance.buildingsData.UpdateCurrentUseBricks();


        //for (int i = 0; i < ExtrationSides.Count; i++)
        //{
        //    var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        //    var extraBricksTypes = DataManager.Instance.buildingsData.ExtrabricksType;

        //    if (i >= buildingData.CurrentUseBricksColors.Count)
        //    {
        //        BrickType extraType = buildingData.CurrentUseBricksColors[i];
        //        var matchedMat = buildingData.bricksMats.FirstOrDefault(mat => mat.BrickType == extraType);
        //        ExtrationSides[i].UpdateMat(matchedMat.Material);
        //        ExtrationSides[i].BrickExtrationColor = extraType;
        //    }
        //    else
        //    {
        //        BrickType extraType = buildingData.CurrentUseBricksColors[i];
        //        var matchedMat = buildingData.bricksMats.FirstOrDefault(mat => mat.BrickType == extraType);
        //        ExtrationSides[i].UpdateMat(matchedMat.Material);
        //        ExtrationSides[i].BrickExtrationColor = buildingData.CurrentUseBricksColors[i];
        //    }
        //}
        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        var extraBricksTypes = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo + 1];
        for (int i = 0; i < container.Count; i++)
        {


            if (i >= buildingData.CurrentUseBricksColors.Count)
            {
                BrickType extraType = buildingData.CurrentUseBricksColors[i];
                var matchedMat = buildingData.bricksMats.FirstOrDefault(mat => mat.BrickType == extraType);
                container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                container[i].ContainerBlock.MyBrickType = extraType;
            }
            else
            {
                BrickType extraType = buildingData.CurrentUseBricksColors[i];
                var matchedMat = buildingData.bricksMats.FirstOrDefault(mat => mat.BrickType == extraType);
                container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                container[i].ContainerBlock.MyBrickType = extraType;
            }
        }

        for (int i = 0; i < ExtrationSides.Count; i++)
        {
            var matchingContainer = container.FirstOrDefault(c =>
                c != null &&
                c.ContainerBlock != null &&
                c.ContainerBlock.Color == ExtrationSides[i].BlockColor);

            if (matchingContainer != null)
            {
                ExtrationSides[i].BrickExtrationColor = matchingContainer.ContainerBlock.MyBrickType;

                var matchedMat_ = buildingData.bricksMats.FirstOrDefault(mat => mat.BrickType == ExtrationSides[i].BrickExtrationColor);

                ExtrationSides[i].UpdateMat(matchedMat_.Material);
            }
        }
    }



    private void OnDisable()
    {
        BlockPass -= AllBlockspassed;
    }


    public void AllBlockspassed()
    {
        foreach (var item in container)
        {
            if (container.All(item => item.ContainerBlock.BlockPass))
            {
                Debug.Log("Level Complete");
            }
        }

    }
    [Button(ButtonSizes.Medium)]
    public void SaveExtrationSides()
    {
        ExtrationSides.Clear();
        int foundCount = 0;

        // Start recursive search from root transform
        CheckChildrenRecursive(transform);

        Debug.Log($"Found {foundCount} extraction sides in all children");

        void CheckChildrenRecursive(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Check current child
                ExtrationSide extrationSide = child.GetComponent<ExtrationSide>();
                if (extrationSide != null)
                {
                    ExtrationSides.Add(extrationSide);
                    foundCount++;
                }

                // Recursively check all descendants
                if (child.childCount > 0)
                {
                    CheckChildrenRecursive(child);
                }
            }
        }
    }
}
