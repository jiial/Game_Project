using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    [SerializeField] private float dragForce = 10;
    [SerializeField] private float dragSpeedMultiplier = 2;

    private Vector2 lastMousePos;
    private Transform draggingObject;
    private FixedJoint2D fj;

    public bool hold = false;


    void Update() {
        Vector3 temp = Input.mousePosition;
        Vector2 mousePos = new Vector2(temp.x, temp.y);
        if (Input.GetKey(KeyCode.Mouse0)) {
            hold = true;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            transform.position = pos;
            if (draggingObject != null) {
                Vector3 velocity = (pos - draggingObject.position) * dragSpeedMultiplier;
                draggingObject.GetComponent<Rigidbody2D>().velocity = velocity;
                draggingObject.GetComponent<Rigidbody2D>().AddForceAtPosition((mousePos - lastMousePos) * dragForce, new Vector2(pos.x, pos.y), ForceMode2D.Force);
            }
        } else if (hold) {
            hold = false;
            if (draggingObject != null) {
                Destroy(fj);
                if (draggingObject.gameObject.layer.Equals(8)) { // Check if the target is an enemy
                    draggingObject.SendMessage("SetBeingDragged", false);
                    Debug.Log("Message sent to enemy!");
                }
                Debug.Log("Stopped dragging: " + draggingObject.name);
                draggingObject = null;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        lastMousePos = mousePos;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (hold && draggingObject == null 
            && col.gameObject.GetComponent<MovableObject>() != null) {
            draggingObject = col.transform;
            if (col.gameObject.layer.Equals(8)) { // Check if the target is an enemy
                draggingObject.SendMessage("SetBeingDragged", true);
            } else if (col.gameObject.name.Equals("CageKey")) {
                draggingObject.SendMessage("StartDragging");
            }
            Rigidbody2D rb = draggingObject.GetComponent<Rigidbody2D>();
            if (rb != null) {
                fj = transform.gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
                fj.connectedBody = rb;
                Debug.Log("Started dragging: " + col.gameObject.name);
            }
        }
    }
}
