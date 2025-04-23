using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsMainManger : MonoBehaviour
{

    #region Singleton
    public static BuildingsMainManger Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public GameObject Obj;

    [Button(ButtonSizes.Medium)]
    public void SpawnCurrentBuilding()
    {

        AllBuildingsData buildingsData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        if (buildingsData.BuildingNumber == DataManager.BuildingNo)
        {
            Obj = Instantiate(buildingsData.BuildingPrefab.gameObject, transform);
            Obj.transform.localPosition = Vector3.zero;
        }

    }
}
