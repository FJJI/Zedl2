using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass
{
    public string owner;
    public string roomSize;

    public RoomClass(string owner, string gameType)
    {
        this.owner = owner;
        this.roomSize = gameType;
    }
}