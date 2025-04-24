using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BricksMenuManger : MonoBehaviour
{
    public Image BuildingIcon;

    public BrickInfoUI brickPrefab;
    public Transform ParentObject;

    [Button(ButtonSizes.Medium)]
    private void SpawnUiMenus()
    {
        for (int i = 0; i < DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos.Count; i++)
        {
            BrickInfoUI brick = Instantiate(brickPrefab, ParentObject);
            brick.Initialize(DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos[i].BrickType,
                DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos[i].TotalBricksPerColor);
        }
    }
}
