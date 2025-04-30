using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseInfo : MonoBehaviour
{
    public Slider FillBar;
    public int Index;
    public Image Icon;
    public TextMeshProUGUI HouseName, Progress, comingsoon;
    public GameObject ComingSoon;

    private void OnEnable()
    {
    }

    private void Start()
    {
        GetBuildData();

    }

    private void GetBuildData()
    {
        HouseName.text = DataManager.Instance.buildingsData.buildingNames[Index].Name;
        Icon.sprite = DataManager.Instance.buildingsData.buildingNames[Index].Icon;
    }


    private void Update()
    {
        Getprogress();
    }
    private void Getprogress()
    {
        if (DataManager.Instance.buildingsData.allBuildingsDatas.Count > Index)
        {
            ComingSoon.SetActive(false);
            float TotalBricks = DataManager.Instance.buildingsData.allBuildingsDatas[Index].TotalBuildingBricks;
            float Remaining = DataManager.Instance.buildingsData.allBuildingsDatas[Index].BuildingRemainingBrick;
            float FillbarValue = 1f - (float)Remaining / TotalBricks;
            FillBar.value = FillbarValue;

            float percentage = FillbarValue * 100f;
            Progress.text = $"Progress {percentage:F0}%";

        }
        else
        {
            Progress.text = $"Progress {0:F0}%";
            FillBar.value = 0;
            ComingSoon.SetActive(true);
            comingsoon.text = "Locked";
        }
    }

    [Button(ButtonSizes.Medium)]
    public void Get()
    {
        comingsoon = ComingSoon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
