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

    static public Data_Inicio_Turno data;
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde
    public List<bool> defeated = new List<bool> { false, false, false, false }; 
    public List<string> fav_unit = new List<string> { "none", "none", "none", "none" };
    public List<string> players = new List<string> { "eivelez", "perro", "none", "none" };

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
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (data != this)
        {
            Destroy(gameObject);
        }
        players = new List<string> { "MRFJJI", "MRFJJI" };
    }

    [Obsolete]
    private void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        matchID = int.Parse(PlayerPrefs.GetString("Room"));
        dbInstance.GetReference("rooms").Child(matchID.ToString()).Child("datapartida").ChildChanged += HandleChangeGame;


    }
    private void HandleChangeGame(object sender, ChildChangedEventArgs args)
    {
        
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.LogWarning("ASDASDASDASD");
        DataSnapshot msg = args.Snapshot;
        Debug.Log(args.Snapshot);
        IDictionary dataPartida = (IDictionary)msg.Value;
        Debug.Log(dataPartida["turn"]);
        turn = int.Parse(dataPartida["turn"].ToString());
        playerTurn = int.Parse(dataPartida["playerTurn"].ToString());
        InitialPlayers = int.Parse(dataPartida["InitialPlayers"].ToString());
        
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
        DataClass dc = new DataClass(data);
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

    
}


