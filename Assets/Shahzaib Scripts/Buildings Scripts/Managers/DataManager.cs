using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public BuildingsData buildingsData;
    private const string BuildingNoKey = "BuildingNo";

    public static int BuildingNo
    {
        get => PlayerPrefs.GetInt(BuildingNoKey, 0); // Default is 0 if not set
        set
        {
            PlayerPrefs.SetInt(BuildingNoKey, value);
            PlayerPrefs.Save(); // Optional but ensures immediate save
        }
    }

}
