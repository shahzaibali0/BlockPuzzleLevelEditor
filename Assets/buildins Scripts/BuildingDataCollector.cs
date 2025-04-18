using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataCollector : MonoBehaviour
{
    public int BuildingNo = 0;
    public AllBuildingsData allBuildingsData;
    public BuildingManager BuildingManager;

    [Button(ButtonSizes.Medium)]


    public void CollectData()
    {
        allBuildingsData.brickTypes.Clear();
        GetDataOfBuildings();
    }

    public AllBuildingsData GetDataOfBuildings()
    {
        allBuildingsData.brickTypes.Clear();
        BuildingManager.BuildingInfoNumber = BuildingNo;
        allBuildingsData.BuildingPrefab = this;
        allBuildingsData.TotalBuildingBricks = BuildingManager.TotalBricksInBuilding;
        for (int i = 0; i < BuildingManager.building_Infos.Count; i++)
        {
            BrickData brickType = new BrickData(BuildingManager.building_Infos[i].GetFloorBrick(), BuildingManager.building_Infos[i].GetBricks());
            allBuildingsData.brickTypes.Add(brickType);
        }

        return allBuildingsData;
    }

}
