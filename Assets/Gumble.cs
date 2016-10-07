using UnityEngine;
using System.Collections;

public class Gumble : MonoBehaviour {

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

    public Vector3 target;

    BoxCollider2D myBox;

    Animator myAnim;

	// Use this for initialization
	void Start () {
        myBox = GetComponent<BoxCollider2D>();
        myAnim = GetComponent<Animator>();
        Idle();
        InvokeRepeating("Sniff", sniffTimer, sniffTimer);
    }
	
	// Update is called once per frame
	void Update () {

        if (hunting)
        {
            Hunt();
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

    void Idle()
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

    void Sniff()
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
            target = new Vector3(newTarget.gameObject.transform.position.x, newTarget.gameObject.transform.position.y, newTarget.transform.position.z);
            hunting = false;
            Hunt();
        }
        else
        {
            hunting = false;
            if (!IsInvoking("Idle"))
                Invoke("Idle", Random.Range(minMoveTime, maxMoveTime));
        }
    }

    void Hunt()
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
    }

    void MoveRight()
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

    void MoveLeft()
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

    void StopMoving()
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
            if (col.gameObject.GetComponent<Scent>().active)
            {
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
