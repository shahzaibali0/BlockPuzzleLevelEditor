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
    public Image FillBar2;
    public TextMeshProUGUI TotalBricksText;
    public TextMeshProUGUI RemainingBricksText, BrickName;

    private int _totalBricks;
    private int _remainingBricks;
    private int _currentVisibleBricks; // this will animate
    private bool IsTrue;
    private Coroutine animateCoroutine;

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
        if (BrickName != null)
            BrickName.text = BrickType + " Brick";

        var brickInfo = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo]
            .BrickColorInfos.FirstOrDefault(x => x.BrickType == BrickType);

        if (brickInfo != null)
        {
            Icon.sprite = brickInfo.Icon;
        }

        _totalBricks = totalBricks;
        _remainingBricks = UserBricksManager.instance.UserBrickDatas
            .FirstOrDefault(x => x.BrickType == BrickType)?.UserTotalBrick ?? 0;

        _currentVisibleBricks = _remainingBricks;
        UpdateUI(_currentVisibleBricks);
        IsTrue = true;
    }

    private void UpdateUI(int displayValue)
    {
        float fillAmount = (float)displayValue / _totalBricks;

        if (Fillbar != null)
            Fillbar.value = fillAmount;
        else if (FillBar2 != null)
            FillBar2.fillAmount = fillAmount;

        TotalBricksText.text = _totalBricks.ToString();
        RemainingBricksText.text = displayValue.ToString();
    }

    public void GetData()
    {
        var buildingData = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo];
        var brickInfo = buildingData.BrickColorInfos.Find(x => x.BrickType == BrickType);

        if (brickInfo != null)
        {
            _totalBricks = brickInfo.TotalBricksPerColor;
            int newRemaining = UserBricksManager.instance.UserBrickDatas
                .FirstOrDefault(x => x.BrickType == BrickType)?.UserTotalBrick ?? 0;

            if (animateCoroutine != null)
                StopCoroutine(animateCoroutine);

            animateCoroutine = StartCoroutine(AnimateRemainingBricks(_currentVisibleBricks, newRemaining));
            _remainingBricks = newRemaining;
        }
    }

    private IEnumerator AnimateRemainingBricks(int from, int to)
    {
        int step = from < to ? 5 : -5;
        while ((step > 0 && from < to) || (step < 0 && from > to))
        {
            from += step;

            // Clamp to avoid overshooting the target
            if ((step > 0 && from > to) || (step < 0 && from < to))
                from = to;

            _currentVisibleBricks = from;
            UpdateUI(_currentVisibleBricks);
            yield return new WaitForSeconds(0.0002f); // adjust speed as needed
        }

    }

}
