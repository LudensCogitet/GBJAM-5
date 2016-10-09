using UnityEngine;
using UnityEngine.SceneManagement;
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

        public bool done = false;
    }

    public Animator anim;
    public pState myState;
    public float walkSpeed = 2f;
    public float fallSpeed = 1f;
    public float climbSpeed = 0.5f;
    public float fallDistance = 0f;
    public float maxFall = 6;

    public GameObject target = null;
    public Moveable toMove = null;
    public GameObject currentCenterLine = null;

    public float scentFrequency = 5f;
    public Scent currentScent;
    public Gumble theGumble;
    public Cage theCage;


    void Awake()
    {
        myState = new pState();
        anim = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {
        theGumble = FindObjectOfType<Gumble>();
        theCage = FindObjectOfType<Cage>();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0);
        }
        if (!myState.done)
        {
            if (myState.initialLoop)
            {
                myState.initialLoop = false;
            }
            else
            {


                if (myState.climbing == false && myState.logsTouching == 0)
                {
                    myState.falling = true;
                }
            }
            //Debug.Log(myState.logsTouching);
            if (myState.falling == false)
            {
                if (myState.climbing == false)
                {
                    /*if (Input.GetKeyDown(KeyCode.LeftAlt))
                    {
                        //theGumble.StopMoving();
                        //theGumble.target = transform.position;
                        theGumble.hunting = false;
                    }*/
                    if (Input.GetKey(KeyCode.LeftControl))
                        myState.pushing = true;
                    else
                    {
                        myState.pushing = false;
                    }

                    if (Input.GetKeyDown(KeyCode.LeftAlt) && toMove != null)
                    {
                        if (toMove.gameObject == theCage.gameObject)
                        {
                            theCage.falling = true;
                        }
                    }

                    if (Input.GetKey(KeyCode.LeftArrow) && myState.canMoveLeft == true)
                    {
                        anim.SetBool("WalkingLeft", true);
                        transform.position += Vector3.left * walkSpeed;
                        if (myState.pushing == true && toMove != null)
                        {
                            if (toMove.canMoveLeft)
                                toMove.transform.position += Vector3.left * walkSpeed;
                        }
                    }
                    else
                        anim.SetBool("WalkingLeft", false);

                    if (Input.GetKey(KeyCode.RightArrow) && myState.canMoveRight == true)
                    {
                        anim.SetBool("WalkingRight", true);
                        transform.position += Vector3.right * walkSpeed;
                        if (myState.pushing == true && toMove != null)
                        {
                            if (toMove.canMoveRight)
                                toMove.transform.position += Vector3.right * walkSpeed;
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
                fallDistance += fallSpeed;
                transform.position += Vector3.down * fallSpeed;
                if (myState.stopFalling == true)
                {
                    myState.falling = false;
                    myState.stopFalling = false;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("OnTriggerEnter: " + col.gameObject.tag);

        if (col.gameObject.CompareTag("Log"))
        {
            myState.logsTouching++;

            if (myState.falling == true)
            {
                myState.falling = false;
                if (fallDistance > maxFall)
                {
                    transform.position = new Vector3(transform.position.x, currentCenterLine.transform.position.y, transform.position.z);
                    myState.done = true;
                    transform.Rotate(0f, 0f, 90f);
                    transform.position += Vector3.down * 4;
                }
                else
                    fallDistance = 0;
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

        if (col.gameObject.CompareTag("CanMove"))
            toMove = col.gameObject.GetComponent<CanMove>().myMoveable;

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
        if (col.gameObject.CompareTag("Gumble"))
        {
            myState.done = true;
            transform.Rotate(0f, 0f, 90f);
            transform.position += Vector3.down * 4;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        //Debug.Log("OnTriggerExit: " + col.gameObject.tag);

        if (col.gameObject.CompareTag("Log"))
        {
            if (myState.ridingElevator == false)
            {
                myState.logsTouching--;
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

        if (col.gameObject.CompareTag("CanMove"))
            toMove = null;

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
