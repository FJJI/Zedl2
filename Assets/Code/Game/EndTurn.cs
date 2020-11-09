using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject data;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Debug.Log(data.GetComponent<Data_Inicio_Turno>().playerTurn-1);
            if (!(data.GetComponent<Data_Inicio_Turno>().defeated[data.GetComponent<Data_Inicio_Turno>().playerTurn-1]))
            {
                data.GetComponent<Data_Inicio_Turno>().SaveData();
                break;
            }
        }
        
        
    }
}
