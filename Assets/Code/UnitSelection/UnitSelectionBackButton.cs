using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySelectionBackButton : MonoBehaviour
{
    public GameObject controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        controller.GetComponent<UnitSelection>().changeFavUnitFromDB();
        //and change scene
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
