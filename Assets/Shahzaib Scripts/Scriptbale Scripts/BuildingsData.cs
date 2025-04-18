using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "BuildingMenu", menuName = "BuildingData", order = 1)]
public class BuildingsData : ScriptableObject
{
    public List<AllBuildingsData> allBuildingsDatas = new List<AllBuildingsData>();

    [Button(ButtonSizes.Large)]
    public void SetAllBuildingsData()
    {
        // Clear existing data first
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            allBuildingsDatas[i].bricksDataHolder.Clear();
        }

        // Update data for all buildings
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            if (allBuildingsDatas[i].BuildingPrefab != null)
            {
                allBuildingsDatas[i] = allBuildingsDatas[i].BuildingPrefab.GetDataOfBuildings();
            }
            else
            {
                Debug.LogWarning($"BuildingPrefab at index {i} is null!");
            }
        }
    }
}

[Serializable]
public class AllBuildingsData
{
    public int BuildingNumber;
    public BuildingDataCollector BuildingPrefab;
    public int TotalBuildingBricks;
    public int BuildingRemainingBrick;
    public List<BrickData> bricksDataHolder = new List<BrickData>();
}

[Serializable]
public class BrickData
{
    public Image brickIcon;
    public string floortype;
    public int TotalBricks;
    public int RemainingBrick;
    public int BrickPlaced;
    public List<IndividualBrick> individualBricks = new List<IndividualBrick>();

    public BrickData() : this(null, "DefaultBrickType", 0) { }

    public BrickData(string type, int totalBricks) : this(null, type, totalBricks) { }

    public BrickData(Image icon, string type, int totalBricks)
    {
        brickIcon = icon;
        floortype = type;
        TotalBricks = totalBricks;
        RemainingBrick = totalBricks; // Initialize all as remaining
        BrickPlaced = 0;
    }

    public BrickData(BrickData other)
    {
        brickIcon = other.brickIcon;
        floortype = other.floortype;
        TotalBricks = other.TotalBricks;
        RemainingBrick = other.RemainingBrick;
        BrickPlaced = other.BrickPlaced;
        individualBricks = new List<IndividualBrick>(other.individualBricks);
    }
}

[Serializable]
public class IndividualBrick
{
    public BrickType RequriedBrickType;
    public int TotalBrick;
    public int RemainingBrick;
    public int BrickPlaced;

    public IndividualBrick() : this(BrickType.None, 0, 0, 0) { }

    public IndividualBrick(BrickType brickType, int totalBrick) : this(brickType, totalBrick, totalBrick, 0) { }

    public IndividualBrick(BrickType brickType, int totalBrick, int remainingBrick, int brickPlaced)
    {
        RequriedBrickType = brickType;
        TotalBrick = totalBrick;
        RemainingBrick = remainingBrick;
        BrickPlaced = brickPlaced;
    }

    public IndividualBrick(IndividualBrick other) : this(other.RequriedBrickType, other.TotalBrick, other.RemainingBrick, other.BrickPlaced) { }
}