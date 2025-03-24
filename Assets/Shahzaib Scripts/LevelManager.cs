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

        LevelInfo = Instantiate(Levels[curlvl], transform);
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
}
