using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public float defaultSpeed = 4.0f;
    public float speed;
    public float currentSpeed;
    public bool isDefense = false;
    private Animator animator;

    private bool isAttacked = false;
    private float attackTimer = 0;

    private GameObject blackObject;
    private Animator blackAnimator;
    private Move1 blackMove;

    private string animationAttackName1 = "GhostSamurai_APose_Attack01_1_ALL_Inplace";
    private string animationAttackName2 = "GhostSamurai_APose_Attack01_2_Inplace";
    private string animationAttackName3 = "GhostSamurai_APose_Attack01_3_Inplace";
    private string animationAttackName4 = "GhostSamurai_APose_Attack01_4_Inplace";
    private string animationAttackName5 = "GhostSamurai_APose_Hit_F_Inplace";
    private string animationAttackName6 = "GhostSamurai_APose_JumpAttack02_Inplace";
    void Start()
    {
        animator = GetComponent<Animator>();
        blackObject = GameObject.Find("Model_Unity_Ver1 (1)");
        blackAnimator = blackObject.GetComponent<Animator>();
        blackMove = blackObject.GetComponent<Move1>();

        speed = defaultSpeed;
        currentSpeed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        walk();

        isDefense = defense();

        attack();
    }

    void walk()
    {
        isStopMoving();
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
        animator.SetBool("isAttack", Input.GetKey(KeyCode.T));
        //animator.SetBool("beenHit", Input.GetKey(KeyCode.B));
    }

    bool defense()
    {
        animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunForward", Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunBackward", Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunRight", Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRunLeft", Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift));

        if (animator.GetBool("isRun") || animator.GetBool("isRunForward") ||
            animator.GetBool("isRunBackward") || animator.GetBool("isRunRight") ||
            animator.GetBool("isRunLeft"))
        {
            return true;
        }
        else
        {
            return false;
        }
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
        else if (isDefense == true)
        {
            speed = 1.0f;
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

    private void OnTriggerEnter(Collider other)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float currentAnimationTime = stateInfo.normalizedTime;

        if (stateInfo.IsName(animationAttackName1) && currentAnimationTime < 0.5)
        {
            if(blackMove.isDefense == false)
            {
                blackAnimator.SetBool("beenHit", true);
                isAttacked = true;
            }
        }

        if (stateInfo.IsName(animationAttackName2) && currentAnimationTime < 0.5)
        {
            blackAnimator.SetBool("beenHit", true);
            isAttacked = true;
        }

        if (stateInfo.IsName(animationAttackName6) && currentAnimationTime < 0.5)
        {
            blackAnimator.SetBool("beenHit", true);
            isAttacked = true;
        }
    }

    void attack()
    {
        if (isAttacked == true)
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTimer > 1)
        {
            attackTimer = 0;
            isAttacked = false;
            blackAnimator.SetBool("beenHit", false);
        }
    }
}
