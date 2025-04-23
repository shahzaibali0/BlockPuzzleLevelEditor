using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "BuildingMenu", menuName = "BuildingData", order = 1)]
public class BuildingsData : ScriptableObject
{
    public List<AllBuildingsData> allBuildingsDatas = new List<AllBuildingsData>();
    public List<UserBrickData> UserBrickInfo = new List<UserBrickData>();
    public List<BrickType> ExtrabricksType = new List<BrickType>();
    public List<PuzzleBrickReserveMats> brickReserveMats = new List<PuzzleBrickReserveMats>();
    public AllBuildingsData ReserveData;
    public void UpdateCurrentUseBricks()
    {
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {


            Debug.Log("UpdateCurrentUseBricks");
            allBuildingsDatas[i].CurrentUseBricksColors.Clear();
            Debug.Log("UpdateCurrentUseBricks__");

            foreach (var brickInfo in allBuildingsDatas[i].BrickColorInfos)
            {

                Debug.Log("UpdateCurrentUseBricks__A");

                var userBrick = UserBricksManager.instance.UserBrickDatas.FirstOrDefault(x => x.BrickType == brickInfo.BrickType);
                Debug.Log("UpdateCurrentUseBricks__B" + userBrick.UserTotalBrick);
                Debug.Log("UpdateCurrentUseBricks__C" + brickInfo.TotalBricksPerColor);

                if (userBrick != null && userBrick.UserTotalBrick <= brickInfo.TotalBricksPerColor)
                {

                    Debug.Log("UpdateCurrentUseBricks__C");

                    allBuildingsDatas[i].CurrentUseBricksColors.Add(brickInfo.BrickType);
                }
            }
        }
    }

    [Button(ButtonSizes.Large)]
    public void SetAllBuildingsData()
    {
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            allBuildingsDatas[i].FloorDatahHolder.Clear();
        }
        brickReserveMats.Clear();
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            var prefab = allBuildingsDatas[i].BuildingPrefab;

            if (prefab != null)
            {
                var data = prefab.GetDataOfBuildings();

                if (data != null)
                {
                    allBuildingsDatas[i] = data;
                    if (data.bricksMats != null)
                    {
                        foreach (var matData in data.bricksMats)
                        {
                            if (matData != null && matData.Material != null)
                            {
                                var bricksMat = new BricksMats(matData.BrickType, matData.Material);
                                bool alreadyExists = brickReserveMats.Any(r => r.bricksMatsCollections.BrickType == bricksMat.BrickType);

                                if (!alreadyExists)
                                {
                                    var reserveMat = new PuzzleBrickReserveMats { bricksMatsCollections = bricksMat };
                                    brickReserveMats.Add(reserveMat);
                                }
                            }
                            else
                            {
                                Debug.Log($"Null material or brick mat at Building index {i}.");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"bricksMats is null for BuildingPrefab at index {i}.");
                    }
                }
                else
                {
                    Debug.Log($"GetDataOfBuildings returned null at index {i}.");
                }
            }
            else
            {
                Debug.Log($"BuildingPrefab at index {i} is null!");
            }
        }

    }


    public void AddOrUpdateBuildingData(int id, AllBuildingsData newBuildingData)
    {
        // Find existing entry based on a unique identifier
        var existingBuilding = allBuildingsDatas.FirstOrDefault(b => b.BuildingNumber == id);

        if (existingBuilding != null)
        {
            // Update the existing building
            int index = allBuildingsDatas.IndexOf(existingBuilding);
            allBuildingsDatas[index] = newBuildingData;
            Debug.Log($"Updated existing building: {newBuildingData.BuildingNumber}");
        }
        else
        {
            // Add new building data
            allBuildingsDatas.Add(newBuildingData);
            Debug.Log($"Added new building: {newBuildingData.BuildingNumber}");
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this); // Mark asset dirty so changes are saved
#endif
    }

}

#region Buildings Data

[Serializable]
public class AllBuildingsData
{
    public int BuildingNumber;
    public BuildingDataCollector BuildingPrefab;
    public int TotalBuildingBricks;
    public int BuildingRemainingBrick;
    public List<BrickData> FloorDatahHolder = new List<BrickData>();
    public List<BrickInfo> BrickColorInfos = new List<BrickInfo>();
    public List<BrickType> CurrentUseBricksColors = new List<BrickType>();
    public List<BricksMats> bricksMats = new List<BricksMats>();
}

[Serializable]
public class BrickData
{
    public string floortype;
    public int TotalBricks;
    public int RemainingBrick;
    public int BrickPlaced;
    public List<IndividualBrick> individualBricks = new List<IndividualBrick>();
    // Default constructor
    public BrickData() : this("DefaultBrickType", 0) { }

    // Main constructor
    public BrickData(string type, int totalBricks)
    {
        floortype = type;
        TotalBricks = totalBricks;
        RemainingBrick = totalBricks;
        BrickPlaced = 0;
    }

    // Copy constructor
    public BrickData(BrickData other)
    {
        floortype = other.floortype;
        TotalBricks = other.TotalBricks;
        RemainingBrick = other.RemainingBrick;
        BrickPlaced = other.BrickPlaced;
        individualBricks = new List<IndividualBrick>(other.individualBricks);

    }
}

[System.Serializable]
public class IndividualBrick
{
    public Image brickIcon;
    public BrickType RequriedBrickType;
    public int TotalBrick;
    public int RemainingBrick;
    public int BrickPlaced;

    // Default constructor
    public IndividualBrick()
    {
        brickIcon = null;
        RequriedBrickType = BrickType.None;
        TotalBrick = 0;
        RemainingBrick = 0;
        BrickPlaced = 0;
    }

    // Constructor with type and total count
    public IndividualBrick(BrickType brickType, int totalBrick)
    {
        brickIcon = null;
        RequriedBrickType = brickType;
        TotalBrick = totalBrick;
        RemainingBrick = totalBrick;
        BrickPlaced = 0;
    }

    // Full parameter constructor
    public IndividualBrick(BrickType brickType, int totalBrick, int remainingBrick, int brickPlaced)
    {
        brickIcon = null;
        RequriedBrickType = brickType;
        TotalBrick = totalBrick;
        RemainingBrick = remainingBrick;
        BrickPlaced = brickPlaced;
    }

    // Copy constructor
    public IndividualBrick(IndividualBrick other)
    {
        brickIcon = other.brickIcon;
        RequriedBrickType = other.RequriedBrickType;
        TotalBrick = other.TotalBrick;
        RemainingBrick = other.RemainingBrick;
        BrickPlaced = other.BrickPlaced;
    }
}

#endregion

#region User Bricks Data

[Serializable]
public class UserBrickData
{
    public BrickType BrickType;
    public int UserTotalBrick;

    public UserBrickData() : this(BrickType.None, 0) { }

    public UserBrickData(BrickType brickType, int totalBrick)
    {
        BrickType = brickType;
        UserTotalBrick = totalBrick;
    }

    public UserBrickData(UserBrickData other) : this(other.BrickType, other.UserTotalBrick) { }
}


[Serializable]
public class BrickInfo
{
    public BrickType BrickType;
    public int TotalBricksPerColor, RemaingBricksPerColor;
}

#endregion

[Serializable]
public class PuzzleBrickReserveMats
{
    public BricksMats bricksMatsCollections;

}
[Serializable]
public class BricksMats
{
    public BrickType BrickType;
    public Material Material;

    public BricksMats(BrickType brickType, Material material)
    {
        BrickType = brickType;
        Material = material;
    }
}