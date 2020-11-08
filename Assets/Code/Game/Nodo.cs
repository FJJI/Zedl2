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

    void AddEmpty() // esto esta para cuando se crean, no tener problemas de que me salgo de la lista
    {
        for (int i = 0; i < total_unions; i++)
        {
            objectives.Add(null);
            unions.Add(null);
        }
    }

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
        }
        else if (first == this.transform.gameObject)
        {
            Debug.Log("Ya no hay first" + this.gameObject);
            Debug.Log("Todos los nodos eliminados" + this.gameObject);
            for (int i = 0; i < total_unions; i++)
            {
                if (objectives[i] != null)
                {
                    int pointsToAdd = (int)(100f * Vector2.Distance(transform.position, objectives[i].transform.position) / Camera.main.GetComponent<CameraSize>().camWidth);
                    if (points + pointsToAdd >= 100)
                    {
                        points = 100;
                    }
                    else
                    {
                        points += pointsToAdd;
                    }
                }
                objectives[i] = null;
                try { Destroy(unions[i].gameObject); } catch { }
                unions[i] = null;
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
                for (int i = 0; i < first_code.total_unions; i++)
                {
                    if (first_code.objectives[i] == this.gameObject)
                    {
                        //ya existe, elimino la flecha y libero el cupo
                        first_code.used_unions -= 1;
                        int pointsToAdd = (int)(100f * Vector2.Distance(gameObject.transform.position, first_code.transform.position) / Camera.main.GetComponent<CameraSize>().camWidth);
                        if (first_code.points + pointsToAdd >= 100)
                        {
                            first_code.points = 100;
                        }
                        else
                        {
                            first_code.points += pointsToAdd;
                        }
                        first_code.objectives[i] = null;
                        Destroy(first_code.unions[i].gameObject);
                        first_code.unions[i] = null;
                        //Debug.Log("nodo sacado" + this.gameObject);
                        first = null;
                        return;
                    }
                }
                int index_to_use = 0;
                //no existe, selecciono el espacio para agregar la union
                for (int i = 0; i < first_code.total_unions; i++)
                {
                    // cuando encontremos uno vacio lo usaremos
                    if (first_code.objectives[i] == null)
                    {
                        first_code.objectives[i] = this.gameObject;
                        index_to_use = i;
                        first_code.used_unions += 1;
                        break;
                    }
                }

            }
            else
            {
                for (int i = 0; i < first_code.total_unions; i++)
                {
                    if (first_code.objectives[i] == this.gameObject)
                    {
                        //ya existe, elimino la flecha y libero el cupo
                        first_code.used_unions -= 1;
                        first_code.objectives[i] = null;
                        Destroy(first_code.unions[i].gameObject);
                        first_code.unions[i] = null;
                        Debug.Log("nodo sacado" + this.gameObject);
                        first = null;
                        return;
                    }
                    else
                    {
                        //Debug.Log("This node can´t have more nodes");
                        sendMessage("No connections left!");
                    }
                }
            }
        }
    }

    void UpdatePointsNumber(GameObject unit) //this function update the value on the centerText of the unit, this should be looped throught all units
    {
        int points = unit.GetComponent<Nodo>().points;
        unit.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }

    void Start()
    {
        data = GameObject.Find("Data").GetComponent<Data_Inicio_Turno>();
        AddEmpty();
        textObject = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        textObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
    }

    void Update()
    {
        textObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
        ChangeColor();
        if (fade > 0)
        {
            fade -= 0.5f;
            fadeMsg();
        }
    }
}
