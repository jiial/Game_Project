using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemScript : MonoBehaviour {

    [SerializeField] public ParticleSystem ps;
    new private bool enabled = false;

    private void Awake() {
        ps.gameObject.SetActive(false);
    }

    private void Update() {
        if (enabled) {
            if (!ps.isPlaying) {
                ps.Pause();
                Disable();
            }
        }
    }

    public void PlayAtPosition(Vector2 pos) {
        SetPosition(pos);
        Enable();
        ps.Play();
    }

    private void Enable() {
        ps.gameObject.SetActive(true);
        enabled = true;
    }

    private void Disable() {
        ps.gameObject.SetActive(false);
        enabled = false;
    }

    private void SetPosition(Vector2 pos) {
        ps.transform.position = pos;
    }
}
