using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BricksMenuManger : MonoBehaviour
{
    public Image BuildingIcon;

    public BrickInfoUI brickPrefab;
    public Transform ParentObject;

    public List<BrickInfoUI> Bricks = new List<BrickInfoUI>();

    [Button(ButtonSizes.Medium)]
    public void SpawnUiMenus()
    {
        // Update the building icon first
        BuildingIcon.sprite = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BuildingIcon;
        BuildingIcon.SetNativeSize();

        var brickColorInfos = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos;

        // Loop over BrickColorInfos
        for (int i = 0; i < brickColorInfos.Count; i++)
        {
            Debug.Log("SpawnUiMenus__A" + i);

            if (i < Bricks.Count)
            {
                // If the BrickInfoUI already exists, just initialize it with new data
                Bricks[i].Initialize(brickColorInfos[i].BrickType, brickColorInfos[i].TotalBricksPerColor);
            }
            else
            {
                // If it does not exist, instantiate a new one and add it to the list
                BrickInfoUI brick = Instantiate(brickPrefab, ParentObject);
                brick.Initialize(brickColorInfos[i].BrickType, brickColorInfos[i].TotalBricksPerColor);
                Bricks.Add(brick);
            }
        }

        // If Bricks list is longer than BrickColorInfos, destroy the extra UI elements
        for (int j = Bricks.Count - 1; j >= brickColorInfos.Count; j--)
        {
            Destroy(Bricks[j].gameObject);
            Bricks.RemoveAt(j);
        }
    }

    public void SpawnUiMenus_()
    {
        for (int i = 0; i < Bricks.Count; i++)
        {
            Destroy(Bricks[i].gameObject);
        }
        Bricks.Clear();
        BuildingIcon.sprite = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BuildingIcon;
        BuildingIcon.SetNativeSize();
        for (int i = 0; i < DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos.Count; i++)
        {

            // Check if BrickColorInfos has any elements
            if (DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos.Any())
            {
                // Check if any BrickInfoUI in Bricks list has a BrickType
                if (Bricks.Any(b => b.BrickType != null))
                {
                    Debug.Log("Brick types exist in both BrickColorInfos and Bricks list");
                }
                else
                {
                    Debug.Log("BrickColorInfos has data but no BrickTypes found in Bricks list");
                }
            }
            else
            {
                Debug.Log("No brick data found in BrickColorInfos");
            }



            Debug.Log("SpawnUiMenus__A" + i);
            BrickInfoUI brick = Instantiate(brickPrefab, ParentObject);
            Bricks.Add(brick);
            brick.Initialize(DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos[i].BrickType,
                DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos[i].TotalBricksPerColor);
        }
    }
}
