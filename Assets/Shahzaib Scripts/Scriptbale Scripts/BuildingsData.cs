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

        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            allBuildingsDatas[i].brickTypes.Clear();
        }
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {

            allBuildingsDatas[i] = allBuildingsDatas[i].BuildingPrefab.GetDataOfBuildings();
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

    public List<BrickData> brickTypes = new List<BrickData>();
}

[Serializable]
public class BrickData
{

    public BrickData()
    {
        brickIcon = null;
        floortype = "DefaultBrickType";
        TotalBricks = 0;
        RemainingBrick = 0;
    }

    public BrickData(string type, int totalBricks)
    {
        floortype = type;
        TotalBricks = totalBricks;
    }

    public BrickData(BrickData other)
    {
        brickIcon = other.brickIcon;
        floortype = other.floortype;
        TotalBricks = other.TotalBricks;
        RemainingBrick = other.RemainingBrick;
    }

    public Image brickIcon;
    public string floortype;
    public int TotalBricks;
    public int RemainingBrick;
    public int BrickPlaced;
    public List<IndividualBrick> individualBricks = new List<IndividualBrick>();
}

[Serializable]
public class IndividualBrick
{
    public BrickType RequriedBrickType;
    public int TotalBrick;
    public int RemainingBrick;
    public int BrickPlaced;

}