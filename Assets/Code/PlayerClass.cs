using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass
{
    public string username;
    public string ready;
    public string fav_unit;

    public PlayerClass(string username, string ready, string fav_unit)
    {
        this.username = username;
        this.ready = ready;
        this.fav_unit = fav_unit;
    }
}
