﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Data_Inicio_Turno : MonoBehaviour
{
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;

    static public Data_Inicio_Turno data;
    public int matchID;  // para sacar - poner la data
    public int turn;
    public int playerTurn;  // para saber a quien le damos el beneficio de jugar, sino poner una nota
    public int InitialPlayers;  // para saber la cantidad de la player para armar la partida, los cambios de turno y saber cuando alguien pierde
    public List<bool> defeated;
    public List<string> fav_unit = new List<string> { "none", "none", "none", "none" };
    public List<string> players;

    // Los Nodos + la flecha 
    public GameObject Normal;
    public GameObject Extra;
    public GameObject Ataque;
    public GameObject Defensa;
    public GameObject Flecha;

    public List<GameObject> nodos;
    public List<NodoClass> nodosListos;

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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
    }


    public async void SaveData()
    {
        for (int i = 1; i <= 10; i++) // pongo los nodos en formato para guardar
        {
            NodoClass nc = new NodoClass(nodos[i - 1]);
            nodosListos.Add(nc);


        }
        string jsonNodos = JsonUtility.ToJson(nodosListos);
        await reference.Child("rooms").Child(matchID.ToString()).SetRawJsonValueAsync(jsonNodos);
        Debug.Log("Nodos Guardados");
    }
}

