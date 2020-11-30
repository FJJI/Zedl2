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
using System.Text.RegularExpressions;

public class Winner : MonoBehaviour
{
    public Button Menu;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    string myId;

    private void Awake()
    {
        Destroy(GameObject.Find("Data"));
    }

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        Menu.onClick.AddListener(() => GoToMenu());
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference; //escritura
        dbInstance = FirebaseDatabase.DefaultInstance; //lectura
        myId = PlayerPrefs.GetString("UID");
        setWin_Loses_stats();
        DeleteRoom();
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    async void DeleteRoom()
    {
        await reference.Child("rooms").Child(PlayerPrefs.GetString("rooms")).RemoveValueAsync();
    }

    async void setWin_Loses_stats()
    {
        List<string> ids = new List<string>();
        await dbInstance.GetReference("rooms").Child(PlayerPrefs.GetString("rooms")).Child("players").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot player in snapshot.Children)
                {
                    ids.Add(player.Key.ToString());
                }
            }
        });
        for (int i = 0; i < ids.Count; i++)
        {
            int vsWin = 0;
            int FriendLoses = 0;
            int friendVsLoses = 0;
            int winVsFriend = 0;

            // aumentar en 1 la derreta ante mi de mi amigo
            await dbInstance.GetReference("friendLists").Child(ids[i]).Child("friends").Child(myId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    IDictionary friend = (IDictionary)snapshot.Value;
                    friendVsLoses = int.Parse(friend["loses"].ToString());
                }
            });
            await reference.Child("friendLists").Child(ids[i]).Child("friends").Child(myId).Child("loses").SetValueAsync(friendVsLoses + 1);

            // aumentar en 1 mi victoria frente a mi amigo
            await dbInstance.GetReference("friendLists").Child(myId).Child("friends").Child(ids[i]).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    IDictionary friend = (IDictionary)snapshot.Value;
                    winVsFriend = int.Parse(friend["loses"].ToString());
                }
            });
            await reference.Child("friendLists").Child(myId).Child("friends").Child(ids[i]).Child("wins").SetValueAsync(winVsFriend + 1);

            // aumentar en 1 la esdistica global de derrotas de mi amigo
            await dbInstance.GetReference("users").Child(ids[i]).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    IDictionary friend = (IDictionary)snapshot.Value;
                    FriendLoses = int.Parse(friend["loses"].ToString());
                }
            });
            await reference.Child("users").Child(ids[i]).Child("loses").SetValueAsync(FriendLoses + 1);

            //Aumentar en 1 mis victorias globales
            await dbInstance.GetReference("users").Child(myId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    IDictionary user = (IDictionary)snapshot.Value;
                    vsWin = int.Parse(user["wins"].ToString());
                }
            });
            await reference.Child("users").Child(myId).Child("wins").SetValueAsync(vsWin + 1);
        }
    }
}
