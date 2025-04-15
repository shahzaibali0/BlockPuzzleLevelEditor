using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    private void Awake()
    {
        instance = this;
    }


    public string BuildingPref;
    public List<Building_Info> building_Infos = new List<Building_Info>();
    public List<Ornaments_Info> ornaments_Infos = new List<Ornaments_Info>();

    public int CurrentBuildingStep = 0;

    public int CurrentRawmaterialStep = 0;
    public int CurrentOrnamentsStep = 0;

    public bool IsBuilding = false;
    public bool IsOrnamentBuilding = false;


    public GameObject OutterEnvironment;
    public int TotalBricksInBuilding;
    public Vector3 CameraEndPos;
    public Transform MyParentObj;


    public int BuildingInfoNumber;
    public int BuildingInfo_BrickNumber;

    public int TotalBrickActive;

    [HideInInspector] public bool ShowWinPanel;
    private int GetAppliedBuilding_InfosCount()
    {

        return PlayerPrefs.GetInt(BuildingManager.instance.BuildingPref + "_" + "Currentbuilding_Infos", -1);
    }

    private void SetAppliedBuilding_InfosCount(int Building_InfosCount)
    {

        PlayerPrefs.SetInt(BuildingManager.instance.BuildingPref + "_" + "Currentbuilding_Infos", Building_InfosCount);


    }



    [Button(ButtonSizes.Medium)]
    public void Gether_Buidinginfo_Data()
    {
        building_Infos.Clear();
        ornaments_Infos.Clear();

        CurrentBuildingStep = 0;
        CurrentOrnamentsStep = 0;


        Building_Info[] buildingInfos = transform.GetComponentsInChildren<Building_Info>();

        foreach (var item in buildingInfos)
        {
            building_Infos.Add(item);
        }


        Ornaments_Info[] ornaments_Info = transform.GetComponentsInChildren<Ornaments_Info>();

        foreach (var item in ornaments_Info)
        {
            ornaments_Infos.Add(item);
        }
    }

    [Button(ButtonSizes.Medium)]
    public void TotalNumberOfBrickInBuilding()
    {
        TotalBricksInBuilding = 0;

        for (int i = 0; i < building_Infos.Count; i++)
        {
            TotalBricksInBuilding += building_Infos[i].GetComponent<Building_Info>().Objs.Count;
        }
    }
    [Button(ButtonSizes.Medium)]
    public void startBuilding()
    {
        IsBuilding = true;
    }

    [Button(ButtonSizes.Medium)]
    public void startOrnamentBuilding()
    {
        IsOrnamentBuilding = true;
    }

    [Button(ButtonSizes.Medium)]
    public void OnBuilding()
    {
        for (int i = 0; i < building_Infos.Count; i++)
        {
            building_Infos[i].GetComponent<Building_Info>().ActivateAll();
        }
    }

    [Button(ButtonSizes.Medium)]
    public void OFFBuilding()
    {
        for (int i = 0; i < building_Infos.Count; i++)
        {
            building_Infos[i].GetComponent<Building_Info>().DeactivateAll();
        }
    }


    [Button(ButtonSizes.Medium)]
    public void OnCurrenBuildingInfo()
    {
        //PlayerPrefs.DeleteAll();


        float T1 = Time.realtimeSinceStartup;
        Debug.Log("StartTime_ForBuilding: " + T1);

        foreach (var item in building_Infos)
        {
            item.currentActive = 0;
            item.DeactivateAll();
        }

        if (BuildingInfoNumber >= building_Infos.Count)
        {
            BuildingInfoNumber = building_Infos.Count - 1;
        }

        for (int i = 0; i < BuildingInfoNumber; i++)
        {
            Building_Info building_Info = building_Infos[i];

            building_Info.currentActive = 0;

            for (int j = 0; j < building_Info.Objs.Count; j++)
            {
                building_Info.ForceActivate_Next();

            }
        }


        Building_Info building_Info1 = building_Infos[BuildingInfoNumber];

        if (BuildingInfo_BrickNumber >= building_Info1.Objs.Count)
        {
            BuildingInfo_BrickNumber = building_Info1.Objs.Count - 1;
        }

        building_Info1.currentActive = 0;


        for (int j = 0; j < BuildingInfo_BrickNumber; j++)
        {
            building_Info1.ForceActivate_Next();
        }

        float T2 = Time.realtimeSinceStartup;

        Debug.Log("TotalTIme_ForBuilding: " + (T2 - T1));

    }




    public void BuilingStepComplete_CallBack()
    {
        if (CurrentBuildingStep < building_Infos.Count)
        {
            CurrentBuildingStep++;

            if (CurrentBuildingStep >= building_Infos.Count)
            {
                CurrentBuildingStep -= 1;
                SetAppliedBuilding_InfosCount(CurrentBuildingStep);

                IsBuilding = false;
                Debug.Log("BuildingIsCompleted");
                PlayerPrefs.DeleteKey(BuildingManager.instance.BuildingPref + "_" + "Currentbuilding_Infos");
                PlayerPrefs.DeleteKey(BuildingManager.instance.BuildingPref + "_" + "CurrentBrick");
            }
            else
            {
                SetAppliedBuilding_InfosCount(CurrentBuildingStep);

            }

        }

    }

    public void ForceBuilding()
    {
        if (CurrentBuildingStep < building_Infos.Count - 1)
        {
            CurrentBuildingStep++;
            SetAppliedBuilding_InfosCount(CurrentBuildingStep);
        }
        else
        {
            IsBuilding = false;
        }
    }

    public void OrnamentsBuilingStepComplete_CallBack()
    {
        if (CurrentOrnamentsStep < ornaments_Infos.Count - 1)
        {
            CurrentOrnamentsStep++;
        }
        else
        {
            IsOrnamentBuilding = false;
            ShowWinPanel = true;
        }
    }

    public Building_Info get_CurrentBuildingInfo()
    {
        return building_Infos[CurrentBuildingStep];
    }


    public void BuildTheBuilding()
    {
        building_Infos[CurrentBuildingStep].Activate_Next(BuilingStepComplete_CallBack);

    }

    public Transform getCurrentBrick()
    {

  

        return building_Infos[CurrentBuildingStep].GetNextBrickPos();
    }

    public Transform getCurrentBrick_Increment()
    {



        return building_Infos[CurrentBuildingStep].GetNextBrickPos_Increment();
    }

    public Material GetCurrentBrickMat()
    {


        return building_Infos[CurrentBuildingStep].GetBrickMat();


    }

    public void InilizeBuilding()
    {
        BuildingInfoNumber = GetAppliedBuilding_InfosCount();
        if (BuildingInfoNumber == -1)
        {
            SetAppliedBuilding_InfosCount(0);
            return;
        }
        //Debug.Log("BuildingInfoNumber_" + BuildingInfoNumber);
        if (BuildingInfoNumber >= building_Infos.Count)
        {
            BuildingInfoNumber = building_Infos.Count - 1;
        }

        CurrentBuildingStep = BuildingInfoNumber;

        //for (int i = 0; i < BuildingInfoNumber - 1; i++)
        //{
        //    Building_Info building_Info = building_Infos[i];
        //    Debug.Log("building_Info" + building_Infos.Count);
        //    for (int j = 0; j < building_Info.Objs.Count; j++)
        //    {
        //        building_Info.ForceActivate_Next();
        //        Debug.Log("building_Info.Objs" + building_Info.Objs.Count);
        //    }
        //}



        Building_Info building_Info1 = building_Infos[BuildingInfo_BrickNumber];
        BuildingInfo_BrickNumber = building_Info1.GetAppliedBrickCount();




        if (BuildingInfo_BrickNumber >= building_Info1.Objs.Count)
        {
            BuildingInfo_BrickNumber = building_Info1.Objs.Count - 1;
        }

        OnCurrenBuildingInfo();

        TotalBrickActive = BuildingInfo_BrickNumber;


        //for (int j = 0; j < BuildingInfo_BrickNumber; j++)
        //{
        //    building_Info1.ForceActivate_Next();
        //}



    }



    private void FixedUpdate()
    {
        if (IsBuilding)
        {
            building_Infos[CurrentBuildingStep].Activate_Next(BuilingStepComplete_CallBack);
        }

        if (IsOrnamentBuilding)
        {
            ornaments_Infos[CurrentOrnamentsStep].Activate_Next(OrnamentsBuilingStepComplete_CallBack);
        }
    }

    public void getRequiredInfo(out int _count, out RawMaterial mat)
    {
        if (CurrentRawmaterialStep >= building_Infos.Count)
        {
            _count = 0;
            mat = RawMaterial.BrickGrey;

        }
        else
        {
            _count = building_Infos[CurrentRawmaterialStep].Objs.Count;
            mat = building_Infos[CurrentRawmaterialStep].rawMaterial;
            CurrentRawmaterialStep++;
        }
    }

    public void MoveableBuilding()
    {
        Transform brick = BuildingManager.instance.getCurrentBrick();

    
    }

    void AdujestBuildingPos()
    {
        MyParentObj.transform.DOMoveY(MyParentObj.transform.position.y - 3, 2);
    }

}
