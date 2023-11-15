using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5.0f;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        walk();
    }

    void walk()
    {
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(horizontal, 0, vertical);

        animator.SetBool("forward", Input.GetKey(KeyCode.W));
        animator.SetBool("backward", Input.GetKey(KeyCode.S));
        animator.SetBool("left", Input.GetKey(KeyCode.A));
        animator.SetBool("right", Input.GetKey(KeyCode.D));
    }
}
