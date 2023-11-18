using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D enemyCollider;
    protected AudioSource deathAudio;


    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        deathAudio = GetComponent<AudioSource>();

    }

    protected void OnHit() {
        animator.SetTrigger("dead");
        deathAudio.Play();
        // disable double kick in the same object
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        enemyCollider.enabled = false;
    }

    protected void Death() {
        Destroy(gameObject);
    }

}
