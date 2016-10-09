using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scent : MonoBehaviour {

    public float odorStrength;
    public float age = 0;
    public bool active = false;
    public static List<Scent> scents;

    void Awake()
    {
        if (scents == null)
        {
            scents = new List<Scent>();
        }
        scents.Add(this);
        Debug.Log(scents.Count);
    }

    // Use this for initialization
    void Start() {
    }
           
	// Update is called once per frame
	void Update () {
        if (active)
            age++;
	}

    public void Smell()
    {
        age = 0;
        active = true;
        gameObject.layer = LayerMask.NameToLayer("Scent");
        GetComponent<SpriteRenderer>().enabled = true;
        Invoke("Dissipate", odorStrength);
    }

    void Dissipate()
    {
        active = false;
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.layer = 0;
    }
}
