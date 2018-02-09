using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float xAcc;
    public float xSlowdown;
    public float maxXSpeed;
    public float initJumpVelocity;

    private bool inputJump;
    private bool inputRight;
    private bool inputLeft;
    private bool isGrounded;

    // Use this for initialization
    void Start() {
        inputJump = false;
        inputLeft = false;
        inputRight = false;
    }

    // Update is called once per frame
    void Update() {

        getInput();

    }

    private void getInput() {
        //Getting input from the user.
        if (Input.GetKey(KeyCode.A)) {
            inputLeft = true;
        } else {
            inputLeft = false;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputRight = true;
        } else {
            inputRight = false;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            inputJump = true;
        }
    }

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // Applying acceleration if the player gives input.
        if (inputLeft && rb.velocity.x > -maxXSpeed) {
            rb.AddForce(Vector2.left * xAcc);
        }
        if (inputRight && rb.velocity.x < maxXSpeed) {
            rb.AddForce(Vector2.right * xAcc);
        }

        //Slowing the player down if they dont give input.

        if (!inputLeft && rb.velocity.x < 0) {
            rb.AddForce(Vector2.right * xSlowdown);
        }
        if (!inputRight && rb.velocity.x > 0) {
            rb.AddForce(Vector2.left * xSlowdown);
        }

        if (!inputRight && !inputLeft && Math.Abs(rb.velocity.x) < 0.2) {
            Vector3 v = rb.velocity;
            v.x = 0f;
            rb.velocity = v;
        }


        if (inputJump) {
            if (isGrounded) {
                rb.velocity = new Vector2(rb.velocity.x, initJumpVelocity);
            }
            inputJump = false;
        }
        Debug.Log(rb.velocity);
    }

    public void setIsGrounded(bool isGrounded) {
        this.isGrounded = isGrounded;
    }
}
