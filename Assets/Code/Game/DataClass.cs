using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataClass // En teoria Firebase puede guardar clases 
{
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde
    public int losers;

    public DataClass(int matchID, int turn, int playerTurn, int InitialPlayers, int losers)
    {
        this.matchID = matchID;
        this.turn = turn;
        this.playerTurn = playerTurn;
        this.InitialPlayers = InitialPlayers;
        this.losers = losers;
    }
}

