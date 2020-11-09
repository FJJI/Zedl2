using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataClass // En teoria Firebase puede guardar clases 
{
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde

    public DataClass(Data_Inicio_Turno data)
    {
        matchID = data.matchID;
        turn = data.turn;
        playerTurn = data.playerTurn;
        InitialPlayers = data.InitialPlayers;
    }
}

