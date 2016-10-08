using UnityEngine;
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
    public GameObject target;
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
        if (brain.state == GumbleBrain.done)
        {
            myAnim.SetTrigger("Jump");
        } else if (brain.state == GumbleBrain.win)
        {
            myAnim.SetTrigger("Eat");
        }
        else
        {
            Look();

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

    void Hunt()
    {
        if (!jumping)
        {
            if (target.transform.position.y == transform.position.y)
            {
                if (target.transform.position.x > transform.position.x)
                {
                    MoveRight();
                    brain.hasHunted += Time.deltaTime;
                }
                else if (target.transform.position.x < transform.position.x)
                {
                    MoveLeft();
                    brain.hasHunted += Time.deltaTime;
                }
            }
            else
            {
                if (target.transform.position.y > transform.position.y)
                    StartCoroutine(JumpUp());
                else if(target.transform.position.y < transform.position.y)
                    StartCoroutine(JumpDown());
            }
        }
    }



    public void Look()
    {
        if (brain.state != GumbleBrain.hunting)
        {

            RaycastHit2D player;
            if (facingLeft)
            {
                player = Physics2D.Raycast(transform.position, Vector2.left,Mathf.Infinity,LayerMask.GetMask("Player"));
            }
            else
            {
                player = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, LayerMask.GetMask("Player"));
            }

            if (player)
            {
                Destroy(target);
                target = player.collider.gameObject;
                brain.state = GumbleBrain.hunting;
                brain.waiting = false;
                brain.hasWaited = 0;
            }
        }
    }

    public void Sniff()
    {

        Debug.Log("*Sniff, sniff*");


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
            target = Instantiate(TargetPrefab, newTarget.transform.position, Quaternion.identity) as GameObject;
        }
        else
        {
            target = Instantiate(TargetPrefab, Scent.scents[Random.Range(0, Scent.scents.Count - 1)].transform.position, Quaternion.identity) as GameObject;
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
        if (target == null)
        {
            Sniff();
        }

        if (target == thePlayer.gameObject)
        {
            brain.state = GumbleBrain.hunting;
            brain.hasHunted = 0;
        }
        else
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

            if (brain.hasRoamed >= brain.roamDistance)
            {
                brain.waiting = true;
                brain.hasRoamed = 0;
                StopWalking();
            }
        }
    }

    public void MoveRight()
    {
        facingLeft = false;
        facingRight = true;
     
            myAnim.SetBool("WalkLeft", false);
            myAnim.SetBool("WalkRight", true);
        
        transform.position += Vector3.right * speed;
    }

    public void MoveLeft()
    {
        facingLeft = true;
        facingRight = false;
            myAnim.SetBool("WalkRight", false);
            myAnim.SetBool("WalkLeft", true);
       
        transform.position += Vector3.left * speed;
    }

    IEnumerator JumpUp()
    {
            myAnim.SetTrigger("Jump");
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
        myAnim.SetTrigger("Jump");
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
        brain.waiting = true;
        brain.hasWaited = 0;
        target = null;
        StopWalking();
    }

    public void StopWalking()
    {
        if (myAnim.GetBool("WalkRight"))
        {
            myAnim.SetBool("WalkRight", false);
        }
        if (myAnim.GetBool("WalkLeft"))
        {
            myAnim.SetBool("WalkLeft", false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CenterLine") && col.gameObject.GetComponentInParent<Elevator>() == false)
        {
            brain.waiting = true;
            jumping = false;
            StopWalking();
            transform.position = new Vector3(transform.position.x, col.gameObject.transform.position.y, transform.position.z);
        }

        if (col.gameObject.CompareTag("Scent"))
        {
            if (hunting)
                Sniff();
        }
        if (col.gameObject.CompareTag("Splort"))
        {
            brain.waiting = true;
            brain.hasWaited = 0;
            brain.splortsEaten++;
            myAnim.SetTrigger("Eat");
            col.gameObject.GetComponent<Splort>().Die();
        }
        if (col.gameObject.CompareTag("Cage"))
        {
            Debug.Log("Cage collison:"+brain.splortsEaten+brain.splortsToEat);

            if (brain.splortsEaten == brain.splortsToEat)
            {
                transform.position = col.gameObject.transform.position;
                brain.state = GumbleBrain.done;
            }
            else
            {
                brain.waiting = true;
                brain.hasWaited = 0;
                col.gameObject.GetComponent<Cage>().falling = true;
            }
        }

        if (col.gameObject.CompareTag("Player"))
        {
            brain.state = GumbleBrain.win;
            transform.position = new Vector3(col.transform.position.x, col.transform.position.y + 4, col.transform.position.z);
        }

        if (col.gameObject == target && target != thePlayer.gameObject)
        {
            Destroy(col.gameObject);
            target = null;
            brain.waiting = true;
            brain.state = GumbleBrain.roaming;
            StopWalking();
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
