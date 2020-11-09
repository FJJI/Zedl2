using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject data;

    int turn;

    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        turn = data.GetComponent<Data_Inicio_Turno>().playerTurn;
        if(data.GetComponent<Data_Inicio_Turno>().players[turn-1]==PlayerPrefs.GetString("UserName"))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        */
        
    }

    void OnMouseDown()
    {
        
        data.GetComponent<Data_Inicio_Turno>().turn++;
        while(true)
        {
            data.GetComponent<Data_Inicio_Turno>().playerTurn++;
            if(data.GetComponent<Data_Inicio_Turno>().playerTurn>data.GetComponent<Data_Inicio_Turno>().InitialPlayers)
            {
                data.GetComponent<Data_Inicio_Turno>().playerTurn=1;
            }
            if (!(data.GetComponent<Data_Inicio_Turno>().defeated[data.GetComponent<Data_Inicio_Turno>().playerTurn-1]))
            {
                ExecuteChanges();
                data.GetComponent<Data_Inicio_Turno>().SaveData();
                break;
            }
        }
        
        
    }

    void ExecuteChanges()
    {
        for(int i = 0; i<data.GetComponent<Data_Inicio_Turno>().nodos.Count;i++)
        {
            data.GetComponent<Data_Inicio_Turno>().nodos[i].GetComponent<Nodo>(). DefinePowerFactors();
        }
        for(int i = 0; i<data.GetComponent<Data_Inicio_Turno>().nodos.Count;i++)
        {
            for(int j = 0; j < data.GetComponent<Data_Inicio_Turno>().nodos[i].GetComponent<Nodo>().used_unions;j++)
            {
                data.GetComponent<Data_Inicio_Turno>().nodos[i].GetComponent<Nodo>().AtackHealUnit(data.GetComponent<Data_Inicio_Turno>().nodos[i].GetComponent<Nodo>().objectives[j]);
            }
        }
        
    }
}
