using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour {

    [SerializeField] private float observingDistance;
    [SerializeField] private float attackDistance;
    private Player player;
    private State currentState;

    private enum State {
        MOVING,
        CHASING,
        KNOCKBACK,
        ATTACKING,
        DEAD
    }

    private void Awake() {
        player = GetComponent<Player>();
        currentState = State.MOVING;
    }

    private void Update() {
        UpdateState();
        // ...
    }

    private void UpdateState() {
        if (IsPlayerInRadar()) {
            currentState = State.CHASING;
        }
        if (IsPlayerWithinAttackDistance()) {
            Attack();
        }
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
