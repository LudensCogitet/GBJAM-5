using UnityEngine;
using System.Collections;

public class Chad : MonoBehaviour {

    public class pState
    {
        public bool initialLoop = true;

        public bool falling = false;
        public bool stopFalling = false;

        public bool canClimbUp = false;
        public bool stopClimbingUp = false;

        public bool canClimbDown = false;
        public bool stopClimbingDown = false;


        public bool climbing = false;
        

        public bool ridingElevator = false;
    }

    public pState myState;
    public float walkSpeed = 2;
    public float fallSpeed = 1;
    public float climbSpeed = 0.5f;

    public GameObject targetLadder = null;


    void Awake()
    {
        myState = new pState();
    }

	// Use this for initialization
	void Start () {
	    
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (myState.falling == false)
        {
            if (myState.climbing == false)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) { transform.position += Vector3.left * walkSpeed; }
                if (Input.GetKey(KeyCode.RightArrow)) { transform.position += Vector3.right * walkSpeed; }

                if (myState.canClimbUp == true)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        myState.climbing = true;
                        transform.position = new Vector3(targetLadder.transform.position.x, transform.position.y, transform.position.z);
                    }

                }
                if (myState.canClimbDown == true)
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        myState.climbing = true;
                        transform.position = new Vector3(targetLadder.transform.position.x, transform.position.y, transform.position.z);
                    }
                }
            }
            else if (myState.climbing == true)
            {
                if (myState.canClimbUp)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                        transform.position += Vector3.up * climbSpeed;
                }
                /*else if(myState.stopClimbingUp == true)
                {
                    myState.climbing = false;
                    myState.stopClimbingUp = false;
                    //transform.position += Vector3.up * climbSpeed;
                }*/

                if (myState.canClimbDown)
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                        transform.position += Vector3.down * climbSpeed;
                }
                /*else if (myState.stopClimbingDown == true)
                {
                    myState.climbing = false;
                    myState.stopClimbingDown = false;
                    //transform.position += Vector3.down * climbSpeed;
                }*/
            }
        }
        else if (myState.falling == true)
        {
            transform.position += Vector3.down * fallSpeed;
            if(myState.stopFalling == true)
            {
                myState.falling = false;
                myState.stopFalling = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("OnTriggerEnter: " + col.gameObject.tag);
        if (myState.climbing == false)
        {
            if (col.gameObject.CompareTag("Log"))
            {
                if (myState.initialLoop)
                    myState.initialLoop = false;
                else
                    //myState.stopFalling = true;
                    myState.falling = false;
            }
        }

        if (col.gameObject.CompareTag("StopClimbUp"))
        {
            myState.canClimbUp = false;
            myState.canClimbDown = true;
            if (myState.climbing == true)
            {
                myState.stopClimbingUp = true;
                myState.climbing = false;
            }
        }

        if (col.gameObject.CompareTag("StopClimbDown"))
        {
            myState.canClimbUp = true;
            myState.canClimbDown = false;
            if (myState.climbing == true)
            {
                myState.stopClimbingDown = true;
                myState.climbing = false;
            }
        }


        if(col.gameObject.CompareTag("Ladder"))
            targetLadder = col.gameObject;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("OnTriggerExit: " + col.gameObject.tag);
        if (myState.climbing == false)
        {
            if (col.gameObject.CompareTag("Log"))
            {
                myState.falling = true;
            }
        }
        else if (myState.climbing == true)
        {
            if (col.gameObject.CompareTag("StopClimbDown"))
                myState.canClimbDown = true;
            if (col.gameObject.CompareTag("StopClimbUp"))
                myState.canClimbUp = true;
        }

        if (col.gameObject.CompareTag("Ladder"))
        {
            myState.canClimbUp = false;
            myState.canClimbDown = false;
            targetLadder = null;
        }
    }
}
