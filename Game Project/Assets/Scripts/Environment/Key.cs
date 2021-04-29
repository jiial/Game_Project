using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    [SerializeField] private Transform guard;

    private Collider2D col;
    private bool draggingStarted = false;
    private bool needsToBeUpdated = true;

    void Awake() {
        col = GetComponent<Collider2D>();
        GameObject cageCorner = GameObject.Find("CageFront");
        Physics2D.IgnoreCollision(col, cageCorner.GetComponent<Collider2D>()); // Ignore collision with the corner of the cage so that you can reach the door
        Physics2D.IgnoreCollision(col, guard.GetComponent<Collider2D>()); // Also ignore collision with the guard holding the key
    }

    void Update() {
        if (draggingStarted && needsToBeUpdated) {
            transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            col.attachedRigidbody.mass = 10000; // Makes the key feel heavier to move
            col.attachedRigidbody.gravityScale = 20;
            needsToBeUpdated = false;
        }
    }

    public void StartDragging() {
        draggingStarted = true;
        transform.SetParent(null);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Equals("CageDoor")) {
            GameObject.Find("Cage").SendMessage("Open");
            Destroy(gameObject);
        }
    }
}
