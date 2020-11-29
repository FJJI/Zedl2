using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Threading;
using System.Linq;

public class Data_Inicio_Turno : MonoBehaviour
{
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    Inicio_Ronda inicio;
    static public Data_Inicio_Turno data;
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde
    public List<bool> defeated;
    public List<string> fav_unit = new List<string> { "none", "none", "none", "none" };
    public List<string> players;
    public List<NodoClass> DBnodos;

// Los Nodos + la flecha 
    public GameObject Normal;
    public GameObject Extra;
    public GameObject Ataque;
    public GameObject Defensa;
    public GameObject Flecha;
    public List<GameObject> nodes;

    public List<GameObject> nodos;
    public List<NodoClass> nodosListos;
    string jsonNodos;

    void Awake()  // Hacemos que la esta data exista en lapartida y de ser necesario, desde el room para su trata con la informacion
    {
        inicio = GameObject.Find("Manager Partida").GetComponent<Inicio_Ronda>();
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

    [Obsolete]
    private void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        players = new List<string>();
        defeated = new List<bool>();
        DBnodos = new List<NodoClass>();
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        matchID = int.Parse(PlayerPrefs.GetString("Room"));
        dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("datapartida").Child("turn").ValueChanged += HandleChangeTurn;
        dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("datapartida").Child("playerTurn").ValueChanged += HandleChangePlayer;

        nodes = new List<GameObject> { Normal, Ataque, Defensa, Extra }; // Ajustar por el que se toma en favoritos 

        turno_de_mentira = turn;
    }

    private void HandleChangeTurn(object sender, ValueChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot msg = args.Snapshot;
        Debug.Log(msg.Value.ToString());
        turn = int.Parse(msg.Value.ToString());

    }

    private void HandleChangePlayer(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot msg = args.Snapshot;
        Debug.Log(msg.Value.ToString());
        playerTurn = int.Parse(msg.Value.ToString());
        GetNodes();
    }

    public async void GetPlayers() 
    {
        List<String> users = new List<string>();
        await dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("datapartida").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IDictionary dictData = (IDictionary)snapshot.Value;
                playerTurn = int.Parse(dictData["playerTurn"].ToString());
            }
        });
        await dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("players").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children)
                {
                    IDictionary dictUser = (IDictionary)user.Value;
                    users.Add(dictUser["username"].ToString());
                    Debug.Log(dictUser["username"].ToString());
                    defeated.Add(false);
                }
            }
            players = users;
            Debug.Log(players.Count);
        });
        inicio.setup();
    }
    

    public async void SaveData()
    {
        for (int i = 1; i <= nodos.Count; i++) // pongo los nodos en formato para guardars
        {
            Debug.Log("Nodo" + (i-1));
            NodoClass nc = new NodoClass(nodos[i - 1]);
            string ident = nc.identifier.ToString();
            jsonNodos = JsonUtility.ToJson(nc);
            await reference.Child("rooms").Child(matchID.ToString()).Child("nodos").Child(ident).SetRawJsonValueAsync(jsonNodos);
        }
        DataClass dc = new DataClass(matchID, turn, playerTurn, InitialPlayers);
        string jsonData = JsonUtility.ToJson(dc);
        await reference.Child("rooms").Child(matchID.ToString()).Child("datapartida").SetRawJsonValueAsync(jsonData);
        Debug.Log("data segura");

        for (int i = 0; i < InitialPlayers; i++)
        {
            PlayerClassGame pc = new PlayerClassGame(players[i], i+1, defeated[i], -1);
            string jsonPlayer = JsonUtility.ToJson(pc);
            await reference.Child("rooms").Child(matchID.ToString()).Child("participantes").SetRawJsonValueAsync(jsonPlayer);
            Debug.Log("PC segura");
        }
    }

    int turno_de_mentira;
    void Update()
    {
        gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text="Player "+playerTurn+" turn";
        if (turno_de_mentira != turn)
        {
            Thread t = new Thread(new ParameterizedThreadStart((object sender) =>
            {
                Thread.Sleep(4000);
                GetNodes();
            }));
        }
    }

    public async void GetNodes()
    {
        bool creado = false;
        DBnodos = new List<NodoClass>();
        List<List<int>> objetivos_tot = new List<List<int>>();
        List<int> owners = new List<int>();
        List<int> t_us = new List<int>();
        List<int> u_us = new List<int>();
        List<int> dmgs = new List<int>();
        List<int> heals = new List<int>();
        List<int> idents = new List<int>();
        List<int> pointss = new List<int>();
        List<int> types = new List<int>();
        List<float> p_xs = new List<float>();
        List<float> p_ys = new List<float>();
        List<float> p_zs = new List<float>();
        List<Nodo> nodoClasses = new List<Nodo>();
        foreach (GameObject nod in nodos)
        {
            nodoClasses.Add(nod.GetComponent<Nodo>());
        }

        await dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("nodos").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot nodo in snapshot.Children)
                {
                    List<int> objetivos = new List<int>();
                    IDictionary dictNodos = (IDictionary)nodo.Value;
                    int owner = int.Parse(dictNodos["owner"].ToString());
                    int t_u = int.Parse(dictNodos["owner"].ToString());
                    int u_u = int.Parse(dictNodos["used_unions"].ToString());
                    int dmg = int.Parse(dictNodos["dmgFactor"].ToString());
                    int healing = int.Parse(dictNodos["healingFactor"].ToString());
                    int ident = int.Parse(dictNodos["identifier"].ToString());
                    int points = int.Parse(dictNodos["points"].ToString());
                    Debug.Log(points);
                    float p_x = float.Parse(dictNodos["posx"].ToString());
                    float p_y = float.Parse(dictNodos["posy"].ToString());
                    float p_z = float.Parse(dictNodos["posz"].ToString());
                    int type = int.Parse(dictNodos["type"].ToString());
                    if (u_u != 0)
                    {
                        Debug.Log("objetivos");
                        foreach (DataSnapshot objs in nodo.Child("objectives").Children)
                        {
                            Debug.Log(objs.Value.ToString());
                            objetivos.Add(int.Parse(objs.Value.ToString()));
                        }
                        objetivos_tot.Add(objetivos);
                    }
                    // Acualizamos nodos
                    if (nodos.Count == 0)
                    {
                        creado = true;

                        owners.Add(owner);
                        t_us.Add(t_u);
                        u_us.Add(u_u);
                        dmgs.Add(dmg);
                        heals.Add(healing);
                        idents.Add(ident);
                        pointss.Add(points);
                        types.Add(type);
                        p_xs.Add(p_x);
                        p_ys.Add(p_y);
                        p_zs.Add(p_z);
                    }
                    else
                    {
                        foreach (Nodo nodoLocal in nodoClasses)
                        {
                            if (nodoLocal.identifier == ident)
                            {
                                nodoLocal.owner = owner;
                                nodoLocal.total_unions = t_u;
                                nodoLocal.used_unions = u_u;
                                nodoLocal.dmgFactor = dmg;
                                nodoLocal.healingFactor = healing;
                                nodoLocal.identifier = ident;
                                nodoLocal.points = points;


                                nodoLocal.objectives.Clear();
                                // To Do, Botar Flechas  --> Emilio

                                for (int i = 0; i < objetivos.Count; i++)
                                {
                                    nodoLocal.objectives.Add(nodos[objetivos[i]]);
                                    // TO DO: Hacer Flechas --> Emilio

                                }
                            }
                        }
                    }

                }
            }
        });
        if (creado)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject nuevo_nodo;
                nuevo_nodo = Instantiate(nodes[types[j]], new Vector3(p_xs[j], p_ys[j], p_zs[j]), Quaternion.identity);
                nuevo_nodo.GetComponent<Nodo>().owner = owners[j];
                nuevo_nodo.GetComponent<Nodo>().total_unions = t_us[j];
                nuevo_nodo.GetComponent<Nodo>().used_unions = u_us[j];
                nuevo_nodo.GetComponent<Nodo>().dmgFactor = dmgs[j];
                nuevo_nodo.GetComponent<Nodo>().healingFactor = heals[j];
                nuevo_nodo.GetComponent<Nodo>().identifier = idents[j];
                nuevo_nodo.GetComponent<Nodo>().points = pointss[j];

                nodos.Add(nuevo_nodo);
            }

            //objetivos 
            foreach (GameObject node in nodos)
            {
                int i = 0;
                Nodo dataNode = node.GetComponent<Nodo>();
                for(int e = 0; e < objetivos_tot[i].Count; e++)
                {
                    GameObject data_obj = nodos.SingleOrDefault(x => x.GetComponent<Nodo>().identifier == objetivos_tot[i][e]);
                    dataNode.objectives.Add(data_obj);
                }
            }
        }
        else
        {
            //nodoLocal.transform.position = new Vector3(p_x, p_y, p_z);
        }
    }
    
}


