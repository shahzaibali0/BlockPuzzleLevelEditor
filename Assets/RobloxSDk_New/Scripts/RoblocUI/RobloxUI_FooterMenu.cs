using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;



public class RobloxUI_FooterMenu : MonoBehaviour
{



    [SerializeField]
    private Button ProfileBtn,
        BuildingBtn,
        HomeBtn,
        RewardBtn,
        SettingsBtn;

    [SerializeField]
    GameObject RobloxRewardAmountObj;

    [SerializeField]
    TextMeshProUGUI RobloxRewardAmountTxt;

    [SerializeField]
    Sprite UnSelected,
        Selected;

    [SerializeField]
    float AnimSpeed;

    [SerializeField]
    Image CheckoutBtnIcon;

    [SerializeField]
    Color LockedColor,
        UnLockedColor;

    private void Start()
    {
        InitializeAllButtons();
    }

    private void OnEnable()
    {
        CheckoutBtn_checkUnlockStatus();
    }

    [Button(ButtonSizes.Medium)]
    public void SlideDown()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 196f, 0.5f).SetEase(Ease.OutCubic);
    }
    [Button(ButtonSizes.Medium)]
    public void SlideUp()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 196, 0.5f).SetEase(Ease.OutCubic);
    }
    private void MarlAllUnselected()
    {
        ProfileBtn.GetComponent<Image>().sprite = UnSelected;
        ProfileBtn.GetComponent<Image>().SetNativeSize();
        BuildingBtn.GetComponent<Image>().sprite = UnSelected;
        BuildingBtn.GetComponent<Image>().SetNativeSize();
        HomeBtn.GetComponent<Image>().sprite = UnSelected;
        HomeBtn.GetComponent<Image>().SetNativeSize();
        RewardBtn.GetComponent<Image>().sprite = UnSelected;
        RewardBtn.GetComponent<Image>().SetNativeSize();
        SettingsBtn.GetComponent<Image>().sprite = UnSelected;
        SettingsBtn.GetComponent<Image>().SetNativeSize();


    }

    private void MarkAsSelected(Button button)
    {
        button.GetComponent<Image>().sprite = Selected;
        button.GetComponent<Image>().SetNativeSize();
        //StartCoroutine(ScaleUp(button.GetComponent<RectTransform>()));
    }

    IEnumerator ScaleUp(RectTransform rectTransform)
    {
        float T = 0;
        Vector2 Delta = rectTransform.sizeDelta;
        while (T <= 1)
        {
            T += Time.deltaTime / AnimSpeed;

            Delta.x = Mathf.Lerp(180, 357f, T);

            rectTransform.sizeDelta = Delta;
            yield return null;
        }
    }

    private void InitializeAllButtons()
    {
        ProfileBtn.onClick.AddListener(ProfileBtn_Clicked);
        BuildingBtn.onClick.AddListener(buildingBtn_Clicked);
        HomeBtn.onClick.AddListener(HomeBtn_Clicked);
        RewardBtn.onClick.AddListener(RewardBtn_Clicked);
        SettingsBtn.onClick.AddListener(SettingsBtn_Clicked);



    }

    public void checkForUnlockStatus() { }

    private void ProfileBtn_Clicked()
    {
        MarlAllUnselected();
        MarkAsSelected(ProfileBtn);
    }

    public void buildingBtn_Clicked()
    {
        if (CheckoutBtn_checkUnlockStatus())
        {
            Debug.Log("buildingBtn_enable");

            CanvasManger.Instance.AnimateBuildingMenu(0);
            MarlAllUnselected();
            MarkAsSelected(BuildingBtn);
        }
        else
        {
            Debug.Log("buildingBtn_Disable");

        }
    }

    private bool CheckoutBtn_checkUnlockStatus()
    {
        return true;
    }

    public void HomeBtn_Clicked()
    {
        Debug.Log("HomeBtn_enable");
        CanvasManger.Instance.AnimateBuildingMenu(-1500);

        MarlAllUnselected();
        MarkAsSelected(HomeBtn);
    }

    public void RewardBtn_Clicked()
    {
        MarlAllUnselected();
        //RobloxUIManager.UpdateRobloxScreen(
        //    RobloxScreen.Reward,
        //    RobloxScreenOverlayType.Overlay
        //);
        MarkAsSelected(RewardBtn);
    }

    private void SettingsBtn_Clicked()
    {
        MarlAllUnselected();
        MarkAsSelected(SettingsBtn);
    }
}
