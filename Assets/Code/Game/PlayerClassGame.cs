using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassGame : MonoBehaviour
{
    // Start is called before the first frame update

    public string username;
    public int turn;
    public bool defeated;
    public int fav;

    public PlayerClassGame(string username, int turn, bool defeated, int fav)
    {
        this.username = username;
        this.turn = turn;
        this.defeated = defeated;
        this.fav = fav;
    }
}
