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

    public float PuzzleTime = 120f;
    private void OnEnable()
    {
        BlockPass += AllBlockspassed;


    }

    private IEnumerator Start()
    {
        yield return null;
        SetExtrationsSides();
        SetExtrationsSides_Old();

        yield return new WaitForSeconds(0.5f);
        CanvasManger.Instance.StartPuzzleTimer(PuzzleTime);
        SpawnUI();

    }

    public void SpawnUI()
    {
        CanvasManger.Instance.bricksMenuManger.SpawnUiMenus();

    }
    public List<BrickType> brickTypes = new List<BrickType>();

    public void SetExtrationsSides_Old()
    {
        DataManager.Instance.buildingsData.UpdateCurrentUseBricks();

        for (int i = 0; i < ExtrationSides.Count; i++)
        {
            var currentBrickType = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].CurrentUseBricksColors[i];

            var matchedInfo = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo]
                .BrickColorInfos.FirstOrDefault(x => x.BrickType == currentBrickType);

            BrickInfo brickInfo = new BrickInfo(currentBrickType, matchedInfo.TotalBricksPerColor);
            ExtrationSides[i].BrickExtrationColor = brickInfo.BrickType;
            ExtrationSides[i].BricksValue = brickInfo.TotalBricksPerColor;
        }

        for (int i = 0; i < container.Count; i++)
        {

        }

    }
    public void SetExtrationsSides()
    {
        DataManager.Instance.buildingsData.UpdateCurrentUseBricks();

        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        AllBuildingsData extraBuildingData;

        // Determine extra building data or fallback to reserve
        if (DataManager.BuildingNo + 1 < DataManager.Instance.buildingsData.allBuildingsDatas.Count)
        {
            extraBuildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo + 1];
        }
        else
        {
            // Add any missing brick types to the reserve data
            foreach (BrickType brickType in Enum.GetValues(typeof(BrickType)))
            {
                if (brickType == BrickType.None)
                    continue;

                if (!DataManager.Instance.buildingsData.ReserveData.CurrentUseBricksColors.Contains(brickType))
                {
                    DataManager.Instance.buildingsData.ReserveData.CurrentUseBricksColors.Add(brickType);
                }
            }

            extraBuildingData = DataManager.Instance.buildingsData.ReserveData;
        }

        // Set container materials and brick types
        for (int i = 0; i < container.Count; i++)
        {
            BrickType currentType;

            // Use extra building data if index exceeds current use bricks
            if (i >= buildingData.CurrentUseBricksColors.Count)
            {
                if (i >= extraBuildingData.CurrentUseBricksColors.Count)
                {
                    Debug.LogWarning($"No extra brick type found at index {i}.");
                    continue;
                }

                currentType = extraBuildingData.CurrentUseBricksColors[i];
                var matchedMat = extraBuildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == currentType);

                if (matchedMat != null)
                {
                    container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Extra Material not found for BrickType: {currentType}");
                }

                container[i].ContainerBlock.MyBrickType = currentType;
            }
            else
            {
                currentType = buildingData.CurrentUseBricksColors[i];
                var matchedMat = buildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == currentType);

                if (matchedMat != null)
                {
                    container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Material not found for BrickType: {currentType}");
                }

                container[i].ContainerBlock.MyBrickType = currentType;
            }
        }

        // Match ExtrationSides with container and update materials
        for (int i = 0; i < ExtrationSides.Count; i++)
        {
            var matchingContainer = container.FirstOrDefault(c =>
                c != null &&
                c.ContainerBlock != null &&
                c.ContainerBlock.Color == ExtrationSides[i].BlockColor);

            if (matchingContainer != null)
            {
                ExtrationSides[i].BrickExtrationColor = matchingContainer.ContainerBlock.MyBrickType;

                var matchedMat = buildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == ExtrationSides[i].BrickExtrationColor)
                              ?? extraBuildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == ExtrationSides[i].BrickExtrationColor);

                if (matchedMat != null)
                {
                    ExtrationSides[i].UpdateMat(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Material not found for ExtrationSide BrickType: {ExtrationSides[i].BrickExtrationColor}");
                }
            }
        }
    }


    public void SetExtrationsSidesNew()
    {
        DataManager.Instance.buildingsData.UpdateCurrentUseBricks();

        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        AllBuildingsData extraBuildingData;

        // Determine extra building data or fallback to reserve
        if (DataManager.BuildingNo + 1 < DataManager.Instance.buildingsData.allBuildingsDatas.Count)
        {
            extraBuildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo + 1];
        }
        else
        {
            // Add any missing brick types to the reserve data
            foreach (BrickType brickType in Enum.GetValues(typeof(BrickType)))
            {
                if (brickType == BrickType.None)
                    continue;

                if (!DataManager.Instance.buildingsData.ReserveData.CurrentUseBricksColors.Contains(brickType))
                {
                    DataManager.Instance.buildingsData.ReserveData.CurrentUseBricksColors.Add(brickType);
                }
            }

            extraBuildingData = DataManager.Instance.buildingsData.ReserveData;
        }

        // Set container materials and brick types
        for (int i = 0; i < container.Count; i++)
        {
            BrickType currentType;

            // Use extra building data if index exceeds current use bricks
            if (i >= buildingData.CurrentUseBricksColors.Count)
            {
                if (i >= extraBuildingData.CurrentUseBricksColors.Count)
                {
                    Debug.LogWarning($"No extra brick type found at index {i}.");
                    continue;
                }

                currentType = extraBuildingData.CurrentUseBricksColors[i];
                var matchedMat = extraBuildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == currentType);

                if (matchedMat != null)
                {
                    container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Extra Material not found for BrickType: {currentType}");
                }

                container[i].ContainerBlock.MyBrickType = currentType;
            }
            else
            {
                currentType = buildingData.CurrentUseBricksColors[i];
                var matchedMat = buildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == currentType);

                if (matchedMat != null)
                {
                    container[i].ContainerBlock.SetContainerMaterials(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Material not found for BrickType: {currentType}");
                }

                container[i].ContainerBlock.MyBrickType = currentType;
            }
        }

        // Match ExtrationSides with container and update materials
        for (int i = 0; i < ExtrationSides.Count; i++)
        {
            var matchingContainer = container.FirstOrDefault(c =>
                c != null &&
                c.ContainerBlock != null &&
                c.ContainerBlock.Color == ExtrationSides[i].BlockColor);

            if (matchingContainer != null)
            {
                ExtrationSides[i].BrickExtrationColor = matchingContainer.ContainerBlock.MyBrickType;

                var matchedMat = buildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == ExtrationSides[i].BrickExtrationColor)
                              ?? extraBuildingData.bricksMats?.FirstOrDefault(mat => mat.BrickType == ExtrationSides[i].BrickExtrationColor);

                if (matchedMat != null)
                {
                    ExtrationSides[i].UpdateMat(matchedMat.Material);
                }
                else
                {
                    Debug.LogWarning($"Material not found for ExtrationSide BrickType: {ExtrationSides[i].BrickExtrationColor}");
                }
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

                CanvasManger.Instance.ShowLevelComplete();
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
