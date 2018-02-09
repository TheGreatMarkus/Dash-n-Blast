using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour {

    private PlayerController pc;
    // Use this for initialization
    void Start () {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Ground detector collided with " + other.name);
        pc.setIsGrounded(true);
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        pc.setIsGrounded(false);
    }
}
