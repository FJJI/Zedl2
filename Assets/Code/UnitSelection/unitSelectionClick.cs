using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitSelectionClick : MonoBehaviour
{
    // Start is called before the first frame update
    public string type;   
    public GameObject controller;

    void OnMouseDown()
    {
        controller.GetComponent<UnitSelection>().selected_unit=type;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
