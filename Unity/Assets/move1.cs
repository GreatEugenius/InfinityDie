using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move1 : MonoBehaviour
{
    // Start is called before the first frame update
    public float defaultSpeed = 4.0f;
    public float speed;
    public float currentSpeed;
    public bool isDefense = false;
    private Animator animator;

    private bool isAttacked = false;
    private float attackTimer = 0;
    private bool isDefenseHit = false;
    private float defenseHitTimer = 0;

    private GameObject whiteObject;
    private Animator whiteAnimator;
    private Move whiteMove;

    private string animationAttackName1 = "GhostSamurai_APose_Attack01_1_ALL_Inplace";
    private string animationAttackName2 = "GhostSamurai_APose_Attack01_2_Inplace";
    private string animationAttackName3 = "GhostSamurai_APose_Attack01_3_Inplace";
    private string animationAttackName4 = "GhostSamurai_APose_Attack01_4_Inplace";
    private string animationAttackName5 = "GhostSamurai_APose_Hit_F_Inplace";
    private string animationAttackName6 = "GhostSamurai_APose_JumpAttack02_Inplace";

    public GameObject HP_Bar;
    public Image HP_Image;
    public float HP;
    public float HP_Percent;
    public float Max_HP;
    void Start()
    {
        animator = GetComponent<Animator>();
        whiteObject = GameObject.Find("Model_Unity_Ver1");
        whiteAnimator = whiteObject.GetComponent<Animator>();
        whiteMove = whiteObject.GetComponent<Move>();

        speed = defaultSpeed;
        currentSpeed = defaultSpeed;

        HP_Bar = GameObject.Find("White_HealthBar");
        HP_Image = HP_Bar.GetComponent<Image>();
        HP = 20;
        Max_HP = HP;
        HP_Percent = HP / Max_HP;
        HP_Image.fillAmount = HP_Percent;
        Debug.Log(Max_HP);
    }

    // Update is called once per frame
    void Update()
    {
        walk();

        isDefense = defense();

        attack();

        updata_HP();
    }

    void updata_HP()
    {
        if (HP <= 0)
        {
            whiteAnimator.SetBool("isDie", true);
        }
        HP_Percent = HP / Max_HP;
        HP_Image.fillAmount = HP_Percent;
    }

    void walk()
    {
        if (Input.GetKey(KeyCode.P))
        {
            whiteAnimator.SetBool("isDie", false);
            HP = Max_HP;
        }
        isStopMoving();
        Debug.Log(speed);

        float horizontal = 0, vertical = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vertical = speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vertical = -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -speed * Time.deltaTime;
        }
        transform.Translate(horizontal, 0, vertical);

        animator.SetBool("forward", Input.GetKey(KeyCode.UpArrow));
        animator.SetBool("backward", Input.GetKey(KeyCode.DownArrow));
        animator.SetBool("left", Input.GetKey(KeyCode.LeftArrow));
        animator.SetBool("right", Input.GetKey(KeyCode.RightArrow));
        animator.SetBool("jump", Input.GetKey(KeyCode.J));
        animator.SetBool("airAttack", Input.GetKey(KeyCode.K));
        //animator.SetBool("beenHit", Input.GetKey(KeyCode.L));
    }

    bool defense()
    {
        animator.SetBool("isRun", Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunForward", Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunBackward", Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunLeft", Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isRunRight", Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.RightShift));
        animator.SetBool("isAttack", Input.GetKey(KeyCode.H));

        if (isDefenseHit == true)
        {
            defenseHitTimer += Time.deltaTime;
            if (defenseHitTimer > 0.1)
            {
                whiteAnimator.SetBool("defenseHit", false);
                defenseHitTimer = 0;
                isDefenseHit = false;
            }
        }

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
        if (animator.GetBool("isDie") || animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName1) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName2) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName3) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName4) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName5) ||
            animator.GetCurrentAnimatorStateInfo(0).IsName(animationAttackName6))

        {
            speed = 0.0f;
        }
        else if(isDefense == true)
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

        if (stateInfo.IsName(animationAttackName1) && currentAnimationTime < 0.3)
        {
            if (whiteMove.isDefense == false)
            {
                whiteAnimator.SetBool("beenHit", true);
                isAttacked = true;
                HP -= 0.7f;
            }
            else
            {
                whiteAnimator.SetBool("defenseHit", true);
                isDefenseHit = true;
                HP -= 0.3f;
            }
        }

        if (stateInfo.IsName(animationAttackName2) && currentAnimationTime < 0.3)
        {
            whiteAnimator.SetBool("beenHit", true);
            isAttacked = true;
            if (whiteMove.isDefense == false)
            {
                HP -= 1.3f;
            }
            else
            {
                HP -= 1f;
            }
        }

        if (stateInfo.IsName(animationAttackName6) && currentAnimationTime < 0.7 && currentAnimationTime > 0.3)
        {
            whiteAnimator.SetBool("beenHit", true);
            isAttacked = true;

            if (whiteMove.isDefense == false)
            {
                HP -= 2.8f;
            }
            else
            {
                HP -= 1.5f;
            }
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
            whiteAnimator.SetBool("beenHit", false);
        }
    }
}
