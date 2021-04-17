﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    [SerializeField] private float dragForce = 10;
    [SerializeField] private float maxVelocity = 100;
    [SerializeField] private float dragSpeedMultiplier = 2;

    private Vector2 lastMousePos;
    private Transform draggingObject;

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