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

    public List<PerLevelbricks> perLevelbricks = new List<PerLevelbricks>();
    private void OnEnable()
    {
        BlockPass += AllBlockspassed;


    }

    private IEnumerator Start()
    {
        yield return null;
        SetExtrationsSides_Old();

        yield return new WaitForSeconds(0.5f);
        CanvasManger.Instance.StartPuzzleTimer(PuzzleTime);
        SpawnUI();

    }

    public void SpawnUI()
    {
        //CanvasManger.Instance.bricksMenuManger.SpawnUiMenus();

    }

    public void SetExtrationsSides_Old()
    {
        DataManager.Instance.buildingsData.UpdateCurrentUseBricks();

        for (int i = 0; i < ExtrationSides.Count; i++)
        {
            var currentBrickType = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].CurrentUseBricksColors[i];

            var matchedInfo = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo]
                .BrickColorInfos.FirstOrDefault(x => x.BrickType == currentBrickType);
            Debug.Log("DataManager.BuildingNo" + DataManager.BuildingNo);
            var MatInfo = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo]
              .bricksMats.FirstOrDefault(x => x.BrickType == currentBrickType);

            BrickInfo brickInfo = new BrickInfo(currentBrickType, matchedInfo.TotalBricksPerColor);
            ExtrationSides[i].BrickExtrationColor = brickInfo.BrickType;
            ExtrationSides[i].BricksValue = brickInfo.TotalBricksPerColor;
            if (MatInfo != null)
                ExtrationSides[i].UpdateMat(MatInfo.Material);
            else
                Debug.Log("MatInfo is null ");
            for (global::System.Int32 j = 0; j < container.Count; j++)
            {
                if (container[j].ContainerBlock.Color == ExtrationSides[i].BlockColor)
                {
                    container[j].ContainerBlock.MyBrickType = brickInfo.BrickType;
                    if (MatInfo != null)
                        container[j].ContainerBlock.SetContainerMaterials(MatInfo.Material);
                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        BlockPass -= AllBlockspassed;
    }

    public void SaveBricks(BrickType brickType, int value)
    {
        PerLevelbricks PerLevelbricks = new PerLevelbricks(brickType, value);
        perLevelbricks.Add(PerLevelbricks);

    }

    public void SetBricksData()
    {

        for (int i = 0; i < perLevelbricks.Count; i++)
        {
            UserBricksManager.instance.AddBrickonExtrationPoint(perLevelbricks[i].BrickType, perLevelbricks[i].BricksNo);
        }
    }

    bool OneTime;
    public void AllBlockspassed()
    {
        foreach (var item in container)
        {
            if (container.All(item => item.ContainerBlock.BlockPass))
            {
                Debug.Log("Level Complete");
                if (!OneTime)
                {
                    OneTime = true;
                    SetBricksData();
                }
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
[Serializable]
public class PerLevelbricks
{
    public BrickType BrickType;
    public int BricksNo;

    public PerLevelbricks(BrickType BrickType_, int No)
    {
        BrickType = BrickType_;
        BricksNo = No;
    }
}