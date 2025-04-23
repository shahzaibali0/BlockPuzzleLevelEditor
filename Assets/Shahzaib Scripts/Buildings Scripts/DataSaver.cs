using PregnantMother;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{

    #region Singleton

    public static DataSaver Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion


    private void Start()
    {
        LoadData();
    }

    #region User bricks Data Saver

    public UserData UserData;
    [Button(ButtonSizes.Medium)]
    public void SaveData()
    {
        UserBricksManager userBricksManager = UserBricksManager.instance;

        // Clear previously saved bricks
        UserData.BrickDatas.Clear();

        for (int i = 0; i < userBricksManager.UserBrickDatas.Count; i++)
        {
            BrickType brickType = userBricksManager.UserBrickDatas[i].BrickType;
            int brickCount = userBricksManager.UserBrickDatas[i].UserTotalBrick;

            UserData.BrickDatas.Add(new UserBrickData(brickType, brickCount));
        }

        // Save updated data
        FileHandler.SaveToJSON<UserData>(UserData, "UserBrickDatas");
    }



    [Button(ButtonSizes.Medium)]
    public void LoadData()
    {
        UserBricksManager userBricksManager = UserBricksManager.instance;

        // Clear previous saved data
        UserData.BrickDatas.Clear();
        userBricksManager.UserBrickDatas.Clear();

        // Check if file exists first
        if (FileHandler.IsFileExist("UserBrickDatas"))
        {
            UserData loadedData = FileHandler.ReadFromJSON<UserData>("UserBrickDatas");
            if (loadedData != null)
            {
                UserData = loadedData;

                for (int i = 0; i < UserData.BrickDatas.Count; i++)
                {
                    UserBrickData userBrickData = new UserBrickData(
                        UserData.BrickDatas[i].BrickType,
                        UserData.BrickDatas[i].UserTotalBrick
                    );
                    userBricksManager.UserBrickDatas.Add(userBrickData);
                }
            }
        }
        else
        {
            // If file doesn't exist, create new empty user data
            UserData = new UserData();
        }
    }

    #endregion
}

[Serializable]
public class UserData
{
    public List<UserBrickData> BrickDatas = new List<UserBrickData>();

}
