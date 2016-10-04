using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

    public Elevator otherElevator;
    public Chad thePlayer;

    public bool atBottom;
    public bool moving = false;
    public bool beingMoved = false;

    public int speed = 1;

	// Use this for initialization
	void Start () {
        atBottom = false;
        thePlayer = FindObjectOfType<Chad>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (moving)
        {
            transform.position += Vector3.down * speed;
            thePlayer.transform.position += Vector3.down * speed;
        }
        else if (beingMoved)
        {
            transform.position += Vector3.up * speed;
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("ElevatorStopDown"))
        {
            atBottom = true;
            moving = false;
            otherElevator.beingMoved = false;
            thePlayer.myState.canMoveLeft = true;
            thePlayer.myState.canMoveRight = true;
            thePlayer.myState.ridingElevator = false;
        }
        else if (col.gameObject.CompareTag("Player"))
        {
            if(atBottom == false)
            {
                moving = true;
                thePlayer.myState.canMoveLeft = false;
                thePlayer.myState.canMoveRight = false;
                thePlayer.myState.logsTouching--;
                otherElevator.beingMoved = true;
                otherElevator.atBottom = false;
            }
        }
    }
}
