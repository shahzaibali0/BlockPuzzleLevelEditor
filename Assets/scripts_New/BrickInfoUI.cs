using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrickInfoUI : MonoBehaviour
{

    public BrickType BrickType;
    public Image Icon;
    public Slider Fillbar;
    public TextMeshProUGUI TotalBricksText;
    public TextMeshProUGUI RemainingBricksText, BrickName;

    private int _totalBricks;
    private int _remainingBricks;
    private bool IsTrue;

    private void Update()
    {
        if (IsTrue)
        {
            GetData();
        }
    }

    public void Initialize(BrickType type, int totalBricks)
    {
        BrickType = type;
        BrickName.text = BrickType.ToString() + " " + "Brick";

        for (int j = 0; j < DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos.Count;)
        {
            BrickInfo brickInfo = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos.FirstOrDefault(x => x.BrickType == BrickType);
            Icon.sprite = brickInfo.Icon;
            break;
        }

        _totalBricks = totalBricks;
        UserBrickData userBrickData = UserBricksManager.instance.UserBrickDatas.FirstOrDefault(x => x.BrickType == BrickType);
        _remainingBricks = userBrickData.UserTotalBrick;


        UpdateUI();
        IsTrue = true;
    }

    private void UpdateUI()
    {
        Fillbar.value = ((float)_remainingBricks / _totalBricks);

        // Update text displays
        TotalBricksText.text = _totalBricks.ToString();
        RemainingBricksText.text = _remainingBricks.ToString();
    }
    public void GetData()
    {
        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        var brickInfo = buildingData.BrickColorInfos.Find(x => x.BrickType == this.BrickType);

        if (brickInfo != null)
        {
            _totalBricks = brickInfo.TotalBricksPerColor;
            UserBrickData userBrickData = UserBricksManager.instance.UserBrickDatas.FirstOrDefault(x => x.BrickType == BrickType);
            _remainingBricks = userBrickData.UserTotalBrick;
            UpdateUI();
        }
    }

}
