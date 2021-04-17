using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour {

    private enum State {
        MOVING,
        CHASING,
        KNOCKBACK,
        ATTACKING,
        DEAD
    }

    [SerializeField] private float patrollingDistance;
    [SerializeField] private float observingDistance;
    [SerializeField] private float attackDistance;

    [SerializeField] private float movementSpeed;

    private float initialX;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Player player;
    private State currentState;

    private bool facingForward = true;
    private int updatesSinceLastTurn = 0; // Used in the UpdateMovingState-method to fix a bug where the enemy keeps turning back and forth

    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        currentState = State.MOVING;
        initialX = transform.position.x;
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
        }
    }

    private void EnterMovingState() {

    }

    private void UpdateMovingState() {
        updatesSinceLastTurn++;
        if ((transform.position.x >= initialX + patrollingDistance && facingForward)
            || (transform.position.x <= initialX - patrollingDistance && !facingForward)
            && updatesSinceLastTurn > 10) {
            Turn();
            updatesSinceLastTurn = 0;
        }
        movement.Set(movementSpeed, rb.velocity.y);
        rb.velocity = movement;
    }

    private void ExitMovingState() {

    }

    private void EnterChasingState() {

    }

    private void UpdateChasingState() {

    }

    private void ExitChasingState() {

    }

    private void EnterAttackingState() {

    }

    private void UpdateAttackingState() {

    }

    private void ExitAttackingState() {

    }

    private void EnterKnockbackState() {

    }

    private void UpdateKnockbackState() {

    }

    private void ExitKnockbackState() {

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
        }

        currentState = state;
    }

    private void UpdateState() {
        if (IsPlayerInRadar()) {
            currentState = State.CHASING;
        }
        if (IsPlayerWithinAttackDistance()) {
            Attack();
        }
    }

    private void Turn() {
        transform.Rotate(0.0f, 180.0f, 0.0f);
        movementSpeed = -movementSpeed;
        facingForward = !facingForward;
    }

    private void Attack() {
        currentState = State.ATTACKING;
        // ...
    }

    private bool IsPlayerInRadar() {
        return Vector2.Distance(transform.position, player.transform.position) <= observingDistance;
    }

    private bool IsPlayerWithinAttackDistance() {
        return Vector2.Distance(transform.position, player.transform.position) <= attackDistance;
    }
}
