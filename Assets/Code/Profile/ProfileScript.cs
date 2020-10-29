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

public class ProfileScript : MonoBehaviour
{
    public string logged_key, password, email, wins, loses, fav_game, fav_unit, nemesis;
    private DatabaseReference reference;
    private FirebaseDatabase dbInstance;
    public Text idLabel, userValue, emailValue, passValue, winsLabel, losesLabel, prefGLabel, FavULabel, NemesisLabel;
    public InputField userInput, emailInput, newPassInput, oldPassInput;
    public Button editButton, saveButton, backButton, cancelButton;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zeldnew.firebaseio.com/");
        logged_key = PlayerPrefs.GetString("UID");
        password = null;
        email = null;
        wins = null;
        loses = null;
        fav_game = null;
        fav_unit = null;
        nemesis = null;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        dbInstance = FirebaseDatabase.DefaultInstance;
        GetUserData();
        editButton.onClick.AddListener(() => EditProfile());
        saveButton.onClick.AddListener(() => SaveChanges(userInput.text, emailInput.text, newPassInput.text.ToString(), oldPassInput.text.ToString()));
        cancelButton.onClick.AddListener(() => CancelChanges());
        backButton.onClick.AddListener(() => BackToMenu());
    }

    public async void GetUserData()
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
                IDictionary dictUser = (IDictionary)snapshot.Value;
                password = dictUser["password"].ToString();
                email = dictUser["email"].ToString();
                wins = dictUser["wins"].ToString();
                loses = dictUser["loses"].ToString();
                nemesis = dictUser["nemesis"].ToString();
                fav_game = dictUser["pref_game"].ToString();
                fav_unit = dictUser["fav_unit"].ToString();

            }
        });
        idLabel.text = "Id: " + logged_key;
        userValue.text = PlayerPrefs.GetString("UserName");
        emailValue.text = email;
        passValue.text = password;
        winsLabel.text = "Wins: " + wins;
        losesLabel.text = "Loses: " + loses;
        prefGLabel.text = "Prefered Game: " + fav_game;
        FavULabel.text = "Favorite initial Unit: " + fav_unit;
        SetNemesis();
    }

    public void EditProfile()
    {
        editButton.gameObject.SetActive(false);
        userInput.gameObject.SetActive(true);
        userInput.text = userValue.text;
        emailInput.gameObject.SetActive(true);
        emailInput.text = emailValue.text;
        newPassInput.gameObject.SetActive(true);
        newPassInput.text = "";
        oldPassInput.gameObject.SetActive(true);
        oldPassInput.text = "";
        saveButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    public async void SaveChanges(string usernameS, string emailS, string new_passwordS, string current_passwordS)
    {
        if (current_passwordS == "")
        {
            oldPassInput.text = "Current Pass Required";
            return;
        }
        else if (new_passwordS == "")
        {
            new_passwordS = current_passwordS;
        }
        else if (current_passwordS != password)
        {
            oldPassInput.text = "Wrong password";
            return;
        }
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(email);
        if (match.Success)
        {
            User new_user = new User(usernameS, emailS, new_passwordS, int.Parse(wins), int.Parse(loses), nemesis, fav_game, fav_unit);
            string json = JsonUtility.ToJson(new_user);
            await reference.Child("users").Child(logged_key).SetRawJsonValueAsync(json);
            PlayerPrefs.SetString("UserName", usernameS);
            GetUserData();
            CancelChanges();
        }
        else
        {
            emailInput.text = "invalid email format";
            return;
        }

    }

    public void CancelChanges()
    {
        editButton.gameObject.SetActive(true);
        userInput.gameObject.SetActive(false);
        emailInput.gameObject.SetActive(false);
        newPassInput.gameObject.SetActive(false);
        oldPassInput.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    public async void SetNemesis()
    {
        int loses = 0;
        string nem = "";
        await dbInstance.GetReference("friendLists").Child(logged_key).Child("friends").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted) { }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot friend in snapshot.Children)
                {
                    IDictionary fDict = (IDictionary)friend.Value;
                    int f_loses = int.Parse(fDict["loses"].ToString());
                    if (f_loses > loses)
                    {
                        loses = f_loses;
                        nem = fDict["friend_name"].ToString();
                    }
                }
            }

        });
        NemesisLabel.text = "Nemesis: " + nem;
        await reference.Child("users").Child(logged_key).Child("nemesis").SetValueAsync(nem);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}