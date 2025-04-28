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
    public List<BrickType> ExtrabricksType = new List<BrickType>();
    public List<BricksIcons> bricksIcons = new List<BricksIcons>();
    public List<BuildingNames> buildingNames = new List<BuildingNames>();
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
        for (int i = 0; i < allBuildingsDatas.Count; i++)
        {
            var prefab = allBuildingsDatas[i].BuildingPrefab;

            if (prefab != null)
            {
                var data = prefab.GetDataOfBuildings();

                if (data != null)
                {
                    allBuildingsDatas[i] = data;

                    for (int k = 0; k < allBuildingsDatas[i].FloorDatahHolder.Count; k++)
                    {
                        for (int i1 = 0; i1 < allBuildingsDatas[i].FloorDatahHolder[k].individualBricks.Count; i1++)
                        {
                            var requiredType = allBuildingsDatas[i].FloorDatahHolder[k].individualBricks[i1].RequriedBrickType;

                            BricksIcons bricksIcons1 = bricksIcons.FirstOrDefault(x => x.BrickType == requiredType);

                            if (bricksIcons1 != null)
                            {
                                allBuildingsDatas[i].FloorDatahHolder[k].individualBricks[i1].brickIcon = bricksIcons1.Icon;
                            }
                        }
                    }

                    for (int k = 0; k < allBuildingsDatas[i].BrickColorInfos.Count; k++)
                    {
                        var reqbrick = allBuildingsDatas[i].BrickColorInfos[k].BrickType;
                        BricksIcons bricksIcons1 = bricksIcons.FirstOrDefault(x => x.BrickType == reqbrick);
                        allBuildingsDatas[i].BrickColorInfos[k].Icon = bricksIcons1.Icon;
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
    public Sprite BuildingIcon;
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
    public Sprite brickIcon;
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
    public Sprite Icon;
    public BrickInfo(BrickType brickType, int totalBricksPerColor)
    {
        BrickType = brickType;
        TotalBricksPerColor = totalBricksPerColor;

    }
    public BrickInfo(BrickType brickType, int totalBricksPerColor, Sprite Icon_)
    {
        Icon = Icon_;
        BrickType = brickType;
        TotalBricksPerColor = totalBricksPerColor;

    }

    public BrickInfo()
    {


    }
}

#endregion

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
[Serializable]
public class BricksIcons
{
    public BrickType BrickType;
    public Sprite Icon;
}

[Serializable]
public class BuildingNames
{
    public String Name;
    public Sprite Icon;
}