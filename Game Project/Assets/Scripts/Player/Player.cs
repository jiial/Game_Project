using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum CombatStyle {
        TELEKINESIS,
        MELEE
    }

    [SerializeField] private Transform attack1HitBoxPos;
    [SerializeField] private float attack1Damage;
    [SerializeField] private float attack1Radius;
    [SerializeField] private float attack1Duration;
    [SerializeField] private LayerMask whatIsDamageable;

    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float maxHealth;

    private Rigidbody2D body;
    private PlayerArm arm;
    private bool grounded;
    private HealthBar healthbar;
    private ParticleSystemScript particles;

    private float currentHealth;
    private bool drawingSword = false;
    private bool sheatingSword = false;
    private float updatesSinceSwitching = 0;
    private float combatSwitchCooldown = 1.25f;
    private float[] attackDetails;
    private float attackStartTime;
    private float dyingStartTime;
    private bool dying;

    private Animator animator;

    public bool facingForward = true;
    public CombatStyle currentStyle = CombatStyle.TELEKINESIS;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        arm = GameObject.Find("Arm").GetComponent<PlayerArm>();
        animator = GetComponent<Animator>();
        attackDetails = new float[2];
        currentHealth = maxHealth;
        healthbar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        healthbar.SetMaxHealth(maxHealth);
        particles = GetComponent<ParticleSystemScript>();
        GameObject cageDoor = GameObject.Find("Cage").transform.Find("CageDoor").gameObject;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), cageDoor.GetComponent<Collider2D>()); // Ignore collision with the door as the cage collision is enough
    }

    void Update() {
        if (currentHealth <= 0) {
            // Die
            dying = true;

            if (dying && Time.time >= dyingStartTime + 1f) {
                // Game over, open menu
                GameObject.Find("Canvas").GetComponent<GameOverMenu>().EnterMenu();
                currentHealth = maxHealth;
            } else {
                return;
            }
        }
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
            if (Time.time >= attackStartTime + attack1Duration) {
                Attack();
            }
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

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            grounded = true;
        }
        animator.SetBool("isJumping", false);
    }

    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, verticalSpeed);
        grounded = false;
        animator.SetBool("isJumping", true);
    }

    private void Attack() {
        attackStartTime = Time.time;
        animator.Play("Attack");

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;

        foreach (Collider2D collider in detectedObjects) {
            attackDetails[1] = transform.position.x > collider.transform.position.x ? -1 : 1;
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void SwitchCombatStyle() {
        if (currentStyle.Equals(CombatStyle.TELEKINESIS)) {
            currentStyle = CombatStyle.MELEE;
            arm.ResetPosition();
            arm.GetComponent<Collider2D>().enabled = false;
            GameObject.Find("DragPoint").GetComponent<Drag>().enabled = false;
            animator.Play("DrawSword");
            drawingSword = true;
        } else if (currentStyle.Equals(CombatStyle.MELEE)) {
            arm.ResetPosition();
            arm.GetComponent<Collider2D>().enabled = true;
            animator.Play("SheatheSword");
            sheatingSword = true;
        }
    }

    private void Damage(float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            dyingStartTime = Time.time;
        }
        Debug.Log("Player hit, current health: " + currentHealth);
        healthbar.SetHealth(currentHealth >= 0 ? currentHealth : 0);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y + 3); // Small offset looks better
        particles.PlayAtPosition(pos);
    }
}
