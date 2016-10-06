using UnityEngine;
using System.Collections;

public class Chad : MonoBehaviour {

    public class pState
    {
        public int logsTouching = 0;

        public bool initialLoop = true;

        public bool falling = false;
        public bool stopFalling = false;

        public bool canClimbUp = false;

        public bool canClimbDown = false;

        public bool canMoveLeft = true;
        public bool canMoveRight = true;

        public bool climbing = false;
        
        public bool ridingElevator = false;

        public bool pushing = false;
    }

    public Animator anim;
    public pState myState;
    public float walkSpeed = 2;
    public float fallSpeed = 1;
    public float climbSpeed = 0.5f;

    public GameObject target = null;
    public Ladder ladderToMove = null;
    public GameObject currentCenterLine = null;

    public float scentFrequency = 5f;
    public Scent currentScent;


    void Awake()
    {
        myState = new pState();
        anim = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {
        //InvokeRepeating("Stink", scentFrequency,scentFrequency);
	}

    void Stink()
    {
        if (currentScent)
            currentScent.Smell();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(myState.logsTouching);
        if (myState.falling == false)
        {
            if (myState.climbing == false)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    myState.pushing = true;
                else
                {
                    myState.pushing = false;
                }

                if (Input.GetKey(KeyCode.LeftArrow) && myState.canMoveLeft == true)
                {
                    anim.SetBool("WalkingLeft", true);
                    transform.position += Vector3.left * walkSpeed;
                    if (myState.pushing == true && ladderToMove != null)
                    {
                        if (ladderToMove.canMoveLeft)
                        ladderToMove.transform.position += Vector3.left * walkSpeed;
                    }
                }
                else
                    anim.SetBool("WalkingLeft", false);

                if (Input.GetKey(KeyCode.RightArrow) && myState.canMoveRight == true)
                {
                    anim.SetBool("WalkingRight", true);
                    transform.position += Vector3.right * walkSpeed;
                    if (myState.pushing == true && ladderToMove != null)
                    {
                        if(ladderToMove.canMoveRight)
                        ladderToMove.transform.position += Vector3.right * walkSpeed;
                    }
                }
                else
                    anim.SetBool("WalkingRight", false);

                if (target != null)
                {
                    if (myState.canClimbUp == true)
                    {
                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            myState.climbing = true;
                            anim.SetBool("Climbing", true);
                            transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
                        }

                    }
                    if (myState.canClimbDown == true)
                    {
                        if (Input.GetKey(KeyCode.DownArrow))
                        {
                            myState.climbing = true;
                            anim.SetBool("Climbing", true);
                            transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
                        }
                    }
                }
            }
            else if (myState.climbing == true)
            {
                if (myState.canClimbUp)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        transform.position += Vector3.up * climbSpeed;
                    }
                }

                if (myState.canClimbDown)
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
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
        Debug.Log("OnTriggerEnter: " + col.gameObject.tag);

        if (col.gameObject.CompareTag("Log"))
        {
            myState.logsTouching++;
            if (myState.climbing == false)
            {
                if (myState.initialLoop)
                    myState.initialLoop = false;
                else
                {
                    if (myState.falling == true)
                    {
                        myState.falling = false;
                        transform.position = new Vector3(transform.position.x, currentCenterLine.transform.position.y, transform.position.z);
                    }
                }
            }
        }

        if (col.gameObject.CompareTag("StopClimbUp"))
        {
            myState.canClimbUp = false;
            myState.canClimbDown = true;
            if (myState.climbing == true)
            {
                myState.climbing = false;
                anim.SetBool("Climbing", false);
                transform.position = new Vector3(transform.position.x, currentCenterLine.transform.position.y, transform.position.z);
            }
        }

        if (col.gameObject.CompareTag("StopClimbDown"))
        {
            myState.canClimbUp = true;
            myState.canClimbDown = false;
            if (myState.climbing == true)
            {
                myState.climbing = false;
                anim.SetBool("Climbing", false);
                transform.position = new Vector3(transform.position.x, currentCenterLine.transform.position.y, transform.position.z);
            }
        }


        if(col.gameObject.CompareTag("Ladder"))
            target = col.gameObject;

        if (col.gameObject.CompareTag("LadderCanMove"))
            ladderToMove = col.gameObject.GetComponent<LadderCanMove>().myLadder;

        if (col.gameObject.CompareTag("ScreenBoundLeft"))
            myState.canMoveLeft = false;
        if (col.gameObject.CompareTag("ScreenBoundRight"))
            myState.canMoveRight = false;

        if (col.gameObject.CompareTag("CenterLine"))
            currentCenterLine = col.gameObject;

        if (col.gameObject.CompareTag("Elevator"))
        {
            if(col.gameObject.GetComponent<Elevator>().atBottom == false)
            {
                myState.ridingElevator = true;
            }
        }

        if (col.gameObject.CompareTag("Scent"))
        {
            col.gameObject.GetComponent<Scent>().Smell();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("OnTriggerExit: " + col.gameObject.tag);

        if (col.gameObject.CompareTag("Log"))
        {
            if (myState.ridingElevator == false)
            {
                myState.logsTouching--;
                if (myState.climbing == false && myState.logsTouching == 0)
                {
                    myState.falling = true;
                }
            }
        }

        if (myState.climbing == true)
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
            target = null;
        }

        if (col.gameObject.CompareTag("LadderCanMove"))
            ladderToMove = null;

        if (col.gameObject.CompareTag("ScreenBoundLeft"))
            myState.canMoveLeft = true;
        if (col.gameObject.CompareTag("ScreenBoundRight"))
            myState.canMoveRight = true;

        if (col.gameObject.CompareTag("Scent"))
        {
            currentScent = null;
        }
    }
   
}
