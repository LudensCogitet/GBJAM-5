using UnityEngine;
using System.Collections;

public class Gumble : MonoBehaviour {

    public class GumbleBrain
    {
        public static int roaming = 0;
        public static int hunting = 1;
        public static int eating = 2;
        public static int waiting = 3;

        public float waitTime = 5f;
        public float hasWaited = 0f;
        public int state = GumbleBrain.waiting;
    };

    public GumbleBrain brain;
    public GameObject TargetPrefab;
    public GameObject target;

    public bool movingLeft;
    public bool movingRight;

    public bool canMoveLeft = true;
    public bool canMoveRight = true;

    public bool jumpingUp;
    public bool jumpingDown;

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
	void Start () {
        brain = new GumbleBrain();
        myBox = GetComponent<BoxCollider2D>();
        myAnim = GetComponent<Animator>();

        
    }
	
	// Update is called once per frame
	void Update () {

        if (brain.state == GumbleBrain.waiting)
        {
            if (brain.hasWaited >= brain.waitTime)
            {
                brain.state = GumbleBrain.roaming;
                brain.hasWaited = 0f;
            }
            else
            {
                brain.hasWaited += Time.deltaTime;
            }
        }
        else if(brain.state == GumbleBrain.roaming)
        {
            Sniff();
        }


        if (movingLeft)
        {
            if(canMoveLeft)
            transform.position += Vector3.left * speed; 
        }
        else if (movingRight)
        {
            if(canMoveRight)
            transform.position += Vector3.right * speed;
        }
        else if (jumpingUp)
        {
            transform.position += Vector3.up * jumpSpeed;
        }
        else if (jumpingDown)
        {
            transform.position += Vector3.down * jumpSpeed;
        }
	}

    public void Idle()
    {
        float rand = Random.value;
        if (movingLeft == true || movingRight == true)
        {
            StopMoving();
        }
        else
        {
            if (rand < 0.33f)
            {
                MoveLeft();
            }
            else if (rand > 0.33f)
            {
                MoveRight();
            }
            else
            {
                StopMoving();
            }
        }
        Invoke("Idle", Random.Range(minMoveTime, maxMoveTime));
    }

    public void Sniff()
    {

        Debug.Log("*Sniff, sniff*");
        Collider2D[] odors = Physics2D.OverlapCircleAll(transform.position, noseStrength,LayerMask.GetMask("Scent"));
  
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

        }
    }

    /*public void Hunt()
    {
        if (hunting == true)
        {
            if (target.x >= transform.position.x - (myBox.bounds.extents.x - 1) || target.x <= transform.position.x + (myBox.bounds.extents.x - 1))
            {
                if (target.y > transform.position.y)
                {
                    StopMoving();
                    jumpingUp = true;
                    myAnim.SetTrigger("Jump");
                }
                else if (target.y > transform.position.y)
                {
                    StopMoving();
                    jumpingDown = true;
                    myAnim.SetTrigger("Jump");
                }
            }
        }
        else
        {
            hunting = true;
            if (target.x < transform.position.x - (myBox.bounds.extents.x - 1))
            {
                if (!movingLeft)
                    MoveLeft();
            }
            else if (target.x > transform.position.x + (myBox.bounds.extents.x - 1))
            {
                if (!movingRight)
                    MoveRight();
            }
            else
            {
                if (target.y > transform.position.y)
                {
                    StopMoving();
                    jumpingUp = true;
                    myAnim.SetTrigger("Jump");
                    Debug.Log("jumping");
                }
                else if (target.y < transform.position.y)
                {
                    StopMoving();
                    jumpingDown = true;
                    myAnim.SetTrigger("Jump");
                    Debug.Log("jumping");
                }
            }
        }
    }*/

    public void MoveRight()
    {
        if (canMoveRight)
        {
            movingLeft = false;
            movingRight = true;
            myAnim.SetBool("WalkLeft", false);
            myAnim.SetBool("WalkRight", true);
        }
        else
            StopMoving();
    }

    public void MoveLeft()
    {
        if (canMoveLeft)
        {
            movingLeft = true;
            movingRight = false;
            myAnim.SetBool("WalkRight", false);
            myAnim.SetBool("WalkLeft", true);
        }
        else
        {
            StopMoving();
        }
    }

    public void StopMoving()
    {
        if (movingRight)
        {
            movingRight = false;
            myAnim.SetBool("WalkRight", false);
        }

        if (movingLeft)
        {
            movingLeft = false;
            myAnim.SetBool("WalkLeft", false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CenterLine") && col.gameObject.GetComponentInParent<Elevator>() == false)
        {
            jumpingUp = false;
            jumpingDown = false;
            transform.position = new Vector3(transform.position.x, col.gameObject.transform.position.y, transform.position.z);
        }

        if (col.gameObject.CompareTag("ScreenBoundRight"))
        {
            canMoveRight = false;
            StopMoving();
        }

        if (col.gameObject.CompareTag("ScreenBoundLeft"))
        {
            canMoveLeft = false;
            StopMoving();
        }

        if (col.gameObject.CompareTag("StopMove") && col.gameObject.GetComponentInParent<Ladder>() == false)
        {

            if (transform.position.x > col.gameObject.transform.position.x)
            {
                canMoveLeft = false;
            }
            else
            {
                canMoveRight = false;
            }
            StopMoving();
        }

        if (col.gameObject.CompareTag("Scent"))
        {
            if (hunting)
                Sniff();
        }
        if (col.gameObject.CompareTag("Splort"))
        {
            hunting = false;
            StopMoving();
            myAnim.SetTrigger("Eat");
            col.gameObject.GetComponent<Splort>().Die();
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
