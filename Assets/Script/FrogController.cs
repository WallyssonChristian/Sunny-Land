using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class FrogController : Enemy
{
    // Variables
    private float waitTime = 1.5f;
    private float dt = 0; // timer
    private float jumpHeight = 30f;
    private float jumpLength = 30f;
    private bool readyToJump = true;
    // UI DEBUG
    private Text debugLog;


    protected override void Start()
    {
        base.Start();

        // DEBUG UI
        debugLog = GameObject.Find("DebugLog").GetComponent<Text>();
    }


    void Update() {
        if ((readyToJump) && (enemyCollider.IsTouchingLayers(1 << 8))) { // Preparado pro pulo
            dt += Time.deltaTime;

            if (dt > waitTime) { // Pular
                Jump();
                animator.SetBool("jump", true);
                readyToJump = false;
                dt = 0;
            }
        } else if (enemyCollider.IsTouchingLayers(1 << 8)) { // Preparar pra virar
            dt += Time.deltaTime;

            if (dt > waitTime) { // Virar
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                readyToJump = true;
                dt = 0;
            }
        }

        Animation();
        // DEBUG UI
        //debugLog.text = dt.ToString();
    }


    private void Animation() {
        if (animator.GetBool("jump") && (rb.velocity.y < -.1f)) { // Se estiver no meio do pulo e começar a cair
                animator.SetBool("fall", true);
        }

        if ((enemyCollider.IsTouchingLayers(1 << 8)) && (animator.GetBool("fall"))) {
            animator.SetBool("jump", false);
            animator.SetBool("fall", false);
        }
    }


    private void Jump() {
        rb.AddForce(Vector2.up * (jumpHeight * rb.gravityScale), ForceMode2D.Impulse);
        if (transform.localScale.x == 1) { // Jump to left
            rb.AddForce(Vector2.left * (jumpLength * rb.gravityScale), ForceMode2D.Impulse);
        } else { // Jump to right
            rb.AddForce(Vector2.right * (jumpLength * rb.gravityScale), ForceMode2D.Impulse);
        }
    }

}
