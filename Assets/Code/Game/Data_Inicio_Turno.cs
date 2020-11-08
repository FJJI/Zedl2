using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Inicio_Turno : MonoBehaviour
{
    static public Data_Inicio_Turno data;
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde
    public List<bool> defeated;

    // Los Nodos + la flecha 
    public GameObject Normal;
    public GameObject Extra;
    public GameObject Ataque;
    public GameObject Defensa;
    public GameObject Flecha;


    void Awake()  // Hacemos que la esta data exista en lapartida y de ser necesario, desde el room para su trata con la informacion
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (data != this)
        {
            Destroy(gameObject);
        }
    }


}
