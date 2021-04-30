using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageDoor : MonoBehaviour {

    public Animator animator;

    private GameObject cage;

    private float openStartTime;
    private bool opened = false;
    private bool startedOpening = false;

    void Awake() {
        cage = GameObject.Find("Cage");
    }

    void Update() {
        if (!opened && startedOpening && Time.time >= openStartTime + 1f) {
            cage.transform.Find("CageDoorOpen").gameObject.SetActive(true);
            cage.transform.Find("CageDoor").gameObject.SetActive(false);
            cage.transform.Find("CageFront").GetComponent<Collider2D>().enabled = false;
            animator.enabled = false;
            opened = true;
            GameObject.Find("Warrior1").AddComponent<MovableObject>();
        }
    }

    private void Open() {
        openStartTime = Time.time;
        startedOpening = true;
        animator.Play("OpenDoor");
        GameObject.Find("HelpText").SetActive(false); // Hides the help text
    }
}
