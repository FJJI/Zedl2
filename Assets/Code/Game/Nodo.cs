using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nodo : MonoBehaviour
{
    public int owner; //Asi sabemos a quien le pertenece para luego hacer las acciones con el
    public int total_unions = 2;
    public int used_unions = 0;
    public int type; // Tipo 0 es normal, 1 es warrior, 2 es defensa, 3 es nodo extra
    public List<GameObject> objectives;
    public List<GameObject> unions; // flechas, necesitare las referencias para borrarlas
    public Component halo;
    public SpriteRenderer sprite;
    public int points;
    public int healingFactor;
    public int dmgFactor;
    float fade = 0;
    public int identifier; // dato para la BDD

    //sacados del anterior juego ver como colocar
    public static GameObject first; // para aplicar halo
    GameObject textObject;
    public GameObject msgGameObject;


    //por si necesitamos alguna data mas, toda debiese existir aca
    Data_Inicio_Turno data;

    /*
    void AddEmpty() // esto esta para cuando se crean, no tener problemas de que me salgo de la lista
    {
        for (int i = 0; i < total_unions; i++)
        {
            objectives.Add(null);
            unions.Add(null);
        }
    }
    */


    void ChangeColor()
    {
        #region Colores y halo
        if (owner == 0)
        {
            sprite.color = new Color(1f, 1f, 1f, 1);
        }
        else if (owner == 1)
        {
            sprite.color = new Color(0.2002492f, 9433962f, 0.5556294f, 1);
        }
        else if (owner == 2)
        {
            sprite.color = new Color(0.8867924f, 0.0f, 0.788344f, 1);
        }
        else if (owner == 3)
        {
            sprite.color = new Color(0.9677409f, 1f, 0.495283f, 1);
        }
        else if (owner == 4)
        {
            sprite.color = new Color(1f, 0f, 0f, 1);
        }
        if (first == this.gameObject)
        {
            halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
        }
        else
        {
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
        }
        #endregion
    }

    void fadeMsg()
    {
        msgGameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().faceColor = new Color32(0, 0, 0, (byte)fade);
        
    }

    void sendMessage(string message)
    {
        msgGameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = message;
        fade = 255;
    }

    private void OnMouseDown()
    {
        if (!first)
        {
            if (data.playerTurn == owner)
            {
                Debug.Log("Seleccionado" + this.gameObject);
                first = this.transform.gameObject;
            }
            else
            {
                sendMessage("Not your unit!");
            }
        }
        else if (first == this.transform.gameObject)
        {
            Debug.Log("Ya no hay first" + this.gameObject);
            Debug.Log("Todos los nodos eliminados" + this.gameObject);
            int auxCount = objectives.Count;
            for (int i = 0; i < auxCount; i++)
            {
                RecoverPointsFromConnectionCancel(gameObject,objectives[0]);
                DeleteConnection(gameObject,objectives[0]);
            }
            used_unions = 0;
            first = null;
        }

        else if (first != this.transform.gameObject)
        {
            Nodo first_code = first.GetComponent<Nodo>();
            //con este if, considero que quede al menos un "objective" en null
            if (first_code.used_unions + 1 <= first_code.total_unions)
            {
                //veo la union, si ya existe, la destruyo
                for (int i = 0; i < first_code.objectives.Count; i++)
                {
                    if (first_code.objectives[i] == this.gameObject)
                    {
                        //ya existe, elimino la flecha y libero el cupo
                        RecoverPointsFromConnectionCancel(first,gameObject);
                        DeleteConnection(first,gameObject);
                        first = null;
                        return;
                    }
                }
                //no existe, selecciono el espacio para agregar la union

                if(PermitConnection(first,gameObject))
                {
                    first_code.objectives.Add(gameObject);
                    Connect(first, gameObject);
                    PointsAfterConnection(first, gameObject);
                    first_code.used_unions += 1;
                    first=null;
                }

            }
            else
            {
                for (int i = 0; i < first_code.objectives.Count; i++)
                {
                    if (first_code.objectives[i] == this.gameObject)
                    {
                        //ya existe, elimino la flecha y libero el cupo
                        RecoverPointsFromConnectionCancel(first,gameObject);
                        DeleteConnection(first,gameObject);
                        Debug.Log("nodo sacado" + this.gameObject);
                        first = null;
                        return;
                    }
                    else
                    {
                        Debug.Log("This node can´t have more nodes");
                        sendMessage("No connections left!");
                    }
                }
            }
        }
    }

    void UpdatePointsNumber() //this function update the value on the centerText of the unit, this should be looped throught all units
    {
        int points = gameObject.GetComponent<Nodo>().points;
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
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

    void PointsAfterConnection(GameObject sender, GameObject objective)// use only with values pre validated by PermitConnection, reduce sender points by stretching concept
    {
        Vector2 posSender = sender.transform.position;
        Vector2 posObjective = objective.transform.position;
        float distTotal = Vector2.Distance(posSender, posObjective);
        int points = sender.GetComponent<Nodo>().points;
        int finalPoints = points - (int)(90f * distTotal / Camera.main.GetComponent<CameraSize>().camWidth);
        sender.GetComponent<Nodo>().points = finalPoints;
    }

    void RecoverPointsFromConnectionCancel(GameObject sender, GameObject objective)// recover all the points (or less when over 100) when 
    {
        Vector2 posSender = sender.transform.position;
        Vector2 posObjective = objective.transform.position;
        float distTotal = Vector2.Distance(posSender, posObjective);
        int points = sender.GetComponent<Nodo>().points;
        int finalPoints = points + Mathf.Min(100, (int)(90f * distTotal / Camera.main.GetComponent<CameraSize>().camWidth));
        sender.GetComponent<Nodo>().points = finalPoints;
    }

    void DeleteConnection(GameObject sender, GameObject objective) //remove the arrow from unions and objectives lists, and scene
    {
        int index = sender.GetComponent<Nodo>().objectives.IndexOf(objective);
        Destroy(sender.GetComponent<Nodo>().unions[index]);
        sender.GetComponent<Nodo>().unions.RemoveAt(index);
        sender.GetComponent<Nodo>().objectives.Remove(objective);
        sender.GetComponent<Nodo>().used_unions--;
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
            sendMessage("Too far...");
            return false;
        }
    }

    public void DefinePowerFactors() //this function should be executed when ending the turn before doing the healings/damages, after all connections and points adjustments are done
    {
        healingFactor = (int)Mathf.Sqrt(points);
        dmgFactor = (int)Mathf.Sqrt(points);
        if (type == 1) { dmgFactor *= 2; }
        else if (type == 2) { healingFactor *= 2; }
    }

    public void AtackHealUnit(GameObject objective) //remeber to DefinePowerFactors before atking/healing, this function autoconvert the unit owner when defeated
    {
        Nodo senderAttributes = gameObject.GetComponent<Nodo>();
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


    void Start()
    {
        data = GameObject.Find("Data").GetComponent<Data_Inicio_Turno>();
    }

    void Update()
    {
        UpdatePointsNumber();
        ChangeColor();
        if (fade > 0)
        {
            fade -= 0.5f;
            fadeMsg();
        }
    }
}
