using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour {

    private enum State {
        MOVING,
        CHASING,
        KNOCKBACK,
        ATTACKING,
        DRAGGED,
        DEAD
    }

    [SerializeField] private float patrollingDistance;
    [SerializeField] private float observingDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float baseDamage;
    [SerializeField] private float dragDamageMultiplier;
    [SerializeField] private float dragTargetDamageMultiplier; // Used when enemy gets hit by another object

    [SerializeField] private float movementSpeed;
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float dyingDuration;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCoolDown;
    [SerializeField] private Vector2 knockbackSpeed;

    private Vector2 initialPos;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Player playerScript;
    private GameObject player;
    private State currentState;
    private Animator animator;
    private ParticleSystemScript particles;
    private Transform body;

    private float currentHealth;
    private float knockbackStartTime;
    private float attackStartTime;
    private float attackEndTime;
    private float dyingStartTime;
    private int damageDirection;
    private bool facingForward = true;
    private bool beingDragged = false;
    private int updatesSinceLastTurn = 0; // Used in the UpdateMovingState-method to fix a bug where the enemy keeps turning back and forth

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerScript = GetComponent<Player>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        currentState = State.MOVING;
        currentHealth = maxHealth;
        initialPos = transform.position;
        particles = GetComponent<ParticleSystemScript>();
        body = transform.Find("Enemy_Body");
    }

    private void Update() {
        switch (currentState) {
            case State.MOVING:
                UpdateMovingState();
                break;
            case State.CHASING:
                UpdateChasingState();
                break;
            case State.ATTACKING:
                UpdateAttackingState();
                break;
            case State.KNOCKBACK:
                UpdateKnockbackState();
                break;
            case State.DEAD:
                UpdateDeadState();
                break;
        }
    }

    private void EnterMovingState() {

    }

    private void UpdateMovingState() {
        rb.SetRotation(0f);
        updatesSinceLastTurn++;
        if ((transform.position.x >= initialPos.x + patrollingDistance && facingForward)
            || (transform.position.x <= initialPos.x - patrollingDistance && !facingForward)
            && updatesSinceLastTurn > 10) {
            Turn();
            updatesSinceLastTurn = 0;
        }
        movement.Set(movementSpeed, rb.velocity.y);
        rb.velocity = movement;
        if (IsPlayerInRadar()) {
            SwitchState(State.CHASING);
        }
    }

    private void ExitMovingState() {

    }

    private void EnterChasingState() {

    }

    private void UpdateChasingState() {
        rb.SetRotation(0f);
        if ((player.transform.position.x >= transform.position.x && !facingForward)
            || (player.transform.position.x < transform.position.x && facingForward)) {
            Turn();
        }
        Vector2 targetPos = new Vector2(player.transform.position.x, player.transform.position.y + 5);
        transform.position = 
            Vector2.MoveTowards(transform.position, targetPos, chasingSpeed * Time.deltaTime);
        if (IsPlayerWithinAttackDistance() && Time.time >= attackEndTime + attackCoolDown) {
            SwitchState(State.ATTACKING);
        }
    }

    private void ExitChasingState() {

    }

    private void EnterAttackingState() {
        animator.SetBool("Attacking", true);
        attackStartTime = Time.time;
        player.SendMessage("Damage", baseDamage);
    }

    private void UpdateAttackingState() {
        if (Time.time >= attackStartTime + attackDuration) {
            animator.SetBool("Attacking", false);
            SwitchState(State.CHASING);
        }
    }

    private void ExitAttackingState() {
        attackEndTime = Time.time;
    }

    private void EnterDraggedState() {
        rb.constraints = RigidbodyConstraints2D.None;
        animator.SetBool("BeingDragged", true);
    }

    private void ExitDraggedState() {
        animator.SetBool("BeingDragged", false);
    }

    private void EnterDeadState() {
        dyingStartTime = Time.time;
        animator.Play("Die");
    }

    private void UpdateDeadState() {
        if (Time.time >= dyingStartTime + dyingDuration) {
            Destroy(gameObject);
        }
    }

    private void EnterKnockbackState() {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        rb.velocity = movement;
        animator.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState() {
        if (Time.time >= knockbackStartTime + knockbackDuration) {
            SwitchState(State.MOVING);
        }
    }

    private void ExitKnockbackState() {
        rb.SetRotation(0f);
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        animator.SetBool("Knockback", false);
    }

    private void SwitchState(State state) {
        switch (currentState) {
            case State.MOVING:
                ExitMovingState();
                break;
            case State.CHASING:
                ExitChasingState();
                break;
            case State.ATTACKING:
                ExitAttackingState();
                break;
            case State.KNOCKBACK:
                ExitKnockbackState();
                break;
            case State.DRAGGED:
                ExitDraggedState();
                break;
        }

        switch (state) {
            case State.MOVING:
                EnterMovingState();
                break;
            case State.CHASING:
                EnterChasingState();
                break;
            case State.ATTACKING:
                EnterAttackingState();
                break;
            case State.KNOCKBACK:
                EnterKnockbackState();
                break;
            case State.DRAGGED:
                EnterDraggedState();
                break;
            case State.DEAD:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void Turn() {
        transform.Rotate(0.0f, 180.0f, 0.0f);
        movementSpeed = -movementSpeed;
        facingForward = !facingForward;
    }

    private void Damage(float[] attackDetails) {
        currentHealth -= attackDetails[0];
        damageDirection = (int) attackDetails[1];

        if (currentHealth > 0.0f && !beingDragged) {
            SwitchState(State.KNOCKBACK);
        } else if (currentHealth <= 0.0f) {
            SwitchState(State.DEAD);
        }

        particles.PlayAtPosition(body.position);
    }

    private bool IsPlayerInRadar() {
        return Vector2.Distance(transform.position, player.transform.position) <= observingDistance;
    }

    private bool IsPlayerWithinAttackDistance() {
        return Vector2.Distance(transform.position, player.transform.position) <= attackDistance;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Drag>() == null) { // Ignore collision with the "DragPoint"
            if (currentState.Equals(State.DRAGGED)) {
                float[] attackDetails = new float[2];
                attackDetails[0] = rb.velocity.magnitude * dragDamageMultiplier;
                attackDetails[1] = damageDirection;
                Damage(attackDetails);
            } else if (collision.gameObject.GetComponent<MovableObject>() != null) {
                Rigidbody2D hittingRb = collision.gameObject.GetComponent<Rigidbody2D>();
                float[] attackDetails = new float[2];
                attackDetails[0] = hittingRb.velocity.magnitude * dragTargetDamageMultiplier * rb.mass;
                attackDetails[1] = hittingRb.velocity.x > transform.position.x ? -1 : 1;
                Damage(attackDetails);
            }
        }
    }

    public void SetBeingDragged(bool value) {
        if (value) {
            SwitchState(State.DRAGGED);
        }
        beingDragged = false;
    }
}
