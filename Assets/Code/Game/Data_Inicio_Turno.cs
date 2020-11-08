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
        GameObject arrowObject = Instantiate(Flecha, new Vector3(middleX, middleY, 0), Quaternion.identity);
        arrowObject.transform.Rotate(0, 0, angle - 90);
        arrowObject.transform.localScale = new Vector3(0.3f, 0.15f * colliderDistance, 1);
    }

    public class Nodo // En teoria Firebase puede guardar clases 
    {
        public float posx;
        public float posy;
        public float posz;

        public int type;
        public int points;
        public int total_nodes;
        public int used_nodes;
        public int owner;
        public int healingFactor;
        public int dmgFactor;
        public int identifier;
        public List<int> objectives;
        /*
        public Nodo(GameObject nodo)
        {
            this.posx = nodo.transform.position.x;
            this.posy = nodo.transform.position.y;
            this.posz = nodo.transform.position.z;

            //Seleccion_y_Union data = nodo.GetComponent<Seleccion_y_Union>();
            this.type = data.type;
            this.points = data.points;
            this.total_nodes = data.total_nodes;
            this.used_nodes = data.used_nodes;
            this.owner = data.owner;
            this.healingFactor = data.healingFactor;
            this.dmgFactor = data.dmgFactor;
            this.identifier = data.identifier;

            objectives = new List<int>();
        }
        public void adadObj(int data)
        {
            objectives.Add(data);
        }
        */
    }
        

}
