using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;

    private Rigidbody2D body;
    private bool grounded;

    private Animator animator;

    public bool facingForward = true;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        animator = GetComponent<Animator>();
    }

    void Update() {
        // Move the character based on input and update the animator
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput == 0) {
            animator.SetBool("isWalking", false);
        } else {
            animator.SetBool("isWalking", true);
        }
        body.velocity = new Vector2(horizontalInput * horizontalSpeed, body.velocity.y);

        // Flip the character sprite
        if (horizontalInput > 0.01f) {
            facingForward = true; 
            transform.localScale = Vector2.one;
        } else if (horizontalInput < -0.01f) {
            facingForward = false;
            transform.localScale = new Vector2(-1, 1);
        }

        // Check if we need to jump
        if (Input.GetKey(KeyCode.Space) && grounded) {
            Jump();
        }
    }

    void OnCollisionEnter2D() {
        grounded = true;
        animator.SetBool("isJumping", false);
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, verticalSpeed);
        grounded = false;
        animator.SetBool("isJumping", true);
    }
}
