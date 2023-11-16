using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move1 : MonoBehaviour
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
    private string animationAttackName6 = "GhostSamurai_APose_JumpAttack02_Inplace";
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
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.LeftArrow))
        {
            float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            transform.Translate(horizontal, 0, vertical);
        }
        
        animator.SetBool("forward", Input.GetKey(KeyCode.UpArrow));
        animator.SetBool("backward", Input.GetKey(KeyCode.DownArrow));
        animator.SetBool("left", Input.GetKey(KeyCode.LeftArrow));
        animator.SetBool("right", Input.GetKey(KeyCode.RightArrow));
        animator.SetBool("jump", Input.GetKey(KeyCode.J));
        animator.SetBool("airAttack", Input.GetKey(KeyCode.K));
        animator.SetBool("beenHit", Input.GetKey(KeyCode.L));
    }

    void run()
    {

        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    currentSpeed = defaultSpeed * 3;
        //}
        //else if (speed != 0.0f)
        //{
        //    currentSpeed = defaultSpeed;
        //}
        animator.SetBool("isRun", Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunForward", Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunBackward", Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunLeft", Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunRight", Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isAttack", Input.GetKey(KeyCode.H));
    }

    void isStopMoving()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName1) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName2) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName3) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName4) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName5) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName6))

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
