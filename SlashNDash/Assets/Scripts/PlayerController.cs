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

    public float initJumpVelocity;

    public float initWallJumpSpeed;
    public float wallJumpAngle;

    public float dashSpeed;
    public float totalDashTime;
    public float totalDashCooldown;

    public float gravityScale;
    public float totalJumpTime;

    //Private state variables
    private bool inputHoldJump;
    private bool inputInitJump;
    private bool inputRight;
    private bool inputLeft;
    private bool inputDash;

    private bool wallJumpLeft;
    private bool wallJumpRight;
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
    private Animator anim;
    private SpriteRenderer sr;



    // Use this for initialization
    void Start() {
        inputHoldJump = false;
        inputInitJump = false;
        inputRight = false;
        inputLeft = false;
        inputDash = false;

        wallJumpLeft = false;
        wallJumpRight = false;
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
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = gravityScale;

    }

    // Update is called once per frame
    void Update() {
        HandleInput();
        DeterminePlayerDirection();

    }

    private void FixedUpdate() {
        HandleDashing();
        if (!dashing) {//No other mouvement is allowed during a dash
            HandleHorizontalMouvement();
            HandleJumping();
        }
    }

    //playerDirection is used for animations and for dashing
    private void DeterminePlayerDirection() {
        if (!dashing) {
            if (inputLeft && inputRight) {
            } else if (inputRight) {
                playerDirection = Direction.RIGHT;
            } else if (inputLeft) {
                playerDirection = Direction.LEFT;
            }
        }
        if (playerDirection == Direction.RIGHT) {
            sr.flipX = false;
        } else if (playerDirection == Direction.LEFT) {
            sr.flipX = true;
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
        //Activate dashing if the player gave input and the cooldown has refreshed
        if (inputDash && dashCooldown == 0) {
            dashTime = totalDashTime;
            dashCooldown = totalDashCooldown;
            inputDash = false;
            dashing = true;
            rb.gravityScale = 0;
        }
        if (dashing) {
            if (playerDirection == Direction.RIGHT) {
                rb.velocity = new Vector2(1, 0) * dashSpeed * (float)Math.Pow(Math.E, 3 * (dashTime - totalDashTime));
            } else if (playerDirection == Direction.LEFT) {
                rb.velocity = new Vector2(-1, 0) * dashSpeed * (float)Math.Pow(Math.E, 3 * (dashTime - totalDashTime));
            }
            dashTime -= Time.deltaTime;
            if (dashTime <= 0) {
                rb.gravityScale = gravityScale;
                dashTime = 0;
                dashing = false;
                inputHoldJump = false;
            }
        }
        if (dashCooldown > 0) {
            dashCooldown -= Time.deltaTime;
        } else if (dashCooldown <= 0) {
            dashCooldown = 0;
        }
    }

    private void HandleJumping() {
        if (inputInitJump && !(jumping || wallJumpRight || wallJumpLeft)) {
            if (grounded) {
                jumping = true;
                jumpTime = totalJumpTime;
            } else if (wallLeft && !grounded) {
                wallJumpRight = true;
                //jumpTime = totalJumpTime;
            } else if (wallRight && !grounded) {
                wallJumpLeft = true;
                //jumpTime = totalJumpTime;
            }
            inputInitJump = false;
        }

        if (jumping) {
            rb.velocity = new Vector2(rb.velocity.x, initJumpVelocity);
            if (!inputHoldJump || jumpTime <= 0) {
                jumping = false;
            }
            jumpTime -= Time.deltaTime;
        }
        //Maybe also allow wall jump to be ajusted based on how long the button was held.
        //float tempA = wallJumpAngle * (1 + 3 * (totalJumpTime - jumpTime));
        if (wallJumpRight) {
            rb.velocity = new Vector2((float)Math.Cos(wallJumpAngle), (float)Math.Sin(wallJumpAngle)) * initWallJumpSpeed;
            //if (!inputHoldJump || jumpTime <= 0) {
            wallJumpRight = false;
            //}
            //jumpTime -= Time.deltaTime;
        }
        if (wallJumpLeft) {
            rb.velocity = new Vector2(-(float)Math.Cos(wallJumpAngle), (float)Math.Sin(wallJumpAngle)) * initWallJumpSpeed;
            // if (!inputHoldJump || jumpTime <= 0) {
            wallJumpLeft = false;
            //}
            //jumpTime -= Time.deltaTime;
        }


    }

    private void HandleHorizontalMouvement() {
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

        //Problematic, slows down too fast.
        if (inputRight == inputLeft) {
            if (rb.velocity.x < 0) {
                rb.velocity += Vector2.right * xSlowdown * -rb.velocity.x;
            }
            if (rb.velocity.x > 0) {
                rb.velocity += Vector2.left * xSlowdown * rb.velocity.x;
            }
        }

        //Puting this here for now. not sure what is its best place
        if (!inputInitJump && ((wallLeft && inputLeft) || (wallRight && inputRight))) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.gravityScale = 0;
        } else {
            rb.gravityScale = gravityScale;
        }
    }

    public void SetIsGrounded(bool isGrounded) {
        this.grounded = isGrounded;
    }

    public void SetWallLeft(bool wallLeft) {
        this.wallLeft = wallLeft;
    }

    public void SetWallRight(bool wallRight) {
        this.wallRight = wallRight;
    }

    public String GetDebugText() {
        return "Velocity: " + rb.velocity
        + "\ninputJump: " + inputHoldJump
        + "\ninputRight: " + inputRight
        + "\ninputLeft: " + inputLeft
        + "\nisGrounded: " + grounded
        + "\n Wall Left: " + wallLeft
        + "\n Wall Right: " + wallRight
        + "\nDash Time: " + dashTime
        + "\nDash Cooldown: " + dashCooldown;
    }
}
