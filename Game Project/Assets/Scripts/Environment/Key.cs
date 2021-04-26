using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

    [SerializeField] private Transform guard;

    private Collider2D col;
    private bool draggingStarted = false;

    void Awake() {
        col = GetComponent<Collider2D>();
        GameObject cageCorner = GameObject.Find("CageFront");
        Physics2D.IgnoreCollision(col, cageCorner.GetComponent<Collider2D>()); // Ignore collision with the corner of the cage so that you can reach the door
        GameObject guard = GameObject.Find("Warrior1");
        Physics2D.IgnoreCollision(col, guard.GetComponent<Collider2D>()); // Also ignore collision with the guard holding the key
    }

    void Update() {
        if (draggingStarted) {
            transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            draggingStarted = false;
        }
    }

    public void StartDragging() {
        draggingStarted = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Equals("CageDoor")) {
            GameObject.Find("Cage").SendMessage("Open");
            Destroy(gameObject);
        }
    }
}
