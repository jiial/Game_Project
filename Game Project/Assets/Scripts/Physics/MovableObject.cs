using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour {

    private Collider2D col;

    void Awake() {
        col = GetComponent<Collider2D>();
        GameObject player = GameObject.Find("Player");
        GameObject playerArm = player.transform.Find("Arm").gameObject;
        Physics2D.IgnoreCollision(col, player.GetComponent<Collider2D>()); // Ignore collision with player
        Physics2D.IgnoreCollision(col, playerArm.GetComponent<Collider2D>()); // Ignore collision with player's arm
    }
}
