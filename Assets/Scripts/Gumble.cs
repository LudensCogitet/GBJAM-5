﻿using UnityEngine;
using System.Collections;

public class Gumble : MonoBehaviour
{

    public class GumbleBrain
    {
        public static int roaming = 0;
        public static int hunting = 1;
        public static int eating = 2;
        public static int done = 3;
        public static int win = 4;

        public bool waiting = true;
        public float waitTime = 2f;
        public float hasWaited = 0f;
        public int roamDistance = 50;

        public int splortsEaten = 0;
        public int splortsToEat = 4;

        public float huntTime = 2f;

        public int hasRoamed = 0;

        public float hasHunted = 0;
        public int state = GumbleBrain.roaming;
    };

    public GumbleBrain brain;
    public GameObject TargetPrefab;
    public Target target;
    public Chad thePlayer;

    public bool facingLeft = true;
    public bool facingRight = false;

    public bool canMoveLeft = true;
    public bool canMoveRight = true;

    public bool jumping = false;

    public float minMoveTime;
    public float maxMoveTime;

    public float sniffTimer;

    public bool hunting = false;

    public float speed;
    public float jumpSpeed;

    public float noseStrength;

    Splort numNums = null;

    BoxCollider2D myBox;

    Animator myAnim;

    // Use this for initialization
    void Start()
    {
        brain = new GumbleBrain();
        myBox = GetComponent<BoxCollider2D>();
        myAnim = GetComponent<Animator>();
        thePlayer = FindObjectOfType<Chad>();
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(brain.state);
        if (brain.state == GumbleBrain.done)
        {
            myAnim. Play("Jump");
        } else if (brain.state == GumbleBrain.win)
        {
            if (facingLeft)
                myAnim.Play("GumbleEatLeft");
            else if (facingRight)
                myAnim.Play("GumbleEatRight");
        }
        else
        {
            if (!Look())
                Sniff();
            if (brain.waiting)
            {
                Wait();
            }
            else if (brain.state == GumbleBrain.roaming)
            {
                Roam();
            }
            else if (brain.state == GumbleBrain.hunting)
            {
                Hunt();
            }
        }
    }

    public bool Look()
    {
        if (brain.state != GumbleBrain.hunting)
        {

            RaycastHit2D player;
            if (facingLeft)
            {
                player = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity, LayerMask.GetMask("Player"));
            }
            else
            {
                player = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, LayerMask.GetMask("Player"));
            }

            if (player)
            {
                if (target)
                    Destroy(target.gameObject);

                target = player.collider.gameObject.GetComponent<Target>();
                brain.state = GumbleBrain.hunting;
                brain.waiting = false;
                brain.hasWaited = 0;
                return true;
            }
            else
                return false;
        }
        return true;
    }

    public void Sniff()
    {

        Debug.Log("*Sniff, sniff*");

        if (target == null)
        {
            if (!DetectOdors())
                ChooseRandom();
        }
        else
        {
            if (target.type == Target.TargetType.random)
            {
                DetectOdors();
            }
        }
    }

    bool DetectOdors()
    {
        Collider2D[] odors = Physics2D.OverlapCircleAll(transform.position, noseStrength, LayerMask.GetMask("Scent"));

        Scent newTarget = null;

        if (odors.Length > 0)
        {
            newTarget = odors[0].gameObject.GetComponent<Scent>();
            for (int i = 1; i < odors.Length; i++)
            {
                Scent next = odors[i].gameObject.GetComponent<Scent>();
                if (newTarget.age > next.age)
                    newTarget = next;
            }

            if (target)
                Destroy(target.gameObject);

            target = (Instantiate(TargetPrefab, newTarget.transform.position, Quaternion.identity) as GameObject).GetComponent<Target>();
            target.type = Target.TargetType.smelled;
            Debug.Log("smelled one");
            return true;
        }
        else
        {
            return false;
        }
    }

    void ChooseRandom()
    {
        if (Scent.scents != null)
        {
            target = (Instantiate(TargetPrefab, Scent.scents[Random.Range(0, Scent.scents.Count - 1)].transform.position, Quaternion.identity) as GameObject).GetComponent<Target>();
            target.type = Target.TargetType.random;
        }
    }

    void Hunt()
    {
        if (!jumping)
        {
            if (target.gameObject.transform.position.y == transform.position.y)
            {
                if (target.transform.position.x > transform.position.x)
                {
                    MoveRight();
                }
                else if (target.gameObject.transform.position.x < transform.position.x)
                {
                    MoveLeft();
                }
            }
            else
            {
                if (target.gameObject.transform.position.y > transform.position.y)
                    StartCoroutine(JumpUp());
                else if(target.gameObject.transform.position.y < transform.position.y)
                    StartCoroutine(JumpDown());
            }
        }
    }

    public void killSplort()
    {
        if(numNums != null)
        {
            numNums.Die();
        }
    }

    void Wait()
    {
        if (brain.hasWaited >= brain.waitTime)
        {
            brain.waiting = false;
            brain.hasWaited = 0f;
        }
        else
        {
            brain.hasWaited += Time.deltaTime;
        }
    }

    void Roam()
    {

            if (target.transform.position.x > transform.position.x)
            {
                MoveRight();
                brain.hasRoamed += (int)speed;
            }
            else if (target.transform.position.x < transform.position.x)
            {
                MoveLeft();
                brain.hasRoamed += (int)speed;
            }
            else if (target.transform.position.y > transform.position.y)
            {
                if(!jumping)
                StartCoroutine(JumpUp());
            }
            else if (target.transform.position.y < transform.position.y)
            {
                if(!jumping)
                StartCoroutine(JumpDown());
            }

            /*if (brain.hasRoamed >= brain.roamDistance)
            {
                brain.waiting = true;
                brain.hasRoamed = 0;
                StopWalking();
            }*/
        
    }

    public void MoveRight()
    {
        facingLeft = false;
        facingRight = true;

        myAnim.Play("GumbleWalkRight");
            //myAnim.SetBool("WalkRight", true);
        
        transform.position += Vector3.right * speed;
    }

    public void MoveLeft()
    {
        facingLeft = true;
        facingRight = false;
        myAnim.Play("GumbleWalkLeft");

        transform.position += Vector3.left * speed;
    }

    IEnumerator JumpUp()
    {
            myAnim.Play("GumbleJump");
            jumping = true;

        if (brain.state == GumbleBrain.hunting)
            StopHunting();
        while (jumping == true)
        {
            transform.position += Vector3.up * jumpSpeed;
            yield return null;
        }
    }

    IEnumerator JumpDown()
    {
        myAnim.Play("GumbleJump");
        jumping = true;
        if (brain.state == GumbleBrain.hunting)
            StopHunting();
        while (jumping == true)
        {
            transform.position += Vector3.down * jumpSpeed;
            yield return null;
        }
       
    }

    public void StopHunting()
    {
        brain.state = GumbleBrain.roaming;
        target = null;
    }

    public void StopWalking()
    {
        if (facingLeft)
            myAnim.Play("GumbleStandLeft");
        else
            myAnim.Play("GumbleStandRight");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CenterLine") && col.gameObject.GetComponentInParent<Elevator>() == false)
        {
            jumping = false;
            if (brain.state != GumbleBrain.hunting)
            {
                brain.waiting = true;
                brain.hasWaited = 0;

                if (Random.value < 0.5f)
                {
                    facingLeft = true;
                    facingRight = false;
                }
                else
                {
                    facingRight = true;
                    facingLeft = false;
                }
            }
            else
            {
                if(thePlayer.transform.position.y != transform.position.y)
                {
                    brain.waiting = true;
                    brain.hasWaited = 0;

                    myAnim.Play("GumbleJump");
                }
            }

            transform.position = new Vector3(transform.position.x, col.gameObject.transform.position.y, transform.position.z);
        }

        if (col.gameObject.CompareTag("Splort"))
        {
            StopWalking();
            brain.waiting = true;
            brain.hasWaited = 0;
            brain.splortsEaten++;
            transform.position = new Vector3(col.gameObject.transform.position.x, transform.position.y, transform.position.z);
            numNums = col.gameObject.GetComponent<Splort>();
            if (facingLeft)
                myAnim.Play("GumbleEatLeft");
            else if (facingRight)
                myAnim.Play("GumbleEatRight");
        }
        if (col.gameObject.CompareTag("Cage"))
        {
            Debug.Log("Cage collison:" + brain.splortsEaten + brain.splortsToEat);

            if (brain.splortsEaten >= brain.splortsToEat && col.gameObject.GetComponent<Cage>().falling == true)
            {
                transform.position = new Vector3(col.gameObject.transform.position.x,transform.position.y,transform.position.z);
                brain.state = GumbleBrain.done;
            }
            else
            {
                //brain.waiting = true;
                //brain.hasWaited = 0;
                col.gameObject.GetComponent<Cage>().falling = true;
            }
        }

        if (col.gameObject.CompareTag("Player"))
        {
            brain.state = GumbleBrain.win;
            transform.position = new Vector3(col.transform.position.x, col.transform.position.y + 4, col.transform.position.z);
        }

        if (target != null)
        {
            if (col.gameObject == target.gameObject && target.gameObject != thePlayer.gameObject)
            {
                Destroy(col.gameObject);
                target = null;
                brain.waiting = true;
                brain.hasWaited = 0;
                brain.state = GumbleBrain.roaming;
                StopWalking();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("ScreenBoundRight"))
        {
            canMoveRight = true;
            canMoveLeft = true;
        }

        if (col.gameObject.CompareTag("ScreenBoundLeft"))
        {
            canMoveRight = true;
            canMoveLeft = true;
        }

        if (col.gameObject.CompareTag("StopMove") && col.gameObject.GetComponentInParent<Ladder>() == false)
        {
            canMoveRight = true;
            canMoveLeft = true;
        }
    }
}
