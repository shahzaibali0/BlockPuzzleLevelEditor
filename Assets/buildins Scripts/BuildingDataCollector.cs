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
            BrickData brickType = new BrickData(
                BuildingManager.building_Infos[i].GetFloorBrick(),
                BuildingManager.building_Infos[i].GetBricks()
            );

            allBuildingsData.brickTypes.Add(brickType);

            for (int j = 0; j < BuildingManager.building_Infos[i].individualBrick.Count; j++)
            {
                var sourceBrick = BuildingManager.building_Infos[i].individualBrick[j];
                var newBrick = new IndividualBrick(
                    sourceBrick.RequriedBrickType,
                    sourceBrick.TotalBrick,
                    sourceBrick.RemainingBrick,
                    sourceBrick.BrickPlaced
                );

                allBuildingsData.brickTypes[i].individualBricks.Add(newBrick);
            }
        }

        return allBuildingsData;
    }

}
