using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo : MonoBehaviour
{
    public int owner; //Asi sabemos a quien le pertenece para luego hacer las acciones con el
    public int total_unions = 2;
    public int used_unions = 0;
    public int type; // Tipo 0 es normal, 1 es warrior, 2 es defensa, 3 es nodo extra
    public List<GameObject> objectives;
    public Component halo;
    public SpriteRenderer sprite;
    public int healingFactor;
    public int dmgFactor;
    public int points;
    float fade = 0;

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
        // TextMeshProUGUI agregar ese
        //msgGameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().faceColor = new Color32(0, 0, 0, (byte)fade);
    }

    void Start()
    {
        data = GameObject.Find("Data").GetComponent<Data_Inicio_Turno>();
        AddEmpty();
    }

    void Update()
    {
        //textObject.GetComponent<TextMeshProUGUI>().text = points.ToString();
        ChangeColor();
        if (fade > 0)
        {
            fade -= 0.5f;
            fadeMsg();
        }
    }
}
