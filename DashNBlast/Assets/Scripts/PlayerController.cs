using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Direction enum for different player state variables
    public enum Direction { RIGHT, LEFT };

    //Public Unity-set constants
    public float xAcc;
    public float xSlowdown;
    public float ySlowdown;
    public float maxXSpeed;


    public float initJumpSpeed;

    public float initWjSpeed;
    public float wjAngle;

    public float dashSpeed;
    public float dashSpeedDecay;
    public float totalDashTime;
    public float totalDashCooldown;

    public float gScale;
    public float totalJumpTime;

    //Private state variables
    private bool inputHoldJump;
    private bool inputInitJump;
    private bool inputRight;
    private bool inputLeft;
    private bool inputDash;

    private bool wjLeft;
    private bool wjRight;
    private bool jumping;
    private bool grounded;
    private bool wallLeft;
    private bool wallRight;
    private bool dashing;

    private Direction playerDirection;

    private float dashTime;
    private float dashCooldown;
    private float jumpTime;



    private Rigidbody2D rb;


    // Use this for initialization
    void Start() {
        inputHoldJump = false;
        inputInitJump = false;
        inputRight = false;
        inputLeft = false;
        inputDash = false;

        wjLeft = false;
        wjRight = false;
        jumping = false;
        grounded = false;
        wallLeft = false;
        wallRight = false;
        dashing = false;

        playerDirection = Direction.RIGHT;

        dashTime = 0;
        dashCooldown = 0;
        jumpTime = 0;

        dashTime = 0;
        rb = GetComponent<Rigidbody2D>();


        rb.gravityScale = gScale;

    }

    // Update handles player input and other non-physics related calculations
    void Update() {
        HandleInput();
        DeterminePlayerDirection();

    }

    // FixedUpdate deals with 
    void FixedUpdate() {
        HandleDashing();
        if (!dashing) {//No other mouvement is allowed during a dash
            HandleHorizontalMouvement();
            HandleVerticalMouvement();
            HandleJumping();
        }
    }

    private void DeterminePlayerDirection() {
        if (!dashing) {
            if (inputLeft && inputRight) {
            } else if (inputRight) {
                playerDirection = Direction.RIGHT;
            } else if (inputLeft) {
                playerDirection = Direction.LEFT;
            }
        }
        
    }

    private void HandleInput() {
        //Determine input user for the frame
        inputLeft = Input.GetKey(KeyCode.A);
        inputRight = Input.GetKey(KeyCode.D);
        inputHoldJump = Input.GetKey(KeyCode.W);

        if (Input.GetKeyDown(KeyCode.W)) {
            inputInitJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            inputDash = true;
        }
    }

    private void HandleDashing() {
        // Activate dashing if the player gave input and the cooldown has refreshed
        if (inputDash && !dashing && dashCooldown == 0) {
            dashTime = totalDashTime;
            dashCooldown = totalDashCooldown;
            inputDash = false;
            dashing = true;
            rb.gravityScale = 0;
        }
        // Handle dashing
        if (dashing) {
            if (playerDirection == Direction.RIGHT) {
                rb.velocity = Vector2.right * dashSpeed * (float)Math.Pow(Math.E, dashSpeedDecay * (dashTime - totalDashTime));
            } else if (playerDirection == Direction.LEFT) {
                rb.velocity = Vector2.left * dashSpeed * (float)Math.Pow(Math.E, dashSpeedDecay * (dashTime - totalDashTime));
            }
            dashTime -= Time.deltaTime;
            if (dashTime <= 0) {
                rb.gravityScale = gScale;
                dashTime = 0;
                dashing = false;
                inputInitJump = false;
            }
        }
        // Cooldown Management
        if (dashCooldown > 0) {
            dashCooldown -= Time.deltaTime;
        } else if (dashCooldown <= 0) {
            dashCooldown = 0;
        }
    }

    private void HandleJumping() {
        if (inputInitJump && !(jumping || wjRight || wjLeft)) {
            if (grounded) {
                jumping = true;
                jumpTime = totalJumpTime;
            } else if (wallLeft && !grounded) {
                wjRight = true;
                jumpTime = totalJumpTime;
            } else if (wallRight && !grounded) {
                wjLeft = true;
                jumpTime = totalJumpTime;
            }
            inputInitJump = false;
        }
        // If the player hits something he should stop "jumping"
        if (jumping) {
            rb.velocity = new Vector2(rb.velocity.x, initJumpSpeed);
            if (!inputHoldJump || jumpTime <= 0) {
                rb.velocity = new Vector2(rb.velocity.x, 4);
                jumping = false;
            }
            jumpTime -= Time.deltaTime;
        }
        if (wjRight) {
            rb.velocity = new Vector2((float)Math.Cos(wjAngle), (float)Math.Sin(wjAngle)) * initWjSpeed;
            if (!inputHoldJump || jumpTime <= 0) {
                wjRight = false;
            }
            jumpTime -= Time.deltaTime;
        }
        if (wjLeft) {
            rb.velocity = new Vector2(-(float)Math.Cos(wjAngle), (float)Math.Sin(wjAngle)) * initWjSpeed;
            if (!inputHoldJump || jumpTime <= 0) {
                wjLeft = false;
            }
            jumpTime -= Time.deltaTime;
        }


    }

    private void HandleHorizontalMouvement() {
        /*
        // Applying acceleration if the player gives input.
        if (inputLeft && inputRight) {
            //Do nothing
        } else if (inputLeft && rb.velocity.x > -maxXSpeed) {
            rb.velocity += Vector2.left * xAcc;
            if (rb.velocity.x < -maxXSpeed) {//If the player goes too fast while moving, their speed is corrected.
                rb.velocity = new Vector2(-maxXSpeed, rb.velocity.y);
            }
        } else if (inputRight && rb.velocity.x < maxXSpeed) {
            rb.velocity += Vector2.right * xAcc;
            if (rb.velocity.x > maxXSpeed) {//If the player goes too fast while moving, their speed is corrected.
                rb.velocity = new Vector2(maxXSpeed, rb.velocity.y);
            }
        }

        //Slowing the player down if they dont give input and are grounded
        if (inputRight == inputLeft) {
            Slowdown("x", xSlowdown);
        }
        */
        if (inputLeft == inputRight) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else if (inputLeft) {
            rb.velocity = new Vector2(-maxXSpeed, rb.velocity.y);
        } else if (inputRight) {
            rb.velocity = new Vector2(maxXSpeed, rb.velocity.y);
        }
    }

    public void HandleVerticalMouvement() {
        if (!inputInitJump && ((wallLeft && inputLeft) || (wallRight && inputRight))) {
            //Slowdown("y", ySlowdown);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.gravityScale = 0;
        } else {
            rb.gravityScale = gScale;
        }
    }
    /*
    public void Slowdown(String axis, float slowdown) {
        if (axis == "x") {
            if (Math.Abs(rb.velocity.x) > slowdown) {
                if (rb.velocity.x < 0) {
                    rb.velocity += Vector2.right * slowdown;
                }
                if (rb.velocity.x > 0) {
                    rb.velocity += Vector2.left * slowdown;
                }
            } else {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        } else if (axis == "y") {
            if (Math.Abs(rb.velocity.y) > slowdown) {
                if (rb.velocity.y < 0) {
                    rb.velocity += Vector2.up * slowdown;
                }
                if (rb.velocity.y > 0) {
                    rb.velocity += Vector2.down * slowdown;
                }
            } else {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        } else {
            Debug.Log("Calling Slowdown() with invalid axis given");
        }

    }
    */
    public void SetIsGrounded(bool isGrounded) {
        this.grounded = isGrounded;
    }

    public void SetWallLeft(bool wallLeft) {
        this.wallLeft = wallLeft;
    }

    public void SetWallRight(bool wallRight) {
        this.wallRight = wallRight;
    }

    public Direction GetPlayerDirection() {
        return playerDirection;
    }

    public bool GetGrounded() {
        return grounded;
    }

    public bool GetJumping() {
        return jumping;
    }

    public bool GetDashing() {
        return dashing;
    }

    public String GetDebugText() {
        return "Velocity: " + rb.velocity
        + "\ninputJump: " + inputHoldJump
        + "\ninputRight: " + inputRight
        + "\ninputLeft: " + inputLeft
        + "\nisGrounded: " + grounded
        + "\nWall Left: " + wallLeft
        + "\nWall Right: " + wallRight
        + "\nDash Time: " + dashTime
        + "\nDash Cooldown: " + dashCooldown;
    }
}
