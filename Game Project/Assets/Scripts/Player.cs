using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private float speed;
    private Rigidbody2D body;
    private bool grounded;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
    }

    void Update() {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // Flip the character sprite
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector2.one;
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector2(-1, 1);
        }

        if (Input.GetKey(KeyCode.Space) && grounded) {
            Jump();
        }
    }

    void OnCollisionEnter2D() {
        grounded = true;
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, speed);
        grounded = false;
    }
}
