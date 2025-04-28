using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleAbilities : MonoBehaviour
{
    public AbilityType abilityType;
    public Button MyBtn, OnRewardAd;
    public GameObject TotalNumber;
    public TextMeshProUGUI TotalTriesTxt;
    public int TotalTries;
    public string AbilityPrefs;
    private bool SetOnrwd;

    #region Unity Methods
    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey(AbilityPrefs))
        {
            PlayerPrefs.SetInt(AbilityPrefs, 0);
        }
        else
        {
            TotalTries = PlayerPrefs.GetInt(AbilityPrefs);
        }
        TotalTries = PlayerPrefs.GetInt(AbilityPrefs);
        TotalTriesTxt.text = TotalTries.ToString();
    }
    private void Update()
    {
        if (TotalTries <= 0)
        {
            SetOnrwd = true;
            OnRewardAd.gameObject.SetActive(true);
            TotalNumber.gameObject.SetActive(false);
        }
        else
        {
            OnRewardAd.gameObject.SetActive(false);
            TotalNumber.gameObject.SetActive(true);
            TotalTriesTxt.text = TotalTries.ToString();
            SetOnrwd = false;
        }
    }
    private void Start()
    {
        //MyBtn.onClick.AddListener(OnAbilitySelection);
    }
    #endregion

    public void OnAbilitySelection()
    {
        if (abilityType == AbilityType.FreezTime)
        {
            Debug.Log("Ability");
            OnTimerFreez();
        }

        if (abilityType == AbilityType.Bomb)
        {
            OnBombExplosion();
        }
    }
    #region Timer Freez


    private void OnTimerFreez()
    {
        Debug.Log("Ability__");

        if (SetOnrwd)
        {
            Debug.Log("Ability__A");

            GetTimerOnReward();
        }
        else
        {
            UserTimerAbility();
        }
    }

    private void UserTimerAbility()
    {
        TotalTries--;
        PlayerPrefs.SetInt(AbilityPrefs, TotalTries);
        CanvasManger.Instance.StopTimer();
    }
    [Button(ButtonSizes.Medium)]
    private void GetTimerOnReward()
    {

        Debug.Log("Ability__B");

        OnRewardGranted(true);
    }

    private void OnRewardGranted(bool IsTrue)
    {
        Debug.Log("Ability__C");

        if (IsTrue)
        {
            Debug.Log("Ability__D");

            if (abilityType == AbilityType.FreezTime)
            {
                Debug.Log("Ability__E");

                TotalTries++;
                PlayerPrefs.SetInt(AbilityPrefs, TotalTries);

            }
            else if (abilityType == AbilityType.Bomb)
            {
                TotalTries++;
                PlayerPrefs.SetInt(AbilityPrefs, TotalTries);
            }
        }
    }

    #endregion
    #region Bomb Explosion

    private void OnBombExplosion()
    {
        if (SetOnrwd)
        {
            GetBombOnReward();
        }
        else
        {
            UseBombAbility();
        }
    }
    private void GetBombOnReward()
    {
        OnRewardGranted(true);

    }

    private void UseBombAbility()
    {


    }
    #endregion
}

[Serializable]
public enum AbilityType
{
    FreezTime,
    Bomb
}