using UnityEngine;
using System.Collections;

public class Cage : Moveable {

    public bool falling = false;
    public float fallSpeed = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (falling)
        {
            transform.position += Vector3.down * fallSpeed;
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CenterLine"))
        {
            Debug.Log("THE CAGE!");
            falling = false;
            transform.position = new Vector3(transform.position.x, col.transform.position.y, transform.position.z);
        }
    }
}
