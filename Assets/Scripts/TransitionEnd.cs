using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionEnd : MonoBehaviour
{
    public AudioSource CurtainOpen;
    public AudioSource CurtainClose;
    public Animator Clouds;

    [Button(ButtonSizes.Medium)]
    public void CloudsIn()
    {
        Clouds.Play("CloudsIn");
    }

    [Button(ButtonSizes.Medium)]
    public void CloudsOut()
    {
        Clouds.Play("CloudsOut");
        OnLevelSpawn();
    }
    private void OnLevelSpawn()
    {

    }
    public void markTransitionEnd()
    {
        //CurtainOpen.enabled = false;
        //GameManager.transitioning = false;
    }
    public void transitionEnd()
    {

        Debug.Log("transitionEnd");
        //GameManager.isTransitionEnd = true;
    }
}
