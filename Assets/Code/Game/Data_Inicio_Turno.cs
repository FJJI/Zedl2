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


    void Update()
    {
        gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text="Player "+playerTurn+" turn";
    }

    async void GetNodes()
    {
        DBnodos = new List<NodoClass>();
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
                    int p_x = int.Parse(dictNodos["posx"].ToString());
                    int p_y = int.Parse(dictNodos["posy"].ToString());
                    int p_z = int.Parse(dictNodos["posz"].ToString());
                    int type = int.Parse(dictNodos["type"].ToString());
                    if (u_u != 0)
                    {
                        Debug.Log("objetivos");
                        foreach (DataSnapshot objs in nodo.Child("objective").Children)
                        {
                            Debug.Log(objs.Value.ToString());
                            objetivos.Add(int.Parse(objs.Value.ToString()));
                        }
                    }


                }
            }
        });
    }
    
}


