using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
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

    [Obsolete]
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        created = true;
        found = false;
        roomId = 0;
        twoPButton.onClick.AddListener(() => RoomCreation("2"));
        threePButton.onClick.AddListener(() => RoomCreation("3"));
        FourPButton.onClick.AddListener(() => RoomCreation("4"));
        backButton.onClick.AddListener(() => BackMenu());
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
        }
        Debug.Log("creando room");
        PlayerPrefs.SetString("Room", roomId.ToString());
        RoomClass room = new RoomClass(PlayerPrefs.GetString("UID"), players, "false");
        string json = JsonUtility.ToJson(room);
        await reference.Child("rooms").Child(roomId.ToString()).SetRawJsonValueAsync(json);
        Debug.Log("Room creada");
        PlayerClass player = new PlayerClass(PlayerPrefs.GetString("UserName"), "false");
        json = JsonUtility.ToJson(player);
        await reference.Child("rooms").Child(roomId.ToString()).Child("players").Child(PlayerPrefs.GetString("UID")).SetRawJsonValueAsync(json);
        DataClass datagame = new DataClass(roomId, 0, 1, int.Parse(players));
        string jsonData = JsonUtility.ToJson(datagame); 
        await reference.Child("rooms").Child(roomId.ToString()).Child("datapartida").SetRawJsonValueAsync(jsonData);
        Destroy(GameObject.Find("MusicManager"));
        SceneManager.LoadScene("RoomScene");
        
    }

    public void BackMenu()
    {
        Destroy(GameObject.Find("MusicManager"));
        SceneManager.LoadScene("MainMenu");
    }
}
