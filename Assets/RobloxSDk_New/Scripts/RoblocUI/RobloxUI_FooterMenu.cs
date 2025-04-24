using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;



public class RobloxUI_FooterMenu : MonoBehaviour
{
    [SerializeField]
    private Button ProfileBtn,
        CheckoutBtn,
        HomeBtn,
        RewardBtn,
        SettingsBtn,
        DTOfferWallBtn;

    public ScrollRect scrollRect;

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

    private void Update()
    {
    }

    #region Buttons

    private void MarlAllUnselected()
    {
        ProfileBtn.GetComponent<Image>().sprite = UnSelected;
        ProfileBtn.GetComponent<Image>().SetNativeSize();
        CheckoutBtn.GetComponent<Image>().sprite = UnSelected;
        CheckoutBtn.GetComponent<Image>().SetNativeSize();
        HomeBtn.GetComponent<Image>().sprite = UnSelected;
        HomeBtn.GetComponent<Image>().SetNativeSize();
        RewardBtn.GetComponent<Image>().sprite = UnSelected;
        RewardBtn.GetComponent<Image>().SetNativeSize();
        SettingsBtn.GetComponent<Image>().sprite = UnSelected;
        SettingsBtn.GetComponent<Image>().SetNativeSize();
        DTOfferWallBtn.GetComponent<Image>().sprite = UnSelected;
        DTOfferWallBtn.GetComponent<Image>().SetNativeSize();

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
        CheckoutBtn.onClick.AddListener(CheckoutBtn_Clicked);
        HomeBtn.onClick.AddListener(HomeBtn_Clicked);
        RewardBtn.onClick.AddListener(RewardBtn_Clicked);
        SettingsBtn.onClick.AddListener(SettingsBtn_Clicked);

#if RB_DT_OFFERWALL
            DTOfferWallBtn.gameObject.SetActive(false);
            DTOfferWallBtn.onClick.AddListener(DTOfferWallBtn_Clicked);
#else
        DTOfferWallBtn.gameObject.SetActive(false);
#endif
    }

    public void checkForUnlockStatus() { }

    private void ProfileBtn_Clicked()
    {
        MarlAllUnselected();
        //RobloxUIManager.UpdateRobloxScreen(
        //    RobloxScreen.Profile,
        //    RobloxScreenOverlayType.Overlay
        //);
        MarkAsSelected(ProfileBtn);
    }

    public void CheckoutBtn_Clicked()
    {
        if (CheckoutBtn_checkUnlockStatus())
        {
            MarlAllUnselected();
            //RobloxUIManager.UpdateRobloxScreen(
            //    RobloxScreen.CheckOut,
            //    RobloxScreenOverlayType.Overlay
            //);
            MarkAsSelected(CheckoutBtn);
        }
        else
        {
         
        }
    }

    private bool CheckoutBtn_checkUnlockStatus()
    {
        //if (
        //    RobloxTasksManager.instance.robloxData.robloxAdTaskData.robloxTasksAdsCompleted > 0
        //    || RobloxTasksManager
        //        .instance
        //        .robloxData
        //        .robloxLevelTaskData
        //        .robloxTasksLevelsCompleted > 0
        //    || RobloxTasksManager.instance.robloxData.user_data.robux > 0
        //)
        //{
        //    //CheckoutBtn.interactable = true;
        //    CheckoutBtnIcon.color = UnLockedColor;
        //    CheckoutBtn.GetComponent<Image>().color = UnLockedColor;
        //    return true;
        //}
        //else
        //{
        //    //CheckoutBtn.interactable = false;
        //    CheckoutBtnIcon.color = LockedColor;
        //    CheckoutBtn.GetComponent<Image>().color = LockedColor;
        //    return false;
        //}

        return true;
    }

    public void HomeBtn_Clicked()
    {
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
        //RobloxUIManager.UpdateRobloxScreen(
        //    RobloxScreen.Settings,
        //    RobloxScreenOverlayType.Overlay
        //);
        MarkAsSelected(SettingsBtn);
    }

    private static bool oneTime = true;

    private void DTOfferWallBtn_Clicked()
    {
#if RB_DT_OFFERWALL
            RBDebug.Log("DTOfferWallBtn_Clicked");
            MarlAllUnselected();
            // RobloxUIManager.UpdateRobloxScreen(RobloxScreen.Settings, RobloxScreenOverlayType.Overlay);
            // DT_Offerwall_Manager.instance.initiaize_DT();
            DT_Offerwall_Manager.instance.ShowOfferWall_FromCurrentSelectedPlacementID();

            //if (oneTime)
            //{
            //    oneTime = false;
            //    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            //}

            MarkAsSelected(DTOfferWallBtn);
#endif
    }

    #endregion
}

