using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {

    private Rigidbody2D rb;
    private PlayerController ctrl;
    private Animator anim;
    private SpriteRenderer sr;

    private const int IDLE = 0;
    private const int WALKING = 1;
    private const int JUMPING = 2;
    private const int MID_AIR = 3;


    // Use this for initialization
    void Start() {
        ctrl = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        //Determine what direction the player is facing
        if (ctrl.GetPlayerDirection() == PlayerController.Direction.RIGHT) {
            sr.flipX = false;
        } else if (ctrl.GetPlayerDirection() == PlayerController.Direction.LEFT) {
            sr.flipX = true;
        }

        detAnimState();

    }

    private void detAnimState() {
        
        if (ctrl.GetJumping()) {
            anim.SetInteger("State", 2);
        } else if (ctrl.GetDashing()) {
            anim.SetInteger("State", 4);
        } else {
            if (ctrl.GetGrounded()) {
                if (rb.velocity.x == 0) {
                    anim.SetInteger("State", 0);
                } else {
                    anim.SetInteger("State", 1);
                }
            } else {
                anim.SetInteger("State", 3);
            }
        }
        anim.SetBool("Dashing", ctrl.GetDashing());



    }
}
