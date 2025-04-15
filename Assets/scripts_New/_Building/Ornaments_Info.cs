using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using DG.Tweening;

public class Ornaments_Info : MonoBehaviour
{
    public List<GameObject> Objs = new List<GameObject>();
    public int currentActive = 0;

    public bool IsBuilding = false;

    public void Activate_Next(UnityAction comp_CallBack)
    {
        GameObject CurrentObj = Objs[currentActive];

        CurrentObj.SetActive(true);
        Vector3 currentScale = CurrentObj.transform.localScale;

        CurrentObj.transform.DOPunchScale(currentScale + Vector3.one * currentScale.magnitude / 10, 0.5f);
        //CurrentObj.transform.DOPunchPosition(Vector3.up * 0.25f, 0.25f).OnComplete(() => CurrentObj.transform.DOPunchScale(Vector3.one * 1.5f, 0.25f));

        if (currentActive < Objs.Count - 1)
        {
            currentActive++;
        }
        else
        {
            if (comp_CallBack != null)
            {
                comp_CallBack.Invoke();
            }
            IsBuilding = false;
        }
    }


   

    [Button(ButtonSizes.Medium)]
    public void Update_Data()
    {
        Objs.Clear();
        currentActive = 0;

        BuildingSinglePiece[] buildingSinglePieces = transform.GetComponentsInChildren<BuildingSinglePiece>(true);

        foreach (var item in buildingSinglePieces)
        {
            Objs.Add(item.gameObject);
            item.gameObject.SetActive(false);
        }
    }

    [Button(ButtonSizes.Medium)]
    public void ActivateAll()
    {
        foreach (var item in Objs)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void StartBuilding()
    {
        IsBuilding = true;
    }

    //private void FixedUpdate()
    //{
    //    if(IsBuilding)
    //    {
    //        Activate_Next();
    //    }
    //}

}
