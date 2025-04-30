using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<LevelInfo> Levels = new List<LevelInfo>();
    public LevelInfo LevelInfo;
    public GameObject PuzzelLevelCam;


    private const string _LevelNo = "LevelNo";

    public static int LevelNo
    {
        get => PlayerPrefs.GetInt(_LevelNo, 0); // Default is 0 if not set
        set
        {
            PlayerPrefs.SetInt(_LevelNo, value);
            PlayerPrefs.Save(); // Optional but ensures immediate save
        }
    }


    public void SpaawnLevel()
    {

        if (DataSaver.Instance != null)
            DataSaver.Instance.SaveData();

        if (LevelInfo != null)
        {
            Destroy(LevelInfo.gameObject);
        }

        if (BuildingsMainManger.Instance.Obj != null)
        {

            Destroy(BuildingsMainManger.Instance.Obj);
        }

        if (UserBricksManager.instance.IsBuildingEnable())
        {


            BuildingsMainManger.Instance.SpawnCurrentBuilding();
            Debug.Log("My nigga can Upgrade Building");
        }
        else
        {
            Debug.Log("Nigga Play Some more levels");
            StartCoroutine(EnableRespectiveCam(0.25f, true, false));
            if (LevelInfo != null)
            {
                Destroy(LevelInfo.gameObject);
            }

            LevelInfo = Instantiate(Levels[LevelNo], transform);
            CanvasManger.Instance.CloudsOut();

        }
        CanvasManger.Instance.bricksMenuManger.SpawnUiMenus();
        CanvasManger.Instance.bricksBoxMenuManger.SpawnBox();


    }

    [Button(ButtonSizes.Medium)]
    public void StartNextLevel()
    {
        LevelNo++;

        if (LevelNo >= Levels.Count)
        {

            LevelNo = 0;
        }


        SpaawnLevel();
    }

    public void MoveToMainMenu()
    {
        if (DataSaver.Instance != null)
            DataSaver.Instance.SaveData();

        if (LevelInfo != null)
        {
            Destroy(LevelInfo.gameObject);
        }

        if (BuildingsMainManger.Instance.Obj != null)
        {

            Destroy(BuildingsMainManger.Instance.Obj);
        }
        StartCoroutine(EnableRespectiveCam(0.25f, true, false));
        CanvasManger.Instance.CloudsOut();

 
    }
    public IEnumerator EnableRespectiveCam(float timer, bool PuzzelLevelCamStatus, bool buildCamStatus)
    {
        yield return new WaitForSeconds(timer);

        PuzzelLevelCam.SetActive(PuzzelLevelCamStatus);
        if (BuildingsMainManger.Instance.Obj != null)
            BuildingsMainManger.Instance.Obj.GetComponent<BuildingDataCollector>().BuildingCam.SetActive(buildCamStatus);
    }
}
