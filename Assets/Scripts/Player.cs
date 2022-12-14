using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxWalkSpeed;
    
    private Vector3 playerVelocity;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        playerVelocity = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        // Shift the x position of the player according to the left/right or A/D key presses
        playerVelocity.x = horizontal * maxWalkSpeed;
        rb.velocity = playerVelocity;

        // Animate the player
        if (horizontal != 0f)
        {
            Vector3 theScale = transform.localScale;
            if (horizontal > 0f && theScale.x != 1f)
            {
                theScale.x = 1f; // Normal
                transform.localScale = theScale;
            }
            else if (horizontal < 0f && theScale.x != -1f)
            {
                theScale.x = -1f; // Flipped
                transform.localScale = theScale;
            }
            animator.Play("penguin_walk");
        }
        else
        {
            animator.Play("penguin_idle");
        }
    }
}
