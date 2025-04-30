using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        CanvasManger.Instance.DisbaleTimer();
        CanvasManger.Instance.SpeedUpBuildingsMenu_(true);
        AllBuildingsData buildingsData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        if (buildingsData.BuildingNumber == DataManager.BuildingNo)
        {
            Obj = Instantiate(buildingsData.BuildingPrefab.gameObject, transform);
            Obj.transform.localPosition = Vector3.zero;
        }
    }

    public void BuildBuilding()
    {
        BuildingManager.instance.Build();
    }

    public void BuildOrnaments(bool Status)
    {
        if (Status)
        {
            BuildingManager.instance.BuildOrnamentBuilding();
        }
    }

    Coroutine Coroutine;

    public void SpeedUpBuild()
    {
        BuildingManager.instance.StartBuilding();
        if (Coroutine != null)
        {

            StopCoroutine(Coroutine);
        }
        Coroutine = StartCoroutine(StopBuilding());
    }

    private IEnumerator StopBuilding()
    {
        yield return new WaitForSeconds(5);
        BuildingManager.instance.StopBuilding();

    }
}
