using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour {

    private PlayerController pc;
    // Use this for initialization
    void Start () {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        pc.setIsGrounded(true);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        pc.setIsGrounded(false);
    }
}
