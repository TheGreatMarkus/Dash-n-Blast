using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpPower;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (Input.GetKey(KeyCode.A)) {
            rb.AddForce(Vector2.left * speed);
        }
        if (Input.GetKey(KeyCode.D)) {
            rb.AddForce(Vector2.right * speed);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        
        }
    }
}
