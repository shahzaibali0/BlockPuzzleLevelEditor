using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FogFeature : MonoBehaviour
{





    [SerializeField] private float NearStartFogValue, NearEndFogValue;

    [SerializeField] private float FarStartFogValue, FarEndFogValue;
    [SerializeField] private Color FogColor;

    private Camera CrusherCamera;

    private void OnEnable()
    {
       // CrusherCamera = LevelController.instance.CrusherCamera;
    }



    private void Start()
    {
      //  FogStartValue();
    }


    void FogStartValue()
    {
        CrusherCamera.backgroundColor = FogColor;
        Fog(true, FogColor, NearStartFogValue, NearEndFogValue);
    }

    public void FogEndAfterCompleteion()
    {
        StartCoroutine(FogEndValue());

    }

    private IEnumerator FogEndValue()
    {
        yield return new WaitForSeconds(2.5f);

        CrusherCamera.backgroundColor = FogColor;
        Fog(true, FogColor, FarStartFogValue, FarEndFogValue);
    }

    public void Fog(bool fogBool, Color FogColor, float NearStartFogValue, float NearEndFogValue)
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = NearStartFogValue;
        RenderSettings.fogEndDistance = NearEndFogValue;

    }
}
