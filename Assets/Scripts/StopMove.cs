using UnityEngine;
using System.Collections;

public class StopMove : MonoBehaviour {

    public Moveable myMoveable;
    BoxCollider2D myBox;

	// Use this for initialization
	void Start () {
        myBox = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("StopMove"))
        {
            if (myMoveable != null)
            {
                if (myMoveable.transform.position.x > col.gameObject.transform.position.x)
                    myMoveable.canMoveLeft = false;
                else
                    myMoveable.canMoveRight = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("StopMove"))
        {
            if (myMoveable.transform.position.x > col.gameObject.transform.position.x)
                myMoveable.canMoveLeft = true;
            else
                myMoveable.canMoveRight = true;

        }
    }
}
