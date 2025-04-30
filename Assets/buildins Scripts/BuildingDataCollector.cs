using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingDataCollector : MonoBehaviour
{
    public static BuildingDataCollector Instance;


    private void Awake()
    {
        Instance = this;
    }

    public int BuildingNo = 0;
    public Sprite Icon;
    public AllBuildingsData allBuildingsData;
    public BuildingManager BuildingManager;

    public GameObject BuildingCam;
    private void Start()
    {
        BuildingManager.OffBuilding();
        GetDataFromDatabase();

        CanvasManger.Instance.Taptap(true);

    }


    public void GetDataFromDatabase()
    {
        for (int i = 0; i < DataManager.Instance.buildingsData.allBuildingsDatas.Count; i++)
        {
            if (DataManager.Instance.buildingsData.allBuildingsDatas[i].BuildingNumber == BuildingNo)
            {
                allBuildingsData = DataManager.Instance.buildingsData.allBuildingsDatas[i];
                break;
            }
        }
        Debug.Log("InilizeBuildingdata");
        StartCoroutine(LevelManager.Instance.EnableRespectiveCam(0, false, true));

        BuildingManager.InilizeBuildingdata();

    }

    [Button(ButtonSizes.Medium)]
    public void CollectInternalData()
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
            FloorDatahHolder = new List<BrickData>(),
            BrickColorInfos = new List<BrickInfo>()
        };
        BuildingManager.BuildingInfoNumber = BuildingNo;
        allBuildingsData.TotalBuildingBricks = BuildingManager.TotalBricksInBuilding;
        allBuildingsData.BuildingIcon = Icon;
        // Dictionary to aggregate brick counts by type
        Dictionary<BrickType, BrickInfo> colorInfoDict = new Dictionary<BrickType, BrickInfo>();

        // Process all building info
        for (int i = 0; i < BuildingManager.building_Infos.Count; i++)
        {
            var buildingInfo = BuildingManager.building_Infos[i];
            var brickData = new BrickData(
                buildingInfo.GetFloorBrick(),
                buildingInfo.GetBricks()
            );

            // Process individual bricks
            for (int j = 0; j < buildingInfo.individualBrick.Count; j++)
            {
                var sourceBrick = buildingInfo.individualBrick[j];
                brickData.individualBricks.Add(new IndividualBrick(sourceBrick));

                // Update color info
                if (!colorInfoDict.TryGetValue(sourceBrick.RequriedBrickType, out BrickInfo info))
                {
                    info = new BrickInfo { BrickType = sourceBrick.RequriedBrickType };
                    colorInfoDict[sourceBrick.RequriedBrickType] = info;
                }

                info.TotalBricksPerColor += sourceBrick.TotalBrick;
                info.RemaingBricksPerColor += sourceBrick.RemainingBrick;
            }

            allBuildingsData.FloorDatahHolder.Add(brickData);
        }

        // Store aggregated color info
        allBuildingsData.BrickColorInfos = colorInfoDict.Values.ToList();

        // Calculate remaining bricks
        allBuildingsData.BuildingRemainingBrick = allBuildingsData.TotalBuildingBricks -
            allBuildingsData.FloorDatahHolder.Sum(b => b.BrickPlaced);

        BrickTypeAutoAssigner Mat = gameObject.transform.GetComponent<BrickTypeAutoAssigner>();

        for (int i = 0; i < Mat.PuzzleMat.Count; i++)
        {
            BricksMats bricksMats = new BricksMats(Mat.PuzzleMat[i].brickType, Mat.PuzzleMat[i].material);
            allBuildingsData.bricksMats.Add(bricksMats);
        }

        return allBuildingsData;
    }
    public void TotalRemainingBricks()
    {
        allBuildingsData.BuildingRemainingBrick--;
    }

    #region Bricks Updating Data
    public void FloorBrick_Placed(string FloorName)
    {
        for (int i = 0; i < allBuildingsData.FloorDatahHolder.Count; i++)
        {
            if (FloorName == allBuildingsData.FloorDatahHolder[i].floortype)
            {
                allBuildingsData.FloorDatahHolder[i].BrickPlaced++;
            }
        }
    }
    public void FloorBrick_Remained(string FloorName)
    {
        for (int i = 0; i < allBuildingsData.FloorDatahHolder.Count; i++)
        {
            if (FloorName == allBuildingsData.FloorDatahHolder[i].floortype)
            {
                allBuildingsData.FloorDatahHolder[i].RemainingBrick--;
            }
        }

        SaveCurrentBuildinggData();
    }

    public void FloorIndividualBricks_Placed(string FloorName, BrickType brickType)
    {
        for (int i = 0; i < allBuildingsData.FloorDatahHolder.Count; i++)
        {
            if (FloorName == allBuildingsData.FloorDatahHolder[i].floortype)
            {
                for (global::System.Int32 j = 0; j < allBuildingsData.FloorDatahHolder[i].individualBricks.Count; j++)
                {
                    if (allBuildingsData.FloorDatahHolder[i].individualBricks[j].RequriedBrickType == brickType)
                    {
                        allBuildingsData.FloorDatahHolder[i].individualBricks[j].BrickPlaced++;
                    }
                }
            }
        }

        for (int i = 0; i < allBuildingsData.BrickColorInfos.Count; i++)
        {
            if (allBuildingsData.BrickColorInfos[i].BrickType == brickType)
            {
                allBuildingsData.BrickColorInfos[i].RemaingBricksPerColor--;
            }
        }
    }

    public void FloorIndividualBricks_Remained(string FloorName, BrickType brickType)
    {
        for (int i = 0; i < allBuildingsData.FloorDatahHolder.Count; i++)
        {
            if (FloorName == allBuildingsData.FloorDatahHolder[i].floortype)
            {
                for (global::System.Int32 j = 0; j < allBuildingsData.FloorDatahHolder[i].individualBricks.Count; j++)
                {
                    if (allBuildingsData.FloorDatahHolder[i].individualBricks[j].RequriedBrickType == brickType)
                    {
                        allBuildingsData.FloorDatahHolder[i].individualBricks[j].RemainingBrick--;
                    }
                }
            }
        }
    }

    public void Activate_FloorBrick_Placed(string FloorName, List<BuildingSinglePiece> Objects)
    {
        for (int i = 0; i < allBuildingsData.FloorDatahHolder.Count; i++)
        {
            if (FloorName == allBuildingsData.FloorDatahHolder[i].floortype)
            {
                for (global::System.Int32 j = 0; j < allBuildingsData.FloorDatahHolder[i].BrickPlaced; j++)
                {
                    if (allBuildingsData.FloorDatahHolder[i].BrickPlaced != 0)
                    {
                        Objects[j].gameObject.SetActive(true);
                        BuildingManager.CurrentBuildingStep = i;
                        BuildingManager.building_Infos[i].currentActive = allBuildingsData.FloorDatahHolder[i].BrickPlaced;
                    }
                    else
                    {
                        break;
                    }

                }
            }


        }


    }

    private void SaveCurrentBuildinggData()
    {
        DataManager.Instance.buildingsData.AddOrUpdateBuildingData(BuildingNo, allBuildingsData);
    }

    #endregion
}
