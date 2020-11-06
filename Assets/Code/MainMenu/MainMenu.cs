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

public class MainMenu : MonoBehaviour
{
    public InputField RoomCode, FriendToAdd;
    public Button FriendListButton, ProfileButton, CreateRoomButton, AccessRoomButton, RequestsButton, FriendsButton, AddFriend, BackButton, LogoutButton;
    public GameObject FriendsView, RequestsView;
    public GameObject FriendRow, RequestRow;
    public Transform FriendContent, RequestContent;
    public Text ErrorMessage;
    public List<Request> requestsL;
    public List<Friend> friendsL;
    bool found, friend_showed, request_showed, room_exist, room_full, owner_friend;
    long rows;
    int room_size;
    public string logged_key, target, room_owner, friend_code;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;

    [Obsolete]
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        found = false;
        room_exist = false;
        friend_showed = false;
        friend_code = "";
        request_showed = false;
        target = "";
        rows = 0;
        room_exist = false;
        room_full = false;
        room_owner = "";
        owner_friend = false;
        room_size = 0;
        requestsL = new List<Request>();
        friendsL = new List<Friend>();
        logged_key = PlayerPrefs.GetString("UID");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        ProfileButton.onClick.AddListener(() => DisplayProfile());
        FriendListButton.onClick.AddListener(() => DisplayFriends(logged_key));
        FriendsButton.onClick.AddListener(() => DisplayFriends(logged_key));
        RequestsButton.onClick.AddListener(() => DisplayRequests(logged_key));
        BackButton.onClick.AddListener(() => Back());
        AddFriend.onClick.AddListener(() => AddFriends(logged_key, FriendToAdd.text));
        LogoutButton.onClick.AddListener(() => Logout());
        CreateRoomButton.onClick.AddListener(() => CreateRoom());
        AccessRoomButton.onClick.AddListener(() => JoinRoom(RoomCode.text));

    }

    public async void DisplayFriends(string key)
    {
        int initial_rows = 0;
        if (friend_showed == true)
        {
            initial_rows = friendsL.Count;
        }
        friend_showed = true;
        FriendListButton.gameObject.SetActive(false);
        ProfileButton.gameObject.SetActive(false);
        CreateRoomButton.gameObject.SetActive(false);
        AccessRoomButton.gameObject.SetActive(false);
        RoomCode.gameObject.SetActive(false);
        RequestsButton.gameObject.SetActive(true);
        FriendsButton.gameObject.SetActive(true);
        AddFriend.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
        FriendToAdd.gameObject.SetActive(true);
        FriendsView.gameObject.SetActive(true);
        RequestsView.gameObject.SetActive(false);
        LogoutButton.gameObject.SetActive(false);
        // obtenemos todos los amigos de la lista
        //Primero Consultamos a la Base de datos por nuestra lista de amigos
        await dbInstance.GetReference("friendLists").Child(key).Child("friends").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Vemos si es que tenemos amigos
                rows = snapshot.ChildrenCount;
                // Si hay amigos, los agregamos a una lista que usaremos para las rondas.
                if (rows > 0)
                {
                    foreach (DataSnapshot friend in snapshot.Children)
                    {
                        bool added = false;
                        IDictionary dictFriend = (IDictionary)friend.Value;
                        Friend f_row = new Friend(dictFriend["friend_name"].ToString(), dictFriend["friend_id"].ToString(), 0, 0);
                        foreach (Friend f in friendsL)
                        {
                            if (f.friend_id == f_row.friend_id)
                            {
                                added = true;
                            }
                        }
                        if (added == false)
                        {
                            friendsL.Add(f_row);
                        }
                    }
                }

            }
        });
        // Aqui empezamos el ciclo para agregar cada row al FriendsView
        for (int i = initial_rows; i < friendsL.Count; i++)
        {
            string f_id = friendsL[i].friend_id;
            GameObject SpawnedItem = Instantiate(FriendRow);
            SpawnedItem.transform.SetParent(FriendContent, false);
            SpawnedItem.transform.GetChild(0).GetComponent<Text>().text = friendsL[i].friend_name;
            SpawnedItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => DeleteFriend(logged_key, f_id, SpawnedItem));

        }

    }

    public async void DisplayRequests(string key)
    {
        int initial_rows = 0;
        if (request_showed == true)
        {
            initial_rows = requestsL.Count;
        }
        request_showed = true;
        FriendsView.gameObject.SetActive(false);
        RequestsView.gameObject.SetActive(true);
        // obtenemos todas las solicitudes recibidas en nuestra lista de requests
        //Primero Consultamos a la Base de datos por nuestra lista de solicitudes
        await dbInstance.GetReference("requestLists").Child(key).Child("requests").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Vemos si es que tenemos solicitudes
                rows = snapshot.ChildrenCount;
                // Si hay solicitudes, las agregamos a una lista que usaremos para las rondas.
                if (rows > 0)
                {
                    foreach (DataSnapshot request in snapshot.Children)
                    {
                        IDictionary dictRequest = (IDictionary)request.Value;
                        Request r_row = new Request(dictRequest["from_id"].ToString(), dictRequest["from_name"].ToString(), dictRequest["target_id"].ToString());
                        requestsL.Add(r_row);
                    }
                }
                else
                {
                    return;
                }

            }
        });
        // Aqui empezamos el ciclo para agregar cada row al requestsView

        for (int i = initial_rows; i < requestsL.Count; i++)
        {
            string f_name = requestsL[i].from_name;
            string f_id = requestsL[i].from_id;
            GameObject SpawnedItem = Instantiate(RequestRow);
            SpawnedItem.transform.SetParent(RequestContent, false);
            SpawnedItem.transform.GetChild(0).GetComponent<Text>().text = requestsL[i].from_name;
            SpawnedItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                AcceptFriend(logged_key, SpawnedItem.transform.GetChild(0).GetComponent<Text>().text, f_id, SpawnedItem));
            SpawnedItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
                RefuseRequest(logged_key, f_id, SpawnedItem));
        }

    }

    public async void AddFriends(string key, string friend_name)
    {
        //write to target user's request list
        await dbInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children)
                {
                    IDictionary dictUser = (IDictionary)user.Value;
                    if (dictUser["username"].ToString() == friend_name)
                    {                        
                        friend_code = user.Key.ToString();
                        found = true;
                        break;

                    }
                }

            }
        });

        if (found == false)
        {
            ErrorMessage.text = "User not Found";
        }
        else
        {
            ErrorMessage.text = "Resquest sended";
            Request request = new Request(key, PlayerPrefs.GetString("UserName"), friend_code);
            string json = JsonUtility.ToJson(request);
            await reference.Child("requestLists").Child(friend_code).Child("requests").Child(logged_key).SetRawJsonValueAsync(json);

        }

    }

    public async void AcceptFriend(string key, string name, string friend_key, GameObject spwaned)
    {
        await reference.Child("requestLists").Child(key).Child("requests").Child(friend_key).RemoveValueAsync();
        Friend friend1 = new Friend(name, friend_key, 0, 0); //friend in my list.
        Friend friend2 = new Friend(PlayerPrefs.GetString("UserName"), key, 0, 0); //friend in his list.
        string json = JsonUtility.ToJson(friend1);
        await reference.Child("friendLists").Child(key).Child("friends").Child(friend_key).SetRawJsonValueAsync(json);
        json = JsonUtility.ToJson(friend2);
        await reference.Child("friendLists").Child(friend_key).Child("friends").Child(key).SetRawJsonValueAsync(json);
        Destroy(spwaned);
    }

    public async void RefuseRequest(string key, string friend_key, GameObject spawned)
    {
        await reference.Child("requestLists").Child(key).Child("requests").Child(friend_key).RemoveValueAsync();
        Destroy(spawned);
    }

    public async void DeleteFriend(string key, string friend_key, GameObject spawned)
    {
        await reference.Child("friendLists").Child(key).Child("friends").Child(friend_key).RemoveValueAsync();
        await reference.Child("friendLists").Child(friend_key).Child("friends").Child(key).RemoveValueAsync();
        Destroy(spawned);
    }

    public void Back()
    {
        FriendListButton.gameObject.SetActive(true);
        ProfileButton.gameObject.SetActive(true);
        CreateRoomButton.gameObject.SetActive(true);
        AccessRoomButton.gameObject.SetActive(true);
        RoomCode.gameObject.SetActive(true);
        RequestsButton.gameObject.SetActive(false);
        FriendsButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        FriendToAdd.gameObject.SetActive(false);
        AddFriend.gameObject.SetActive(false);
        FriendsView.gameObject.SetActive(false);
        RequestsView.gameObject.SetActive(false);
        LogoutButton.gameObject.SetActive(true);

    }

    public void DisplayProfile()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Login");
    }

    public void CreateRoom()
    {
        SceneManager.LoadScene("CreateRoomScene");
    }

    public async void JoinRoom(string roomId)
    {
        Debug.Log("Buscando Sala");
        await dbInstance.GetReference("rooms").Child(roomId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot == null)
                {
                    room_exist = false;
                }
                else
                {
                    room_exist = true;
                    IDictionary roomDict = (IDictionary)snapshot.Value;
                    room_size = int.Parse(roomDict["roomSize"].ToString());
                    room_owner = roomDict["owner"].ToString();
                }

            }
        });

        if (room_exist == true)
        {
            Debug.Log("Verificando Amistad");
            await dbInstance.GetReference("friendLists").Child(logged_key).Child("friends").Child(room_owner).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted) { }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot == null)
                    {
                        owner_friend = false;
                    }
                    else
                    {
                        owner_friend = true;
                    }
                }
            });
            if (owner_friend == false)
            {
                ErrorMessage.text = "No eres amigo del dueño de la sesion";
                return;
            }
            Debug.Log("Comprobando estado de la Sala");
            await dbInstance.GetReference("rooms").Child(roomId).Child("players").GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted) { }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    int players = int.Parse(snapshot.ChildrenCount.ToString());
                    if (players < room_size)
                    {
                        room_full = false;
                    }
                    else
                    {
                        room_full = true;
                    }

                }
            });
            if (room_full == false)
            {
                Debug.Log("Ingresando a Sala");
                PlayerPrefs.SetString("Room", roomId);
                PlayerClass player = new PlayerClass(PlayerPrefs.GetString("UserName"), "false");
                string json = JsonUtility.ToJson(player);
                await reference.Child("rooms").Child(roomId.ToString()).Child("players").Child(PlayerPrefs.GetString("UID")).SetRawJsonValueAsync(json);
                SceneManager.LoadScene("RoomScene");

            }
            else
            {
                //indicar sala llena
            }
        }
    }

    public void Wait(float seconds, Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }

    IEnumerator _wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

}
