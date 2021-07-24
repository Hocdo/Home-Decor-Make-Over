using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomDataTable", menuName = "ScriptableObjects/RoomDataTable", order = 1)]
public class RoomDataTable : ScriptableObject
{
    protected static RoomDataTable instance;

    public static RoomDataTable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<RoomDataTable>("")[0];
            }

            return instance;
        }
    }

    public RoomData GetRoomDataWithId(int id)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].id == id)
                return data[i];
        }

        return null;
    }

    public RoomData[] data;
}

