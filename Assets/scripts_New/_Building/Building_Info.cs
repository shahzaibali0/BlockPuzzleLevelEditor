using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using DG.Tweening;


public class Building_Info : MonoBehaviour
{

    public RawMaterial rawMaterial;

    public List<BuildingSinglePiece> Objs = new List<BuildingSinglePiece>();
    public int currentActive = 0;

    public bool IsBuilding = false;


    public int GetAppliedBrickCount()
    {
      
        return PlayerPrefs.GetInt(BuildingManager.instance.BuildingPref + "_" + "CurrentBrick", 0);

    }

    private void SetAppliedBrickCount(int BrickCount)
    {

        PlayerPrefs.SetInt(BuildingManager.instance.BuildingPref + "_" + "CurrentBrick", BrickCount);
 

    }

    bool isCompleted = false;
    public void Activate_Next(UnityAction comp_CallBack = null)
    {

        if (isCompleted) return;

        GameObject CurrentObj = Objs[currentActive].gameObject;
        CurrentObj.SetActive(true);

       


        if (currentActive < Objs.Count)
        {
            currentActive++;

            if (currentActive >= Objs.Count)
            {
                currentActive -= 1;
                isCompleted = true;
                IsBuilding = false;


                Debug.Log("Building Step Completed.");
                if (comp_CallBack != null)
                {
                    comp_CallBack.Invoke();
                }
            }


            SetAppliedBrickCount(currentActive);


            CurrentObj.transform.DOPunchPosition(Vector3.up * 0.25f, 0.25f).OnComplete(() => CurrentObj.transform.DOPunchScale(Vector3.one * 1.5f, 0.25f));

        }



        if (currentActive >= Objs.Count)
        {


        }

    }
    public void ForceActivate_Next()
    {


        if (currentActive < Objs.Count)
        {
            GameObject CurrentObj = Objs[currentActive].gameObject;
            CurrentObj.SetActive(true);

            currentActive++;
            CurrentPosCount = currentActive;
        }
        else
        {

            IsBuilding = false;
        }

    }


    public int CurrentPosCount = 0;
    public Transform GetNextBrickPos()
    {

        return Objs[CurrentPosCount].transform;
    }

    public Transform GetNextBrickPos_Increment()
    {
        Transform dummyTran = Objs[CurrentPosCount].transform;


        CurrentPosCount++;

        if (CurrentPosCount >= Objs.Count - 1)
        {
            CurrentPosCount = Objs.Count - 1;
            //dummyTran = null;
        }


        return dummyTran;
    }

    public Material GetBrickMat()
    {
       
        return Objs[currentActive].GetComponent<Renderer>().material;

    }

    public int Diff_In_Counter()
    {
        int diff = (Objs.Count - 1) - currentActive;

        return diff;

    }

    public Transform GetNextPos()
    {
        Transform pos = Objs[currentActive].gameObject.transform;
        //currentActive++;
        return pos;
    }

    [Button(ButtonSizes.Medium)]
    public void Update_Data()
    {
        Objs.Clear();
        currentActive = 0;

        BuildingSinglePiece[] singlePieces = transform.GetComponentsInChildren<BuildingSinglePiece>(true);

        foreach (var item in singlePieces)
        {
            Objs.Add(item);
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

    [Button(ButtonSizes.Medium)]
    public void DeactivateAll()
    {
        foreach (var item in Objs)
        {
            item.gameObject.SetActive(false);
        }
    }


    public void StartBuilding()
    {
        IsBuilding = true;
    }

}

