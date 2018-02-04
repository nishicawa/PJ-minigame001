using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject gameManager;

    public LayerMask blockLayer;

    private Rigidbody2D rbody;
    private Animator anim;

    private const float MOVE_SPEED = 8;
    private float moveSpeed;
    private float jumpPower = 300;
    private bool goJump = false;
    private bool canJump = false;
    private bool usingButtons = false;

    public enum MOVE_DIR
    {
        STOP,
        LEFT,
        RIGHT,
    }

    private MOVE_DIR moveDirection = MOVE_DIR.STOP;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        canJump =
            Physics2D.Linecast(transform.position - (transform.right * 0.3f), transform.position - (transform.up * 0.1f), blockLayer) ||
            Physics2D.Linecast(transform.position + (transform.right * 0.3f), transform.position - (transform.up * 0.1f), blockLayer);

        if (!usingButtons)
        {
            float x = Input.GetAxisRaw("Horizontal");

            if (x == 0)
            {
                moveDirection = MOVE_DIR.STOP;
            }
            else
            {
                if (x < 0)
                {
                    moveDirection = MOVE_DIR.LEFT;
                }
                else
                {
                    moveDirection = MOVE_DIR.RIGHT;
                }
            }
            if (Input.GetButton("Jump"))
            {
                PushJumpButton();
            }
        }
        //Animatorへパラメーターを送る
        anim.SetFloat("velY", rbody.velocity.y);
        anim.SetBool("isGrounded", canJump);
    }


    void FixedUpdate()
    {
        // move
        switch (moveDirection)
        {
            case MOVE_DIR.STOP:
                moveSpeed = 0;
                break;
            case MOVE_DIR.LEFT:
                moveSpeed = MOVE_SPEED * -1;
                transform.localScale = new Vector2(-1, 1);
                break;
            case MOVE_DIR.RIGHT:
                moveSpeed = MOVE_SPEED;
                transform.localScale = new Vector2(1, 1);
                break;
        }
        rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);

        // jump
        if (goJump)
        {
            rbody.AddForce(Vector2.up * jumpPower);
            goJump = false;
        }
    }

    public void PushLeftButton()
    {
        moveDirection = MOVE_DIR.LEFT;
        usingButtons = true;
    }

    public void PushRightButton()
    {
        moveDirection = MOVE_DIR.RIGHT;
        usingButtons = true;
    }

    public void ReleaseMoveButton()
    {
        moveDirection = MOVE_DIR.STOP;
        usingButtons = false;
    }

    public void PushJumpButton()
    {
        if (canJump)
        {
            goJump = true;
            anim.SetTrigger("Jump");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Ouch!!", gameObject);
        if (col.gameObject.tag == "Trap")
        {
            Debug.Log("OMG!!", gameObject);
            DestroyPlayer();
        }

        if (col.gameObject.tag == "Goal")
        {
            Debug.Log("Goal!!", gameObject);
        }
    }

    void DestroyPlayer()
    {
        Destroy(this.gameObject);
    }
}
