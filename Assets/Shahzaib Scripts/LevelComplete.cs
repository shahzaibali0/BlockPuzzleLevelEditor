using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    public Button nextBtn;


    private void Start()
    {
        nextBtn.onClick.AddListener(OnLevelComplete);
    }
    public void OnLevelComplete()
    {
        LevelManager.Instance.StartNextLevel();
        gameObject.SetActive(false);

    }
}
