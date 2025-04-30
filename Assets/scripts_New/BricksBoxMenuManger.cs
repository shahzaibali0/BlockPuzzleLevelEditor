using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BricksBoxMenuManger : MonoBehaviour
{
    public BrickInfoUI BrickInfoUI;
    public List<BrickInfoUI> Bricks = new List<BrickInfoUI>();

    [Button(ButtonSizes.Medium)]
    public void SpawnBox()
    {

        var brickColorInfos = DataManager.Instance.buildingsData.allBuildingsDatas[DataManager.BuildingNo].BrickColorInfos;

        // Loop over BrickColorInfos
        for (int i = 0; i < brickColorInfos.Count; i++)
        {
            Debug.Log("SpawnUiMenus__A" + i);

            if (i < Bricks.Count)
            {
                // If the BrickInfoUI already exists, just initialize it with new data
                Bricks[i].Initialize(brickColorInfos[i].BrickType, brickColorInfos[i].TotalBricksPerColor);
            }
            else
            {
                // If it does not exist, instantiate a new one and add it to the list
                BrickInfoUI brick = Instantiate(BrickInfoUI, transform);
                brick.Initialize(brickColorInfos[i].BrickType, brickColorInfos[i].TotalBricksPerColor);
                Bricks.Add(brick);
            }
        }

        // If Bricks list is longer than BrickColorInfos, destroy the extra UI elements
        for (int j = Bricks.Count - 1; j >= brickColorInfos.Count; j--)
        {
            Destroy(Bricks[j].gameObject);
            Bricks.RemoveAt(j);
        }
    }
}
