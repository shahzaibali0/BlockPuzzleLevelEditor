using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{

    public List<PickableContainer> container = new List<PickableContainer>();

    public Action BlockPass;
    public float WidthBlocks, HeightBlocks;
    private void OnEnable()
    {
        BlockPass += AllBlockspassed;


    }

    private void OnDisable()
    {
        BlockPass -= AllBlockspassed;
    }


    public void AllBlockspassed()
    {
        foreach (var item in container)
        {
            if (container.All(item => item.ContainerBlock.BlockPass))
            {
                Debug.Log("Level Complete");
            }
        }

    }
}
