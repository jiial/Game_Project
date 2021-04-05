using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private float speed;

    private Rigidbody2D body;
    private Transform arm;
    private bool grounded;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        arm = transform.Find("Arm");
    }

    void Update() {
        // Move the character based on input
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // Flip the character sprite
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector2.one;
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector2(-1, 1);
        }

        // Check if we need to jump
        if (Input.GetKey(KeyCode.Space) && grounded) {
            Jump();
        }

        // Update arm position to follow the mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.Find("Hand").position;
        difference.Normalize();
        float zRotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        arm.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    void OnCollisionEnter2D() {
        grounded = true;
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, speed);
        grounded = false;
    }
}
