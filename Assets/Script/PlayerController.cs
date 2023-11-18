using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController:MonoBehaviour
{
    // TIP: [SerializeField] so you can still drag in unity the component
    private Rigidbody2D fox;
    private Animator animator;
    private SpriteRenderer foxSprite;
    private Collider2D foxCollider; // CapsuleCollider2D /-\ NOT IN USE
    private Collider2D footCollider; // BoxCollider2D
    // Audio
    [SerializeField] AudioSource jumpAudio;
    [SerializeField] AudioSource hurtAudio;
    [SerializeField] AudioSource cherryAudio;
    // Start Point
    private Transform startPosition;
    // States enum
    private enum State {idle, running, jumping, falling, crouching, hurt};
    private State stateAnim;
    // UI
    private Canvas canvas;
    // Layer
    private LayerMask groundLayer;
    private LayerMask enemyLayer;
    private LayerMask mapBorderLayer;

    // TESTE LOCAL
    private FrogController Frog;

    // Variables // fox
    private int velocidadeRaposa = 10;
    public float jumpForce = 45f;
    private float hurtImpulse = 30f;
    private float hurtTime = 0;
    private bool isCrouching = false;
    private int foxLife = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Starting variables // fox
        fox = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        foxSprite = GetComponent<SpriteRenderer>();
        foxCollider = GetComponent<CapsuleCollider2D>();
        footCollider = GetComponent<BoxCollider2D>();

        // Layer
        groundLayer = (1 << LayerMask.NameToLayer("Ground")); // Alternative: groundLayer = (1 << 8); == UnityEngine.LayerMask // Obter a camada pelo numero dela, fazendo uma transformação binaria
        enemyLayer = 9;
        mapBorderLayer = 10;

        // Teste LOCAL
        canvas = GameObject.Find("Canvas-UI").GetComponent<Canvas>();

        // Start player in the start position
        startPosition = GameObject.Find("StartPosition").GetComponent<Transform>();
        transform.position = new Vector2(startPosition.position.x, startPosition.position.y); // IMPROVE: create some empty object and use it to spawn player in the map/phase

    }

    // Update is called once per frame
    void Update() {

        if (!(stateAnim == State.hurt)) {
            Movement(); // WASD Jump
        }
        AnimationState();
        animator.SetInteger("state", (int)stateAnim); // Set Animation
    }

    private void Movement() {

        float horizontalD = Input.GetAxis("Horizontal");
        float verticalD = Input.GetAxis("Vertical");

        if (horizontalD < 0) { // Running Verification
            fox.velocity = new Vector2(-1 * velocidadeRaposa, fox.velocity.y);
            // Flip sprite
            // TIP: Can be used for flip sprite too <> transform.localScale = new Vector2(-1, 1);
            foxSprite.flipX = true;

        } else if (horizontalD > 0) {
            fox.velocity = new Vector2(velocidadeRaposa, fox.velocity.y);
            foxSprite.flipX = false;
        }

        if ((Input.GetButtonDown("Jump")) && footCollider.IsTouchingLayers(groundLayer)) { // Jump button and touching ground verification
            jumpAudio.Play();
            Jump();
        } else if ((verticalD < 0) && (footCollider.IsTouchingLayers(groundLayer))) { // Crouching verification
            stateAnim = State.crouching;
            isCrouching = true;
        } else if ((verticalD == 0) && (isCrouching)) { // Stop crouching
            stateAnim = State.idle;
            isCrouching = false;
        }
    }

    private void Jump() {
        //fox.velocity = new Vector2(fox.velocity.x, jumpForce);
        fox.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Method 2 - Working
        stateAnim = State.jumping;
    }

    private void OnTriggerEnter2D(Collider2D trigger) { // Collision trigger check
        
        if (trigger.tag == "Collect-cherry") {

            cherryAudio.Play();
            Destroy(trigger.gameObject);

            canvas.GetComponent<UIControl>().AddCherryScore();
            /*cherrys += 1; // 999
            cherryCount.text = cherrys.ToString(); // 999*/
        }

        if (trigger.gameObject.layer == mapBorderLayer) {
            restartScene();
        }

    }

    private void OnCollisionEnter2D(Collision2D other) { // Collision hit check   

        if (other.gameObject.layer == enemyLayer) { // hit with a enemy

            if (footCollider.IsTouching(other.collider)) {

                other.gameObject.SendMessage("OnHit") ; // a work away?
                Jump();

            } else {
                //hurtTime = 0;
                hurtAudio.Play();
                stateAnim = State.hurt;
                
                if (other.gameObject.transform.position.x > transform.position.x) {
                    fox.AddForce(Vector2.left * hurtImpulse, ForceMode2D.Impulse);
                } else {
                    fox.AddForce(Vector2.right * hurtImpulse, ForceMode2D.Impulse);
                }

                foxLife -= 1;
                canvas.GetComponent<UIControl>().UpdateDebugLog(foxLife);
                lifeController();
            }

        }

        
    }

    private void AnimationState() {
        if ((stateAnim == State.jumping) || ((stateAnim == State.falling) && (!footCollider.IsTouchingLayers(groundLayer)))) { // Make it falls after jumping, and keep falling until touch the ground

            if ((fox.velocity.y < -.1f) && (stateAnim == State.jumping)) {
                stateAnim = State.falling;
            }

        } else if (stateAnim == State.crouching) { // Just keep crouch animation

            // Do nothing

        } else if (stateAnim == State.hurt) { // Hurt animation controll

            /*hurtTime += Time.deltaTime;
            debugLog.text = hurtTime.ToString();*/
            if ((Mathf.Abs(fox.velocity.x) < .5f) /*&& (hurtTime > 1f)*/) {
                stateAnim = State.idle;
            }

        } else if ((fox.velocity.y < -.1f) && (!footCollider.IsTouchingLayers(groundLayer))) { // Make it falls when not touching the ground

            stateAnim = State.falling;

        } else if ((stateAnim == State.falling) && (footCollider.IsTouchingLayers(groundLayer))) { // Idle animation after falls in a ground

            stateAnim = State.idle;

        } else if ((Mathf.Abs(fox.velocity.x) > 2f) && (footCollider.IsTouchingLayers(groundLayer))) { // Running animation

            stateAnim = State.running;

        } else { // Idle animation while stoped

            stateAnim = State.idle;

        }
    }

    private void restartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void lifeController() {
        canvas.GetComponent<UIControl>().UpdateFoxLife(foxLife);
        if (foxLife == 0) {
            stateAnim = State.hurt;
            Debug.Log("partiu");
            hurtTime += Time.deltaTime;
            if (hurtTime > 1f) {
                restartScene();
            }
        }
    }

}
