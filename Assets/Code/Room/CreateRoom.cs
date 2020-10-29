using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CreateRoom : MonoBehaviour
{
    public Button twoPButton, threePButton, FourPButton, backButton;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    public bool created;
    public bool found;
    public int roomId;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeld-e907d.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        created = true;
        found = false;
        roomId = 0;
        twoPButton.onClick.AddListener(() => RoomCreation("2"));
        threePButton.onClick.AddListener(() => RoomCreation("3"));
        FourPButton.onClick.AddListener(() => RoomCreation("4"));
        backButton.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
    }

    public async void RoomCreation(string players)
    {
        while (created == true)
        {
            System.Random r = new System.Random();
            roomId = r.Next(0, 10000);
            await dbInstance.GetReference("rooms").Child(roomId.ToString()).GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted) { }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot == null)
                    {
                        created = true;
                    }
                    else
                    {
                        created = false;
                    }

                }
            });
            PlayerPrefs.SetString("Room", roomId.ToString());
            RoomClass room = new RoomClass(PlayerPrefs.GetString("UID"), players);
            string json = JsonUtility.ToJson(room);
            await reference.Child("rooms").Child(roomId.ToString()).SetRawJsonValueAsync(json);
            PlayerClass player = new PlayerClass(PlayerPrefs.GetString("UserName"), "false");
            json = JsonUtility.ToJson(player);
            await reference.Child("rooms").Child(roomId.ToString()).Child("players").Child(PlayerPrefs.GetString("UID")).SetRawJsonValueAsync(json);
            SceneManager.LoadScene("RoomScene");
        }
    }
}
