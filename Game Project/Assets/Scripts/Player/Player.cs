using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum CombatStyle {
        TELEKINESIS,
        MELEE
    }

    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;

    private Rigidbody2D body;
    private PlayerArm arm;
    private bool grounded;

    private bool drawingSword = false;
    private bool sheatingSword = false;
    private float updatesSinceSwitching = 0;
    private float combatSwitchCooldown = 1f;

    private Animator animator;

    public bool facingForward = true;
    public CombatStyle currentStyle = CombatStyle.TELEKINESIS;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        arm = GameObject.Find("Arm").GetComponent<PlayerArm>();
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

        // Check if we need to switch the combat style
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !sheatingSword && !drawingSword) {
            SwitchCombatStyle();
        }

        // Check if we need to attack
        if (Input.GetMouseButtonDown(0) && currentStyle.Equals(CombatStyle.MELEE) && !sheatingSword) {
            Attack();
        }

            // Finish switching to telekinesis with cooldown
            if (sheatingSword) {
            updatesSinceSwitching += Time.deltaTime;
            if (updatesSinceSwitching >= combatSwitchCooldown) {
                currentStyle = CombatStyle.TELEKINESIS;
                GameObject.Find("DragPoint").GetComponent<Drag>().enabled = true;
                updatesSinceSwitching = 0;
                sheatingSword = false;
            }
        }

        // Finish switching to melee with cooldown
        if (drawingSword) {
            updatesSinceSwitching += Time.deltaTime;
            if (updatesSinceSwitching >= combatSwitchCooldown) {
                updatesSinceSwitching = 0;
                drawingSword = false;
            }
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

    private void Attack() {
        animator.Play("Attack");
    }

    private void SwitchCombatStyle() {
        if (currentStyle.Equals(CombatStyle.TELEKINESIS)) {
            currentStyle = CombatStyle.MELEE;
            arm.ResetPosition();
            GameObject.Find("DragPoint").GetComponent<Drag>().enabled = false;
            animator.Play("DrawSword");
            drawingSword = true;
        } else if (currentStyle.Equals(CombatStyle.MELEE)) {
            arm.ResetPosition();
            animator.Play("SheatheSword");
            sheatingSword = true;
        }
    }
}
