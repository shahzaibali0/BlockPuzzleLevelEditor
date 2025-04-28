using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CanvasManger : MonoBehaviour
{

    #region Singleton
    public static CanvasManger Instance;
    private void Awake()
    {
        Instance = this;
    }


    #endregion

    public RobloxUI_FooterMenu Footer;
    public TransitionEnd CloudsTransition;
    public BricksMenuManger bricksMenuManger;
    public LevelComplete levelComplete;
    public GameObject AbilityBar;
    [Header("Brick Menus Buttons")]
    public Button BrickMenuBtn;
    public Button BrickMenuCloseBtn;
    public GameObject BrickMenu, Blocker;
    public TextMeshProUGUI LevelNo;

    public Button SettingBtn, closeBtn;
    public GameObject SettingMenu;

    public RectTransform GameLogo, LevelBar, Timerbar, StartBtn, BuildingMenu;
    public Button StartGame;

    [Header("Freez Time Section")]
    public Image TimerBG;
    public GameObject SnowObj;
    public TextMeshProUGUI timerText;
    public float duration = 60f;
    private float currentTime;
    private bool isRunning = false;
    public Color YellowColor, SnowColor;
    private void Start()
    {
        Inilize();
    }
    private void Inilize()
    {
        BrickMenuBtn.onClick.AddListener(EnableBrickMenu);
        BrickMenuCloseBtn.onClick.AddListener(DisbaleBrickMenu);
        StartGame.onClick.AddListener(OnGameStart);
        SettingBtn.onClick.AddListener(EnableSettings);
    }

    [Button(ButtonSizes.Medium)]
    public void OnGameStart()
    {
        AnimateInX(BrickMenuBtn.gameObject.GetComponent<RectTransform>(), -21);
        AnimateInY(GameLogo, 400);
        AnimateInY(LevelBar, 300);
        ScaleZoomIn(StartBtn.gameObject, 0);
        ScaleZoomOut(Timerbar.gameObject, 1);
        AbilityBar.SetActive(true);
        Footer.SlideDown();
        EnableClouds();
    }

    #region Clouds In-Out

    public void EnableClouds()
    {
        CloudsTransition.gameObject.SetActive(true);
        CloudsTransition.CloudsIn();
    }

    public void CloudsOut()
    {
        CloudsTransition.gameObject.SetActive(false);
    }

    #endregion

    [Button(ButtonSizes.Medium)]
    public void BackToMainMenu()
    {
        AnimateInX(BrickMenuBtn.gameObject.GetComponent<RectTransform>(), 200);
        AnimateInY(GameLogo, -400);
        AnimateInY(LevelBar, -300);
        ScaleZoomIn(Timerbar.gameObject, 0);
        ScaleZoomOut(StartBtn.gameObject, 1);
        AbilityBar.SetActive(false);
        Footer.SlideUp();

    }

    #region Settings

    private void EnableSettings()
    {
        SettingMenu.SetActive(true);

    }
    private void DisableSettings()
    {
        SettingMenu.SetActive(false);

    }

    #endregion


    public void ShowLevelComplete()
    {
        levelComplete.gameObject.SetActive(true);
    }

    public void AnimateBuildingMenu(float Value)
    {
        AnimateInX(BuildingMenu, Value);
    }
    private void EnableBrickMenu()
    {
        BrickMenu.SetActive(true);
        Blocker.SetActive(true);
        RectTransform Panel = BrickMenu.GetComponent<RectTransform>();
        AnimateInX(Panel, 0);
    }
    private void DisbaleBrickMenu()
    {
        Blocker.SetActive(false);
        RectTransform Panel = BrickMenu.GetComponent<RectTransform>();
        AnimateInX(Panel, -1500);

    }


    public void SetLevelNo()
    {
        LevelNo.text = "Level " + LevelManager.LevelNo.ToString() + 1;
    }
    public void OnTimerFreez()
    {
        ResetTimer();
    }
    private void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0f)
            {
                currentTime = 0f;
            }

            UpdateTimerUI();
        }
        SetLevelNo();
    }
    public void StartPuzzleTimer(float TimerDuration)
    {
        duration = TimerDuration;
        ResetTimer();
        StartTimer();
    }
    public void StartTimer()
    {
        SnowObj.SetActive(false);
        TimerBG.color = YellowColor;
        isRunning = true;
    }
    public IEnumerator UnPauseTimer()
    {
        yield return new WaitForSeconds(10);

        TimerBG.color = YellowColor;
        SnowObj.SetActive(false);
        isRunning = true;
    }
    public void StopTimer()
    {
        TimerBG.color = SnowColor;
        SnowObj.SetActive(true);
        isRunning = false;

        StartCoroutine(UnPauseTimer());
    }

    public void ResetTimer()
    {
        SnowObj.SetActive(false);
        TimerBG.color = YellowColor;
        currentTime = duration;
        UpdateTimerUI();
        isRunning = false;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #region Animations

    public void AnimateInY(RectTransform rectTransform, float Value)
    {
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Value, 0.5f).SetEase(Ease.OutCubic);
    }
    public void AnimateInX(RectTransform rectTransform, float targetValue)
    {
        float expectedX = targetValue;

        if (Mathf.Approximately(rectTransform.anchoredPosition.x, expectedX))
        {
            Debug.Log("Already at target position. No animation needed.");
            return;
        }

        // Otherwise, animate to the target position
        rectTransform.DOAnchorPosX(expectedX, 0.5f).SetEase(Ease.OutCubic);
    }


    public Ease Ease;
    public void ScaleZoomOut(GameObject Object, float Value)
    {
        Object.SetActive(true);
        Object.transform.localScale = Vector3.zero;
        Object.transform.DOScale(Value * 0.25f, 0.25f).SetEase(Ease).OnComplete(() => Object.transform.DOScale(Value, 0.25f));
    }

    public void ScaleZoomIn(GameObject Object, float Value)
    {
        Object.SetActive(true);
        Object.transform.DOScale(1 * 0.25f, 0.25f).SetEase(Ease).OnComplete(() => Object.transform.DOScale(Value, 0.25f));
    }

    #endregion@E
}
