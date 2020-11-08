using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inicio_Ronda : MonoBehaviour
{
    Data_Inicio_Turno data;

    void setup()
    {
        if (data.turn == 0) // si es el primer turno (turno 0 para el armado), armo una nueva partida, de lo contrario, cargo los datos existentes.
        {
            defaultStart();
        }
        else  // Cargo la partida guardada
        {

        }
    }

    void defaultStart()
    {
        GameObject Normal = data.Normal;
        GameObject Extra = data.Extra;
        GameObject Ataque = data.Ataque;
        GameObject Defensa = data.Defensa;

        GameObject e = Instantiate(Normal, new Vector3(7, 3, -1), Quaternion.identity);
        GameObject n = Instantiate(Extra, new Vector3(7, -3, -1), Quaternion.identity);
        GameObject a = Instantiate(Ataque, new Vector3(-7, 3, -1), Quaternion.identity);
        GameObject d = Instantiate(Defensa, new Vector3(-7, -3, -1), Quaternion.identity);
        GameObject n2 = Instantiate(Normal, new Vector3(4, -2, -1), Quaternion.identity);
        GameObject n3 = Instantiate(Normal, new Vector3(4, 2, -1), Quaternion.identity);
        GameObject n4 = Instantiate(Normal, new Vector3(-4, -2, -1), Quaternion.identity);
        GameObject n5 = Instantiate(Normal, new Vector3(-4, 2, -1), Quaternion.identity);
        GameObject n6 = Instantiate(Normal, new Vector3(0, 3, -1), Quaternion.identity);
        GameObject n7 = Instantiate(Normal, new Vector3(0, -3, -1), Quaternion.identity);

    }

    void Start()
    {
        data = GameObject.Find("DataAGuardar").GetComponent<Data_Inicio_Turno>();
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
