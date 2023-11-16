using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public float defaultSpeed = 3.0f;
    public float speed;
    public float currentSpeed;
    private Animator animator;
    private string animationAttackName1 = "GhostSamurai_APose_Attack01_1_ALL_Inplace";
    private string animationAttackName2 = "GhostSamurai_APose_Attack01_2_Inplace";
    private string animationAttackName3 = "GhostSamurai_APose_Attack01_3_Inplace";
    private string animationAttackName4 = "GhostSamurai_APose_Attack01_4_Inplace";
    private string animationAttackName5 = "GhostSamurai_APose_Hit_F_Inplace";
    void Start()
    {
        animator = GetComponent<Animator>();
        speed = defaultSpeed;
        currentSpeed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        walk();

        run();
    }

    void walk()
    {
        isStopMoving();
        Debug.Log(speed);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {

            float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            transform.Translate(horizontal, 0, vertical);
        }

        animator.SetBool("forward", Input.GetKey(KeyCode.W));
        animator.SetBool("backward", Input.GetKey(KeyCode.S));
        animator.SetBool("left", Input.GetKey(KeyCode.A));
        animator.SetBool("right", Input.GetKey(KeyCode.D));
        animator.SetBool("jump", Input.GetKey(KeyCode.Space));
        animator.SetBool("airAttack", Input.GetKey(KeyCode.R));
        animator.SetBool("beenHit", Input.GetKey(KeyCode.B));
    }

    void run()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = defaultSpeed * 3;
        }
        else if (speed != 0.0f)
        {
            currentSpeed = defaultSpeed;
        }

        animator.SetBool("isRunForward", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunBackward", Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunRight", Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunLeft", Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift));
    }

    void isStopMoving()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName1) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName2) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName3) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName4) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName5))
        {
            speed = 0.0f;
        }
        else
        {
            speed = currentSpeed;
        }
    }

    bool isBeenHit()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName5))
        {
            return true;
        }
        else
        {
            return true;
        }
    }
}
