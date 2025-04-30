using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserBricksManager : MonoBehaviour
{
    #region Singleton
    public static UserBricksManager instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion

    #region Fields

    public List<UserBrickData> UserBrickDatas = new List<UserBrickData>();
    public List<BrickInfo> CurrentBuildingbricksInfo = new List<BrickInfo>();



    public UserBrickData Currentbricks;
    public BrickType BrickType;
    public int BricksNo;


    private void Start()
    {
        InitializeAllBricks();
    }

    #endregion
    public void InitializeAllBricks()
    {
        foreach (BrickType brickType in Enum.GetValues(typeof(BrickType)))
        {
            if (brickType == BrickType.None)
                continue;

            bool alreadyExists = UserBrickDatas.Any(b => b.BrickType == brickType);
            if (!alreadyExists)
            {
                UserBrickDatas.Add(new UserBrickData(brickType, 0));
            }
        }
        if (DataSaver.Instance != null)
            DataSaver.Instance.SaveData();

    }

    public bool IsBuildingEnable()
    {

        bool IsTrue = false;
        CurrentBuildingbricksInfo.Clear();
        foreach (var item in DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos)
        {
            CurrentBuildingbricksInfo.Add(item);
        }

        foreach (var requiredBrick in CurrentBuildingbricksInfo)
        {
            var userBrick = UserBrickDatas.FirstOrDefault(b => b.BrickType == requiredBrick.BrickType);

            if (userBrick == null || userBrick.UserTotalBrick < requiredBrick.TotalBricksPerColor)
            {
                IsTrue = false;
            }
            else
            {
                IsTrue = true;
            }
        }

        return IsTrue;

    }

    public void SetCurrentBrick(BrickType brickType)
    {
        Currentbricks = UserBrickDatas.FirstOrDefault(b => b.BrickType == brickType);

        if (Currentbricks == null)
        {
            Debug.LogWarning($"No bricks of type {brickType} found in inventory");
        }
    }

    public int DecreseBricks()
    {
        int value = 0;
        if (Currentbricks != null)
        {
            // Decrease count in current brick reference
            if (Currentbricks.UserTotalBrick > 0)
            {

                // Update the count in the main list
                var brickInList = UserBrickDatas.FirstOrDefault(b => b.BrickType == Currentbricks.BrickType);
                if (brickInList != null)
                {
                    value = brickInList.UserTotalBrick--;
                }
            }
            else
            {
                Debug.LogWarning($"No bricks left of type {Currentbricks.BrickType}");
            }
        }
        else
        {
            Debug.LogWarning("No current brick selected");
        }

        return value;
    }
    [Button(ButtonSizes.Medium)]
    public void AddData()
    {
        bricksData(BrickType, BricksNo);
    }

    public void AddBrickonExtrationPoint(BrickType newBrickType, int newBrickCount)
    {
        bricksData(newBrickType, newBrickCount);
    }

    public void bricksData(BrickType newBrickType, int newBrickCount)
    {

        Debug.Log("Shah newBrickCount " + newBrickCount);
        Debug.Log("Shah newBrickType " + newBrickType);

        UserBrickData existingData = UserBrickDatas.FirstOrDefault(data => data.BrickType == newBrickType);

        if (existingData != null)
        {
            existingData.UserTotalBrick += newBrickCount;
        }
        else
        {
            UserBrickData userBrickData = new UserBrickData(newBrickType, newBrickCount);

            Debug.Log("Shah newBrickCount __A" + userBrickData);
            Debug.Log("Shah newBrickType __B" + userBrickData);
            UserBrickDatas.Add(userBrickData);
        }
        if (DataSaver.Instance != null)
            DataSaver.Instance.SaveData();
    }

}
