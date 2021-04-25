using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    private Collider2D col;

    void Awake() {
        col = GetComponent<Collider2D>();
        GameObject cageCorner = GameObject.Find("CageFront");
        Physics2D.IgnoreCollision(col, cageCorner.GetComponent<Collider2D>()); // Ignore collision with the corner of the cage so that you can reach the door
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Equals("CageDoor")) {
            GameObject.Find("Cage").SendMessage("Open");
        }
    }
}
