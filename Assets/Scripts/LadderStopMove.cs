using UnityEngine;
using System.Collections;

public class LadderStopMove : MonoBehaviour {

    public Ladder myLadder;
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
            if (myLadder != null)
            {
                if (myLadder.transform.position.x > col.gameObject.transform.position.x)
                    myLadder.canMoveLeft = false;
                else
                    myLadder.canMoveRight = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("StopMove"))
        {
            if (myLadder.transform.position.x > col.gameObject.transform.position.x)
                myLadder.canMoveLeft = true;
            else
                myLadder.canMoveRight = true;

        }
    }
}
