using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour {

    public GameObject player;
    public Player playerScript;

    private Vector3 offset;

    void Awake() {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        offset = transform.position - player.transform.position;
    }

    void Update() {
        if (PauseMenu.isPaused || GameOverMenu.isOver) {
            return;
        }
        // Update arm position according to the position of the player
        if (playerScript.facingForward) {
            transform.position = player.transform.position + offset;
        } else {
            transform.position = player.transform.position + new Vector3(-offset.x, offset.y, offset.z);
        }

        // Update arm rotation to follow the mouse position when using telekinesis
        if (playerScript.currentStyle.Equals(Player.CombatStyle.TELEKINESIS)) {

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, new Vector2(0, 0), 0.01f);
            bool hitPlayer = false;
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider.gameObject == transform.gameObject || hit.collider.gameObject == player || hit.collider.gameObject == GameObject.Find("Hand")) {
                    hitPlayer = true;
                    break;
                }
            }
            if (!hitPlayer) {
                Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - GameObject.Find("Hand").transform.position;
                difference.Normalize();
                float zRotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
            }

        }
    }

    public void ResetPosition() {
        if (playerScript.facingForward) {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        } else {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }
}
