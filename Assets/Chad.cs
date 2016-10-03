using UnityEngine;
using System.Collections;

public class Chad : MonoBehaviour {

    public class pState
    {
        public bool initialLoop = true;
        public bool falling = false;
        public bool stopFalling = false;
        public bool climbing = false;
        public bool ridingElevator = false;
        public bool canClimb = false;
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
    void Update()
    {
        if (myState.falling == false)
        {
            if (myState.climbing == false)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) { transform.position += Vector3.left * walkSpeed; }
                if (Input.GetKey(KeyCode.RightArrow)) { transform.position += Vector3.right * walkSpeed; }
                
                if(myState.canClimb == true)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        if (myState.climbing == false)
                        {
                            myState.climbing = true;
                            transform.position = new Vector3(targetLadder.transform.position.x, transform.position.y, transform.position.z);
                        }
                        else
                            transform.position += Vector3.up * climbSpeed;
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        if (myState.climbing == false)
                        {
                            myState.climbing = true;
                            transform.position = new Vector3(targetLadder.transform.position.x, transform.position.y, transform.position.z);
                        }
                        else
                            transform.position += Vector3.down * climbSpeed;
                    }
                }
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
        Debug.Log("Hi!");
        if (col.gameObject.CompareTag("Log"))
        {
            if (myState.initialLoop)
                myState.initialLoop = false;
            else
                myState.stopFalling = true;
        }
        else if (col.gameObject.CompareTag("Ladder"))
        {
            myState.canClimb = true;
            targetLadder = col.gameObject;
        }
    }

void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("bye!");
        if (col.gameObject.CompareTag("Log"))
        {
            myState.falling = true;
        }
        else if (col.gameObject.CompareTag("Ladder"))
        {
            myState.canClimb = false;
        }
    }
}
