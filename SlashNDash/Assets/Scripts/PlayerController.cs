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
    public float wallJumpAngle;

    public float dashSpeed;
    public float totalDashTime;
    public float totalDashCooldown;

    private bool inputJump;
    private bool inputRight;
    private bool inputLeft;
    private bool inputDash;

    private bool isGrounded;
    private bool wallLeft;
    private bool wallRight;
    private bool dashing;

    private float dashTime;
    private float dashCooldown;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    // Use this for initialization
    void Start() {
        inputJump = false;
        inputRight = false;
        inputLeft = false;
        inputDash = false;

        isGrounded = false;
        wallLeft = false;
        wallRight = false;

        dashTime = 0;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update() {
        getInput();
    }

    private void getInput() {
        anim.SetInteger("State", 0);
        //Getting input from the user.
        if (Input.GetKey(KeyCode.A)) {
            sr.flipX = true;
            inputLeft = true;
            anim.SetInteger("State", 1);
        } else {
            inputLeft = false;
        }
        if (Input.GetKey(KeyCode.D)) {
            sr.flipX = false;
            inputRight = true;
            anim.SetInteger("State", 1);
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
        HandleDashing();
        if (!dashing) {// Not dashing.
            HandleHorizontalMouvement();
            HandleJumping();
        }
    }

    private void HandleDashing() {
        //Activate dashing if the player gave input and the cooldown has refreshed
        if (inputDash && dashCooldown == 0) {
            dashTime = totalDashTime;
            dashCooldown = totalDashCooldown;
            inputDash = false;
            dashing = true;
        }
        if (dashing) {
            //This is wrong, the direction can change mid dash, which is not what i want
            //Gotta put a variable determining the side that the player is facing at all times.
            if (inputRight && inputLeft) {
            } else if (inputRight) {
                rb.velocity = new Vector2(1, 0.1f) * dashSpeed * (float)Math.Pow(Math.E, dashTime - totalDashTime);
            } else if (inputLeft) {
                rb.velocity = new Vector2(-1, 0.1f) * dashSpeed * (float)Math.Pow(Math.E, dashTime - totalDashTime);
            }
            dashTime -= Time.deltaTime;
            if (dashTime < 0) {
                dashTime = 0;
                dashing = false;
            }
        }
        dashCooldown -= Time.deltaTime;
        if (dashCooldown < 0) {
            dashCooldown = 0;
        }
    }

    private void HandleJumping() {
        if (inputJump) {
            if (isGrounded) {
                rb.velocity = new Vector2(rb.velocity.x, initJumpVelocity);
            }
            if (wallLeft && !isGrounded) {
                rb.velocity = new Vector2((float)Math.Cos(wallJumpAngle), (float)Math.Sin(wallJumpAngle)) * initWallJumpSpeed;
            }
            if (wallRight && !isGrounded) {

                rb.velocity = new Vector2(-(float)Math.Cos(wallJumpAngle), (float)Math.Sin(wallJumpAngle)) * initWallJumpSpeed;
            }
            inputJump = false;
        }
    }

    private void HandleHorizontalMouvement() {
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

        //Slowing the player down if they dont give input and are grounded
        if (isGrounded) {
            if ((!inputLeft || inputRight) && rb.velocity.x < 0) {
                rb.AddForce(Vector2.right * xSlowdown);
            }
            if ((!inputRight || inputLeft) && rb.velocity.x > 0) {
                rb.AddForce(Vector2.left * xSlowdown);
            }
        }
        //Stops the player if they aren't trying to move and their speed is low enough.
        if (inputRight == inputLeft && Math.Abs(rb.velocity.x) < 0.2) {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public void SetIsGrounded(bool isGrounded) {
        this.isGrounded = isGrounded;
    }

    public void SetWallLeft(bool wallLeft) {
        this.wallLeft = wallLeft;
    }

    public void SetWallRight(bool wallRight) {
        this.wallRight = wallRight;
    }

    public String GetDebugText() {
        return "Velocity: " + rb.velocity
        + "\ninputJump: " + inputJump
        + "\ninputRight: " + inputRight
        + "\ninputLeft: " + inputLeft
        + "\nisGrounded: " + isGrounded
        + "\n Wall Left: " + wallLeft
        + "\n Wall Right: " + wallRight
        + "\nDash Time: " + dashTime
        + "\nDash Cooldown: " + dashCooldown;
    }
}
