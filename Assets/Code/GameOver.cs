using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Button Menu;

    private void Awake()
    {
        Destroy(GameObject.Find("Data"));
    }

    // Start is called before the first frame update
    void Start()
    {
        Menu.onClick.AddListener(() => GoToMenu());
    }

    void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // Update is called once per frame
    
}
