using UnityEngine;
using System.Collections;

public class Splort : MonoBehaviour {

    public bool movingLeft;
    public bool canMoveLeft = true;
    public bool canMoveRight = true;

    public float minChangeLength = 2f;
    public float maxChangeLength = 10f;
    public float deathTime;

    public float speed = 0.5f;
    float accumulator = 0;

    public Animator myAnim;

	// Use this for initialization
	void Start () {
        myAnim = GetComponent<Animator>();
        Invoke("ChooseDir", Random.Range(minChangeLength, maxChangeLength));
    }
	
	// Update is called once per frame
	void Update () {
        accumulator += speed;

        int temp = Mathf.RoundToInt(accumulator);
        if (temp > 0)
        {
            
            if (movingLeft)
                transform.position += Vector3.left * temp;
            else
                transform.position += Vector3.right * temp;

            accumulator = 0;
        }

	}

    void ChooseDir()
    {
        if (canMoveLeft == false)
        {
            movingLeft = false;
            myAnim.SetBool("MoveLeft", false);
        }
        else if (canMoveRight == false)
        {
            movingLeft = true;
            myAnim.SetBool("MoveLeft", true);
        }
        else
        {
            if (Random.value < 0.5f)
            {
                movingLeft = true;
                myAnim.SetBool("MoveLeft", true);
            }
            else
            {
                movingLeft = false;
                myAnim.SetBool("MoveLeft", false);
            }
        }
        Invoke("ChooseDir", Random.Range(minChangeLength, maxChangeLength));
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("StopMove"))
        {
            if (!col.gameObject.GetComponentInParent<Ladder>())
            {
                if(movingLeft == true)
                {
                    canMoveLeft = false;
                    canMoveRight = true;
                }
                else
                {
                    canMoveLeft = true;
                    canMoveRight = false;
                }
                CancelInvoke("ChooseDir");
                ChooseDir();
            }
        }
        /*if (col.gameObject.CompareTag("Gumble"))
        {
            Invoke("Die", deathTime);
        }*/
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("StopMove"))
        {
            if (!col.gameObject.GetComponentInParent<Ladder>())
            {
                canMoveLeft = true;
                canMoveRight = true;
            }
        }
    }
}
