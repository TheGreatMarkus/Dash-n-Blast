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
    public float maxXSpeed;

    public float jumpSpeed;

    public float initWjSpeed;
    public float wjAngle;

    public float dashSpeed;
    public float dashSpeedDecay;
    public float totalDashTime;
    public float totalDashCooldown;

    public float gScale;
    public float totalJumpTime;
    public float totalWjTime;

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

        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = gScale;

    }

    // Update handles player input and other non-physics related calculations
    void Update() {

        // HANDLING INPUT

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

        // HANDLING THE DIRECTION THE PLAYER IS FACING

        if (!dashing) {
            if (inputLeft && inputRight) {
            } else if (inputRight) {
                playerDirection = Direction.RIGHT;
            } else if (inputLeft) {
                playerDirection = Direction.LEFT;
            }
        }


    }

    // FixedUpdate deals with 
    void FixedUpdate() {
        HandleDashing();
        if (!dashing) {//No other mouvement is allowed during a dash
            HandleNormalPlayerMouvement();
        }
    }

    private void HandleNormalPlayerMouvement() {
        //-----------------------------------
        // HORINZONTAL MOUVEMENT
        //-----------------------------------

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

        //Slowing the player down if they dont move and are grounded
        if (inputRight == inputLeft) {
            Slowdown(xSlowdown);
        }

        //-----------------------------
        // VERTICAL MOUVEMENT
        //-----------------------------

        if (wallLeft && inputLeft || wallRight && inputRight) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.gravityScale = 0;
        } else if (wallLeft || wallRight) {
            rb.velocity = new Vector2(rb.velocity.x, -2);
            rb.gravityScale = 0;
        } else {
            rb.gravityScale = gScale;
        }

        //-----------------------------
        // JUMPING
        //-----------------------------

        if (inputInitJump && !(jumping || wjRight || wjLeft)) {
            if (grounded) {
                jumping = true;
                jumpTime = totalJumpTime;
            } else if (wallLeft && !grounded) {
                wjRight = true;
                jumpTime = totalWjTime;
            } else if (wallRight && !grounded) {
                wjLeft = true;
                jumpTime = totalWjTime;
            }
            inputInitJump = false;
        }
        /* 
        Note: If the player hits something he should stop "jumping"
        Example, if the player hits a ceiling during a jump, the jump should stop
        Example: If the player hits a wall during a wall jump, the jump should stop 
        */
        if (jumping) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            if (!inputHoldJump || jumpTime <= 0) {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                jumping = false;
            }
        }
        if (wjRight) {
            rb.velocity = new Vector2((float)Math.Cos(wjAngle), (float)Math.Sin(wjAngle)) * initWjSpeed;
            if (!inputHoldJump || jumpTime <= 0) {
                wjRight = false;
            }
        }
        if (wjLeft) {
            rb.velocity = new Vector2(-(float)Math.Cos(wjAngle), (float)Math.Sin(wjAngle)) * initWjSpeed;
            if (!inputHoldJump || jumpTime <= 0) {
                wjLeft = false;
            }
        }

        if (jumping || wjRight || wjLeft) {
            jumpTime -= Time.deltaTime;
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
        } else if (dashCooldown < 0) {
            dashCooldown = 0;
        }
    }

    public void Slowdown(float slowdown) {
        if (Math.Abs(rb.velocity.x) >= slowdown) {
            if (rb.velocity.x < 0) {
                rb.velocity += Vector2.right * slowdown;
            }
            if (rb.velocity.x > 0) {
                rb.velocity += Vector2.left * slowdown;
            }
        } else {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

    }
    public void SetIsGrounded(bool grounded) {
        this.grounded = grounded;
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
