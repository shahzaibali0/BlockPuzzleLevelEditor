using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingDataCollector : MonoBehaviour
{
    public int BuildingNo = 0;
    public AllBuildingsData allBuildingsData;
    public BuildingManager BuildingManager;

    [Button(ButtonSizes.Medium)]
    public void CollectData()
    {
        allBuildingsData = GetDataOfBuildings();
    }

    public AllBuildingsData GetDataOfBuildings()
    {
        if (BuildingManager == null)
        {
            Debug.LogError("BuildingManager reference is null!");
            return allBuildingsData;
        }

        // Initialize or clear existing data
        allBuildingsData = new AllBuildingsData
        {
            BuildingNumber = BuildingNo,
            BuildingPrefab = this,
            bricksDataHolder = new List<BrickData>()
        };

        BuildingManager.BuildingInfoNumber = BuildingNo;
        allBuildingsData.TotalBuildingBricks = BuildingManager.TotalBricksInBuilding;

        // Process all building info
        for (int i = 0; i < BuildingManager.building_Infos.Count; i++)
        {
            var buildingInfo = BuildingManager.building_Infos[i];

            // Create new brick data
            var brickData = new BrickData(
                buildingInfo.GetFloorBrick(),
                buildingInfo.GetBricks()
            );

            // Process individual bricks
            for (int j = 0; j < buildingInfo.individualBrick.Count; j++)
            {
                var sourceBrick = buildingInfo.individualBrick[j];
                brickData.individualBricks.Add(new IndividualBrick(sourceBrick));
            }

            allBuildingsData.bricksDataHolder.Add(brickData);
        }

        // Calculate remaining bricks
        allBuildingsData.BuildingRemainingBrick = allBuildingsData.TotalBuildingBricks -
            allBuildingsData.bricksDataHolder.Sum(b => b.BrickPlaced);

        return allBuildingsData;
    }
}
