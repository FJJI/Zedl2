using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
            //TODO
        }
    }

    void defaultStart() // Este metodo crea 10 nodos para partir (por ahora en posicion fixed)
    {
        GameObject Normal = data.Normal;
        GameObject Extra = data.Extra;
        GameObject Ataque = data.Ataque;
        GameObject Defensa = data.Defensa;
        List<GameObject> nodes = new List<GameObject> {Normal,Extra,Ataque,Defensa}; // Ajustar por el que se toma en favoritos 

        List<int> positionsx = new List<int> {7,-7, 7,-7, 4,4,-4,-4,0, 0};
        List<int> positionsy = new List<int> {3,-3,-3, 3,-2,2,-2, 2,3,-3};
        for (int i = 1; i <= 10; i++)
        {
            Debug.Log(i);
            GameObject nuevo_nodo;
            if (i <= data.InitialPlayers)
            {
                int fav;
                if (data.fav_unit[i-1] == "none") // si no tiene unidad favorita
                {
                    fav = Random.Range(1, 4); // entre 1 y 4
                }
                else
                {
                    fav = int.Parse(data.fav_unit[i - 1]);
                }
                nuevo_nodo = Instantiate(nodes[fav], new Vector3(positionsx[i-1], positionsy[i-1], -1), Quaternion.identity);
                nuevo_nodo.GetComponent<Nodo>().owner = i;
                
            }
            else // si ya estan los nodos iniciales
            {
                nuevo_nodo = Instantiate(nodes[Random.Range(0, 4)], new Vector3(positionsx[i-1], positionsy[i-1], -1), Quaternion.identity);
                nuevo_nodo.GetComponent<Nodo>().owner = 0;
            }
            nuevo_nodo.GetComponent<Nodo>().identifier = i - 1;
            nuevo_nodo.GetComponent<Nodo>().points = 50;
        }
        data.turn = 1; //una vez iniciado todo, hacemos que parta el juego con el 1° turno
        data.playerTurn++;

    }

    void Connect(GameObject sender, GameObject objective)//be sure to add the objective to list before using this function
    {
        Vector2 posSender = sender.transform.position;
        Vector2 posObjective = objective.transform.position;
        Vector2 initialPos = sender.GetComponent<CircleCollider2D>().ClosestPoint(posObjective);
        Vector2 finalPos = objective.GetComponent<CircleCollider2D>().ClosestPoint(posSender);
        float distX = Mathf.Abs(posSender.x - posObjective.x);
        float distY = Mathf.Abs(posSender.y - posObjective.y);
        float colliderDistance = Vector2.Distance(initialPos, finalPos);
        float middleX = (posSender.x + posObjective.x) / 2f;
        float middleY = (posSender.y + posObjective.y) / 2f;
        float angle = Mathf.Atan(distY / distX) * 180 / Mathf.PI;
        if (posSender.x < posObjective.x && posSender.y >= posObjective.y) { angle *= -1; }
        else if (posSender.x >= posObjective.x && posSender.y >= posObjective.y) { angle += 180; }
        else if (posSender.x >= posObjective.x && posSender.y < posObjective.y) { angle += (90 - angle) * 2; }
        GameObject arrowObject = Instantiate(data.Flecha, new Vector3(middleX, middleY, 0), Quaternion.identity);
        arrowObject.transform.Rotate(0, 0, angle - 90);
        arrowObject.transform.localScale = new Vector3(0.3f, 0.15f * colliderDistance, 1);
        sender.GetComponent<Nodo>().unions.Add(arrowObject);
    }

    void DefinePowerFactors(GameObject unit) //this function should be executed when ending the turn before doing the healings/damages, after all connections and points adjustments are done
    {
        int points = unit.GetComponent<Nodo>().points;
        int unitType = unit.GetComponent<Nodo>().type;
        unit.GetComponent<Nodo>().healingFactor = (int)Mathf.Sqrt(points);
        unit.GetComponent<Nodo>().dmgFactor = (int)Mathf.Sqrt(points);
        if (unitType == 1) { unit.GetComponent<Nodo>().dmgFactor *= 2; }
        else if (unitType == 2) { unit.GetComponent<Nodo>().healingFactor *= 2; }
    }

    void AtackHealUnit(GameObject sender, GameObject objective) //remeber to DefinePowerFactors before atking/healing, this function autoconvert the unit owner when defeated
    {
        Nodo senderAttributes = sender.GetComponent<Nodo>();
        Nodo objectiveAttributes = objective.GetComponent<Nodo>();
        if (senderAttributes.owner == objectiveAttributes.owner)
        {
            objectiveAttributes.points = Mathf.Min(100, objectiveAttributes.points + senderAttributes.healingFactor);
        }
        else
        {
            objectiveAttributes.points -= senderAttributes.dmgFactor;
            if (objectiveAttributes.points < 0)
            {
                objectiveAttributes.points *= -1;
                objectiveAttributes.owner = senderAttributes.owner;
            }
        }
    }


    bool PermitConnection(GameObject sender, GameObject objective)
    {
        int points = sender.GetComponent<Nodo>().points;
        Vector2 posSender = sender.transform.position;
        Vector2 posObjective = objective.transform.position;
        float distTotal = Vector2.Distance(posSender, posObjective);
        if ((int)(90f * distTotal / Camera.main.GetComponent<CameraSize>().camWidth) <= points)
        {
            return true;
        }
        else
        {
            return false;
            //send out of range error message 
        }
    }





    void Start()
    {
        data = GameObject.Find("Data").GetComponent<Data_Inicio_Turno>();
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
