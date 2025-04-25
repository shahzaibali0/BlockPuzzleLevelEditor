using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [Header("Brick Menus Buttons")]
    public Button BrickMenuBtn;
    public Button BrickMenuCloseBtn;
    public GameObject BrickMenu;
    public TextMeshProUGUI LevelNo;

    public Button SettingBtn , closeBtn;
    public GameObject SettingMenu;

    public RectTransform GameLogo, LevelBar, Timerbar, StartBtn;

    public Button StartGame;

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
        AnimateInX(BrickMenuBtn.gameObject.GetComponent<RectTransform>(), -200);
        AnimateInY(GameLogo, 400);
        AnimateInY(LevelBar, 300);
        ScaleZoomIn(StartBtn.gameObject, 0);
        ScaleZoomOut(Timerbar.gameObject, 1);
        Footer.SlideDown();
    }

    [Button(ButtonSizes.Medium)]
    public void BackToMainMenu()
    {
        AnimateInX(BrickMenuBtn.gameObject.GetComponent<RectTransform>(), 200);
        AnimateInY(GameLogo, -400);
        AnimateInY(LevelBar, -300);
        ScaleZoomIn(Timerbar.gameObject, 0);
        ScaleZoomOut(StartBtn.gameObject, 1);
        Footer.SlideUp();

    }

    private void EnableSettings()
    {
        SettingMenu.SetActive(true);

    }
    private void DisableSettings()
    {
        SettingMenu.SetActive(false);

    }

    private void EnableBrickMenu()
    {
        BrickMenu.SetActive(true);
    }
    private void DisbaleBrickMenu()
    {
        BrickMenu.SetActive(false);

    }

    public void SetLevelNo()
    {
        LevelNo.text = "Level " + LevelManager.LevelNo.ToString();
    }

    public void AnimateInY(RectTransform rectTransform, float Value)
    {
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Value, 0.5f).SetEase(Ease.OutCubic);
    }
    public void AnimateInX(RectTransform rectTransform, float Value)
    {
        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + Value, 0.5f).SetEase(Ease.OutCubic);
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
}
