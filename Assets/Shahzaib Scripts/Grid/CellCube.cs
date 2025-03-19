using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCube : MonoBehaviour
{
    public CellStatus cellStatus;

    public bool OccipedBlock()
    {
        Debug.Log("Detech Cell__C");

        bool Status = false;
        if (cellStatus == CellStatus.Unoccupied)
        {
            Debug.Log("Detech Cell__D");

            Status = true;
            Debug.Log("Detech Cell__E");

            cellStatus = CellStatus.Occupied;
            Debug.LogError("Next Object is occupied");
        }
        else
        {
            Status = false;

        }

        return Status;
    }
}

[Serializable]
public enum CellStatus
{
    Unoccupied,

    Occupied,
}
