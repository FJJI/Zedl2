using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class RoomScript : MonoBehaviour
{

    public Button readyButton, startButton, leaveButton, sendButton, backProfileButton;
    public Text idLabel, playersLabel, friendName, friendLoses, friendWins, prefFriend, friendPrefU, nemesisFriend, Message;
    public InputField chatInput;
    public GameObject chatView, playersView, chatRow, playersRow;
    public Canvas roomCanvas, profileCanvas;
    public Transform chatContent, playersContent;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    public string logged_key, room_owner;
    public string roomId, errorMessage;
    public string fname, floses, fwins, fnemesis, fprefg, ffavu;
    public List<PlayerClass> room_players;
    public List<int> id_players;
    public bool ready;
    public long capacity;
    public int ready_players;

    [Obsolete]
    void Start()
    {
        logged_key = PlayerPrefs.GetString("UID");
        roomId = PlayerPrefs.GetString("Room");
        idLabel.text = roomId;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        room_players = new List<PlayerClass>();
        capacity = 0;
        ready = false;
        ready_players = 0;
        errorMessage = "";
        fname = "";
        floses = "";
        fwins = "";
        fnemesis = "";
        fprefg = "";
        ffavu = "";
        SetupDisplay();
        dbInstance.GetReference("rooms").Child(roomId).Child("messages").ChildAdded += HandleMessageAdded;
        dbInstance.GetReference("rooms").Child(roomId).Child("players").ChildRemoved += HandlePlayerRemoved;
        dbInstance.GetReference("rooms").Child(roomId).Child("players").ChildChanged += HandlePlayerEdited;
        dbInstance.GetReference("rooms").Child(roomId).Child("start").ValueChanged += HandleStartGame;
        sendButton.onClick.AddListener(() => SenderChat(chatInput.text));
        leaveButton.onClick.AddListener(() => LeavePress());
        readyButton.onClick.AddListener(() => ReadyPress());
        startButton.onClick.AddListener(() => StartGame());
        backProfileButton.onClick.AddListener(() => HideProfileCanvas());
    }

    void HandleStartGame(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot msg = args.Snapshot;
        string status = msg.Value.ToString();
        if (status == "true")
        {
            SceneManager.LoadScene("Game");
        }
    }

    void HandlePlayerEdited(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot msg = args.Snapshot;
        IDictionary dictPlayer = (IDictionary)msg.Value;
        if (dictPlayer["ready"].ToString() == "true")
        {
            ready_players += 1;
        }
        else
        {
            ready_players -= 1;
        }
        playersLabel.text = "Players" + ready_players.ToString() + "/" + capacity.ToString();
    }
    void HandleMessageAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot msg = args.Snapshot;
        IDictionary dictMsg = (IDictionary)msg.Value;
        Message m_row = new Message(dictMsg["from"].ToString(), dictMsg["content"].ToString());
        GameObject SpawnedItem = Instantiate(chatRow);
        SpawnedItem.transform.SetParent(chatContent, false);
        SpawnedItem.transform.GetChild(0).GetComponent<Text>().text = m_row.from;
        SpawnedItem.transform.GetChild(1).GetComponent<Text>().text = m_row.content;
    }

    void HandlePLayerAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot data = args.Snapshot;
        IDictionary dictPlayer = (IDictionary)data.Value;
        string temp_name = dictPlayer["username"].ToString();
        PlayerClass p_row = new PlayerClass(dictPlayer["username"].ToString(), dictPlayer["ready"].ToString());
        for(int p = 0; p < room_players.Count; p++)
        {
            Debug.Log(room_players[p].username);
            Debug.Log(p_row.username);
            if (p_row.username == room_players[p].username)
            {
                return;
            }
        }
        if (p_row.username != PlayerPrefs.GetString("UserName"))
        {
            GameObject SpawnedItem = Instantiate(playersRow);
            SpawnedItem.transform.SetParent(playersContent, false);
            SpawnedItem.transform.GetChild(0).GetComponent<Text>().text = p_row.username;
            SpawnedItem.transform.GetChild(1).GetComponent<Text>().text = " ";
            SpawnedItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DisplayProfile(temp_name));
        }
        
    }

    void HandlePlayerRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot data = args.Snapshot;
        IDictionary dictPlayer = (IDictionary)data.Value;
        foreach (Transform jugadores in playersContent)
        {
            if (jugadores.transform.GetChild(0).GetComponent<Text>().text == dictPlayer["username"].ToString())
            {
                Debug.Log(jugadores.transform.GetChild(0).GetComponent<Text>().text);
                jugadores.gameObject.SetActive(false);
            }
        }
        if (data.Key == room_owner)
        {
            dbInstance.GetReference("rooms").Child(roomId).Child("players").ChildRemoved -= HandlePlayerRemoved;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public async void SetupDisplay()
    {
        await dbInstance.GetReference("rooms").Child(roomId).GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            IDictionary room = (IDictionary)snapshot.Value;
            capacity = long.Parse(room["roomSize"].ToString());
            room_owner = room["owner"].ToString();

        });
        await dbInstance.GetReference("rooms").Child(roomId).Child("players").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot p in snapshot.Children)
                {
                    IDictionary dictPlayer = (IDictionary)p.Value;
                    PlayerClass p_row = new PlayerClass(dictPlayer["username"].ToString(), dictPlayer["ready"].ToString());
                    room_players.Add(p_row);
                    if (dictPlayer["ready"].ToString() == "true")
                    {
                        ready_players += 1;
                    }                   
                }

            }
        });
        playersLabel.text = "Players" + ready_players.ToString() + "/" + capacity.ToString();
        for (int i = 0; i < room_players.Count; i++)
        {

            GameObject SpawnedItem = Instantiate(playersRow);
            string temp_name = room_players[i].username;
            SpawnedItem.transform.SetParent(playersContent, false);
            SpawnedItem.transform.GetChild(0).GetComponent<Text>().text = temp_name;
            SpawnedItem.transform.GetChild(1).GetComponent<Text>().text = " ";
            SpawnedItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DisplayProfile(temp_name));
        }
        dbInstance.GetReference("rooms").Child(roomId).Child("players").ChildAdded += HandlePLayerAdded;
    }

    public async void ReadyPress()
    {
        if (ready == false)
        {
            await reference.Child("rooms").Child(roomId).Child("players").Child(logged_key).Child("ready").SetValueAsync("true");
            ready = true;
        }
        else
        {
            await reference.Child("rooms").Child(roomId).Child("players").Child(logged_key).Child("ready").SetValueAsync("false");
            ready = false;
        }

    }

    public async void LeavePress()
    {
        dbInstance.GetReference("rooms").Child(roomId).Child("players").ChildRemoved -= HandlePlayerRemoved;
        await reference.Child("rooms").Child(roomId).Child("players").Child(logged_key).RemoveValueAsync();
        if (logged_key == room_owner)
        {
            Debug.Log("Eliminando Sala");
            await reference.Child("rooms").Child(roomId).RemoveValueAsync();
        }
        SceneManager.LoadScene("MainMenu");
    }

    public async void StartGame()
    {
        Message.text = "";
        await dbInstance.GetReference("rooms").Child(roomId).Child("players").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.ChildrenCount != capacity)
                {
                    errorMessage = "Not enough players";
                    return;

                }
                foreach (DataSnapshot p in snapshot.Children)
                {
                    IDictionary dictPlayer = (IDictionary)p.Value;
                    if (dictPlayer["ready"].ToString() == "false")
                    {
                        ready = false;
                        errorMessage = "All players must be ready";
                        break;
                    }
                    else
                    {
                        ready = true;
                    }

                }
            }
        });
        if (ready == false)
        {
            Message.text = errorMessage;
            return;
        }
        await reference.Child("rooms").Child(roomId).Child("start").SetValueAsync("true");
        SceneManager.LoadScene("Game");
    }

    public async void SenderChat(string content)
    {
        chatInput.text = "";
        Message message = new Message(PlayerPrefs.GetString("UserName"), content);
        string json = JsonUtility.ToJson(message);
        await reference.Child("rooms").Child(roomId).Child("messages").Push().SetRawJsonValueAsync(json);
    }

    public async void DisplayProfile(string name)
    {
        await dbInstance.GetReference("users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children)
                {
                    IDictionary Duser = (IDictionary)user.Value;
                    if (Duser["username"].ToString() == name)
                    {
                        fname = Duser["username"].ToString();
                        floses = "Loses: " + Duser["loses"].ToString();
                        fwins = "Wins: " + Duser["wins"].ToString();
                        fnemesis = "Nemesis: " + Duser["nemesis"].ToString();
                        fprefg = "Prefered Game: " + Duser["pref_game"].ToString();
                        ffavu = "Favorite Unit: " + Duser["fav_unit"].ToString();
                    }
                }
            }
        });
        friendName.text = fname;
        friendWins.text = fwins;
        friendLoses.text = floses;
        prefFriend.text = fprefg;
        friendPrefU.text = ffavu;
        nemesisFriend.text = fnemesis;
        profileCanvas.gameObject.SetActive(true);
        roomCanvas.gameObject.SetActive(false);
    }

    public void HideProfileCanvas()
    {
        roomCanvas.gameObject.SetActive(true);
        profileCanvas.gameObject.SetActive(false);
    }

}