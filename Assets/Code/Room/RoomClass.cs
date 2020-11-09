using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass
{
    public string owner;
    public string roomSize;
    public string start;

    public RoomClass(string owner, string gameType, string starto)
    {
        this.owner = owner;
        this.roomSize = gameType;
        this.start = starto;
    }
}