using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrickInfoUI : MonoBehaviour
{

    public BrickType BrickType;
    public Image Icon;
    public Slider Fillbar;
    public TextMeshProUGUI TotalBricksText;
    public TextMeshProUGUI RemainingBricksText;

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

    public void Initialize(BrickType type/*, Sprite icon*/, int totalBricks)
    {
        BrickType = type;
        //Icon.sprite = icon;
        _totalBricks = totalBricks;
        _remainingBricks = totalBricks;

        UpdateUI();
        IsTrue = true;
    }

    public void BrickPlaced()
    {
        if (_remainingBricks > 0)
        {
            _remainingBricks--;
            UpdateUI();

            UpdateDataManager();
        }
    }

    private void UpdateUI()
    {
        Fillbar.value = 1f - ((float)_remainingBricks / _totalBricks);

        // Update text displays
        TotalBricksText.text = _totalBricks.ToString();
        RemainingBricksText.text = _remainingBricks.ToString();
    }

    private void UpdateDataManager()
    {
        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];

        var brickInfo = buildingData.BrickColorInfos.Find(x => x.BrickType == this.BrickType);
        if (brickInfo != null)
        {
            brickInfo.RemaingBricksPerColor = _remainingBricks;
            brickInfo.TotalBricksPerColor = _totalBricks;
        }
    }

    public void GetData()
    {
        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        var brickInfo = buildingData.BrickColorInfos.Find(x => x.BrickType == this.BrickType);

        if (brickInfo != null)
        {
            _totalBricks = brickInfo.TotalBricksPerColor;
            _remainingBricks = brickInfo.RemaingBricksPerColor;
            UpdateUI();
        }
    }

}
