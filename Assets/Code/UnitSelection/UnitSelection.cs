using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public class UnitSelection : MonoBehaviour
{
    public string selected_unit;
    public SpriteRenderer normalSprite;
    public SpriteRenderer attackSprite;
    public SpriteRenderer defenseSprite;
    public SpriteRenderer extraSprite;
    public string logged_key;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    private Color yellow;
    private Color white;
    IDictionary dictUser;

    // Start is called before the first frame update
    void Start()
    {
        yellow = new Color(1f,1f,0f,1);
        white = new Color(1f,1f,1f,1);
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        logged_key = PlayerPrefs.GetString("UID");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        dbInstance = FirebaseDatabase.DefaultInstance;
        GetUserFavUnit();
        
    }

    // Update is called once per frame
    void Update()
    {
        paintSelected(selected_unit);
    }

    void paintSelected(string selectedType)
    {
        switch (selectedType)
                {
                    case "0":
                        normalSprite.color = yellow;
                        attackSprite.color = white;
                        defenseSprite.color = white;
                        extraSprite.color = white;
                        break;
                    case "1":
                        normalSprite.color = white;
                        attackSprite.color = yellow;
                        defenseSprite.color = white;
                        extraSprite.color = white;
                        break;
                    case "2":
                        normalSprite.color = white;
                        defenseSprite.color = yellow;
                        extraSprite.color = white;
                        attackSprite.color = white;
                        break;
                    case "3":
                        normalSprite.color = white;
                        defenseSprite.color = white;
                        attackSprite.color = white;
                        extraSprite.color = yellow;
                        break;
                    default:
                        normalSprite.color = white;
                        defenseSprite.color = white;
                        attackSprite.color = white;
                        extraSprite.color = white;
                        break;
                }
    }


    public async void GetUserFavUnit()
    {
        await dbInstance.GetReference("users").Child(logged_key).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                dictUser = (IDictionary)snapshot.Value;
                selected_unit=dictUser["fav_unit"].ToString();
                Debug.Log(selected_unit);
                Debug.Log(dictUser["username"].ToString());
            }
        });
    }

    public async void changeFavUnitFromDB()
    {
        User new_user = new User(dictUser["username"].ToString(), dictUser["email"].ToString(),
        dictUser["password"].ToString(), int.Parse(dictUser["wins"].ToString()),
        int.Parse(dictUser["loses"].ToString()),dictUser["nemesis"].ToString(),
        dictUser["pref_game"].ToString(),selected_unit);
        string json = JsonUtility.ToJson(new_user);
        //await reference.Child("users").Push().SetRawJsonValueAsync(json);
        //push crea nuevo, hay que cambiarlo por uno que solo modifique
    }


}
