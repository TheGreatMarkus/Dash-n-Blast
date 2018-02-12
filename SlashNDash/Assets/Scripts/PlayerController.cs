using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float xAcc;
    public float xSlowdown;
    public float maxXSpeed;
    public float initJumpVelocity;
    public float initWallJumpSpeed;
    public float dashSpeed;
    public float totalDashTime;

    private bool inputJump;
    private bool inputRight;
    private bool inputLeft;
    private bool inputDash;

    private bool isGrounded;
    private bool wallLeft;
    private bool wallRight;
    private bool dashing;

    private float dashTime;

    Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        inputJump = false;
        inputRight = false;
        inputLeft = false;
        inputDash = false;

        isGrounded = false;
        wallLeft = false;
        wallRight = false;

        dashTime = float.MaxValue;
        rb = GetComponent<Rigidbody2D>();

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
        if (Input.GetKeyDown(KeyCode.Space)) {
            inputDash = true;
        }
    }

    private void FixedUpdate() {

        handleDashing();
        if(dashTime> totalDashTime) {// Not dashing.
            handleHorizontalMouvement();
            handleJumping();
        }
        
    }

    private void handleDashing() {
        if (inputDash) {
            dashTime = 0;
            inputDash = false;
        }
        if (dashTime < totalDashTime) {
            rb.velocity = new Vector2(1, 0.2f) * dashSpeed * (float)Math.Pow(Math.E, -dashTime);
            dashTime += Time.deltaTime;

        }
    }

    private void handleJumping() {
        if (inputJump) {
            if (isGrounded) {
                rb.velocity = new Vector2(rb.velocity.x, initJumpVelocity);
                Debug.Log("Jumping");
            }
            if (wallLeft && !isGrounded) {
                rb.velocity = new Vector2(1, 1) * initWallJumpSpeed;
                Debug.Log("Wall Jumping");
            }
            if (wallRight && !isGrounded) {
                rb.velocity = new Vector2(-1, 1) * initWallJumpSpeed;
                Debug.Log("Wall Jumping");
            }
            inputJump = false;
        }
    }

    private void handleHorizontalMouvement() {
        // Applying acceleration if the player gives input.
        if (inputLeft && inputRight) {
            //Do nothing
        } else if (inputLeft && rb.velocity.x > -maxXSpeed) {
            rb.AddForce(Vector2.left * xAcc);
        } else if (inputRight && rb.velocity.x < maxXSpeed) {
            rb.AddForce(Vector2.right * xAcc);
        }

        //If the player goes too fast while moving, their speed is corrected.
        if (rb.velocity.x < -maxXSpeed) {
            rb.velocity = new Vector2(-maxXSpeed, rb.velocity.y);
        }
        if (rb.velocity.x > maxXSpeed) {
            rb.velocity = new Vector2(maxXSpeed, rb.velocity.y);
        }

        //Slowing the player down if they dont give input.
        if ((!inputLeft || inputRight) && rb.velocity.x < 0) {
            rb.AddForce(Vector2.right * xSlowdown);
        }
        if ((!inputRight || inputLeft) && rb.velocity.x > 0) {
            rb.AddForce(Vector2.left * xSlowdown);
        }
        //Stops the player if they aren't trying to move and their speed is low enough.
        if (inputRight == inputLeft && Math.Abs(rb.velocity.x) < 0.2) {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public void setIsGrounded(bool isGrounded) {
        this.isGrounded = isGrounded;
    }

    public void setWallLeft(bool wallLeft) {
        this.wallLeft = wallLeft;
    }

    public void setWallRight(bool wallRight) {
        this.wallRight = wallRight;
    }

    public String getDebugText() {
        return "Velocity: " + rb.velocity
        + "\ninputJump: " + inputJump
        + "\ninputRight: " + inputRight
        + "\ninputLeft: " + inputLeft
        + "\nisGrounded: " + isGrounded
        + "\n Dash Time: " + dashTime;
    }
}
