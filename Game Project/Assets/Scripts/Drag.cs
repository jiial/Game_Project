using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    //public int dragForce = 50;    -Currently not needed
    public float MAX_VELOCITY = 100;

    private Vector2 lastMousePos;
    private bool hold = false;
    private Transform draggingObject;
    

    void Update() {
        Vector3 temp = Input.mousePosition;
        Vector2 mousePos = new Vector2(temp.x, temp.y);
        if (Input.GetKey(KeyCode.Mouse0)) {
            hold = true;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            transform.position = pos;
            if (draggingObject != null) {
                Vector3 velocity = (pos - draggingObject.position) * 5;
                if (velocity.x > MAX_VELOCITY) {
                    velocity.x = MAX_VELOCITY;
                } else if (velocity.x < -MAX_VELOCITY) {
                    velocity.x = -MAX_VELOCITY;
                } else if (velocity.y > MAX_VELOCITY) {
                    velocity.y = MAX_VELOCITY;
                } else if (velocity.y < -MAX_VELOCITY) {
                    velocity.y = -MAX_VELOCITY;
                }
                draggingObject.GetComponent<Rigidbody2D>().velocity = velocity;
                // draggingObject.position = pos;   -Removed as dragging this way feels unnatural
                // draggingObject.GetComponent<Rigidbody2D>().AddForceAtPosition((mousePos - lastMousePos) * dragForce, new Vector2(pos.x, pos.y), ForceMode2D.Force);  -Removed as the additional force acted weirdly
            }
        } else {
            hold = false;
            Destroy(GetComponent<FixedJoint2D>());
            draggingObject = null;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        lastMousePos = mousePos;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (hold && draggingObject == null && col.gameObject.GetComponent<MovableObject>() != null) {
            draggingObject = col.transform;
            Rigidbody2D rb = draggingObject.GetComponent<Rigidbody2D>();
            if (rb != null) {
                FixedJoint2D fj = transform.gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
                fj.connectedBody = rb;
            } else {
                FixedJoint2D fj = transform.gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
            }
        }
    }
}
