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

    void defaultStart() // Este metodo crea 10 nodos para partir (por ahora en posicion fixed)
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

    void Connect(GameObject sender, GameObject objective)
    {
        Vector2 posSender = sender.transform.position;
        Vector2 posObjective = objective.transform.position;
        Vector2 initialPos = sender.GetComponent<CircleCollider2D>().ClosestPoint(posObjective);
        Vector2 finalPos = objective.GetComponent<CircleCollider2D>().ClosestPoint(posSender);
        float distX = Mathf.Abs(posSender.x - posObjective.x);
        float distY = Mathf.Abs(posSender.y - posObjective.y);
        //float centerDistance = Vector2.Distance(posSender, posObjective);
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
