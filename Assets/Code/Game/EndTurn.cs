using TMPro;
using UnityEngine;
using System.Linq;

public class EndTurn : MonoBehaviour
{
    // Start is called before the first frame update
    Data_Inicio_Turno data;
    int turn;

    bool activeButton;

    void Start()
    {
       data = GameObject.Find("Data").GetComponent<Data_Inicio_Turno>();
    }

    // Update is called once per frame
    void Update()
    {     
        turn = data.playerTurn;
        if(data.players[turn-1]==PlayerPrefs.GetString("UserName"))
        {
            activeButton=true;
            gameObject.GetComponent<TextMeshProUGUI>().text = "Next Turn";
        }
        else
        {
            activeButton=false;
            gameObject.GetComponent<TextMeshProUGUI>().text = "Waiting for player";
        }
        
    }

    void OnMouseDown()
    {       
        if (activeButton)
        {           
            data.turn++;
            while(true)
            {               
                data.playerTurn++;
                if (data.playerTurn > data.InitialPlayers)
                {
                    data.playerTurn=1;
                }             
                if (data.defeated[data.playerTurn-1] == true)
                {
                    data.playerTurn++;
                }
                
                if (!(data.defeated[data.playerTurn-1]))
                {
                    data.CheckEndGame();
                    ExecuteChanges();
                    data.SaveData();

                    break;
                }
            }
        }
    }

    void ExecuteChanges()
    {
        for (int i = 0; i<data.nodos.Count;i++)
        {
            data.nodos[i].GetComponent<Nodo>().DefinePowerFactors();
        }
        for(int i = 0; i < data.nodos.Count; i++)
        {
            for (int j = 0; j < data.nodos[i].GetComponent<Nodo>().used_unions; j++)
            {
                data.nodos[i].GetComponent<Nodo>().AtackHealUnit(data.nodos[i].GetComponent<Nodo>().objectives[j]);
            }
        }
        
    }
}
