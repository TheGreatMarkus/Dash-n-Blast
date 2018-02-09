using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWallChecker : MonoBehaviour {

    private PlayerController pc;
    // Use this for initialization
    void Start() {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Right wall detector collided with " + other.name);
        pc.setWallRight(true);

    }

    private void OnTriggerExit2D(Collider2D other) {
        pc.setWallRight(false);
    }
}
