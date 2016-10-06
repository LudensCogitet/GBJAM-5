using UnityEngine;
using System.Collections;

public class Scent : MonoBehaviour {

    public float odorStrength;
    public float age = 0;
    public bool active = false;

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
        Invoke("Dissipate", odorStrength);
    }

    void Dissipate()
    {
        active = false;
        gameObject.layer = 0;
    }
}
