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
    int curlvl = 0;

    private void Start()
    {
        SpaawnLevel();
    }

    private void SpaawnLevel()
    {
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
        

            StartCoroutine(EnableRespectiveCam(1.5f, false, true));

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

            LevelInfo = Instantiate(Levels[curlvl], transform);

        }


    }

    [Button(ButtonSizes.Medium)]
    public void StartNextLevel()
    {
        curlvl++;

        if (curlvl >= Levels.Count)
        {

            curlvl = 0;
        }


        SpaawnLevel();
    }


    public IEnumerator EnableRespectiveCam(float timer, bool PuzzelLevelCamStatus, bool buildCamStatus)
    {
        yield return new WaitForSeconds(timer);

        PuzzelLevelCam.SetActive(PuzzelLevelCamStatus);
        if (BuildingsMainManger.Instance.Obj != null)
            BuildingsMainManger.Instance.Obj.GetComponent<BuildingDataCollector>().BuildingCam.SetActive(buildCamStatus);
    }
}
