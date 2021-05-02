using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorEnemyBehavior : MonoBehaviour {
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

    public Transform body;

    private Vector2 initialPos;

    private Vector2 movement;
    private Rigidbody2D rb;
    private GameObject player;
    private State currentState;
    private Animator animator;
    private ParticleSystemScript particles;
    private BoxCollider2D boxCollider;

    private float currentHealth;
    private float knockbackStartTime;
    private float attackStartTime;
    private float attackEndTime;
    private float dyingStartTime;
    private int damageDirection;
    private bool facingForward = true;
    private bool beingDragged = false;
    private int updatesSinceLastTurn = 0; // Used in the UpdateMovingState-method to fix a bug where the enemy keeps turning back and forth
    private float prevX;

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        currentState = State.MOVING;
        currentHealth = maxHealth;
        initialPos = transform.position;
        particles = GetComponent<ParticleSystemScript>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("CageKey").GetComponent<Collider2D>());
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
        float yVelocity;
        if (System.Math.Abs(rb.position.x - prevX) < 0.05f && rb.velocity.magnitude < 2.5f) { // If x hasn't changed, then there must be an uphill so y is increased
            yVelocity = rb.velocity.y + 1f;
        } else {
            yVelocity = 0f;
        }
        movement.Set(movementSpeed, yVelocity);
        rb.velocity = movement;
        if (IsPlayerInRadar()) {
            SwitchState(State.CHASING);
        }
        prevX = rb.position.x;
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
        if (IsInGround((Vector2)transform.position - boxCollider.offset)) {
            transform.position = new Vector2(transform.position.x, transform.position.y + 5.5f);
        }
        Vector2 targetPos = new Vector2(player.transform.position.x, player.transform.position.y + 5);
        Vector2 nextPos =
            Vector2.MoveTowards(transform.position, targetPos, chasingSpeed * Time.deltaTime);
        if (IsInGround((Vector2)transform.position - boxCollider.offset)) {
            transform.position = new Vector2(nextPos.x, nextPos.y + 3.5f);
        } else if (System.Math.Abs(nextPos.x - prevX) < 0.05f && rb.velocity.magnitude < 2.5f) { // If x hasn't changed, then there must be an uphill so y is increased
            float yVel = rb.velocity.y + 2.5f;
            float xVel;
            //nextPos.y = nextPos.y + 0.2f;
            if (facingForward) {
                xVel = rb.velocity.x + 2.5f;
            } else {
                xVel = -(rb.velocity.x + 2.5f);
            }
            rb.velocity = new Vector2(xVel, yVel);
        } else {
            transform.position = nextPos;
        }
        
        prevX = rb.position.x;
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
        animator.SetBool("Attacking", false);
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
            SwitchState(State.CHASING);
        }
    }

    private void ExitKnockbackState() {
        rb.SetRotation(0f);
        rb.velocity = Vector2.zero;
        if (IsInGround((Vector2) transform.position - boxCollider.offset)) {
            transform.position = new Vector2(transform.position.x, transform.position.y + 5.5f);
        }
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
        damageDirection = (int)attackDetails[1];

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

    private bool IsInGround(Vector2 point) {
        EdgeCollider2D[] colliders = GameObject.Find("Ground").GetComponentsInChildren<EdgeCollider2D>();
        
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].bounds.Contains(point)) {
                return true;
            }
        }

        return false;
    }

    private bool IsOnGround(Vector2 point) {
        Vector2 closest = GameObject.Find("NewGround").GetComponent<Collider2D>().ClosestPoint(point);
        return 0.1f >= point.y - closest.y && point.y - closest.y >= 0f;
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
                if (hittingRb.velocity.magnitude > 0.1f) {
                    float[] attackDetails = new float[2];
                    attackDetails[0] = hittingRb.velocity.magnitude * dragTargetDamageMultiplier * rb.mass;
                    attackDetails[1] = hittingRb.velocity.x > transform.position.x ? -1 : 1;
                    Damage(attackDetails);
                }
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
