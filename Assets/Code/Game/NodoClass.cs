using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoClass // En teoria Firebase puede guardar clases 
{
    public float posx;
    public float posy;
    public float posz;

    public int owner;
    public int total_unions;
    public int used_unions;
    public int type;
    public int points;
    public int healingFactor;
    public int dmgFactor;
    public int identifier;
    public List<int> objectives;
    public NodoClass(GameObject nodo)
    {
        this.posx = nodo.transform.position.x;
        this.posy = nodo.transform.position.y;
        this.posz = nodo.transform.position.z;

        Nodo data_nodo = nodo.GetComponent<Nodo>();
        this.type = data_nodo.type;
        this.points = data_nodo.points;
        this.total_unions = data_nodo.total_unions;
        this.used_unions = data_nodo.used_unions;
        this.owner = data_nodo.owner;
        this.healingFactor = data_nodo.healingFactor;
        this.dmgFactor = data_nodo.dmgFactor;
        this.identifier = data_nodo.identifier;

        objectives = new List<int>();
    }
    public void addObjective(int data)
    {
        objectives.Add(data);
    }

}
