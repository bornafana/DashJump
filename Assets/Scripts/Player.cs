using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    #region CONSTANTS
    private const string grabTag = "Grab";

    private const float grabCooldown = 0.2f;
    #endregion

    public int numOfJumps = 1;
    public int numOfAirDashes = 1;

    public bool canWalk = false;
    public float movementSpeed;

    public int totalDashes = 1;
    public Vector2 dashSpeed;
    public int totalJumps = 1;
    public float jumpPower;
    public float positiveCutOffAngle;

    public bool inverseMovement;
    public FloatingJoystick joystick;

    [Header("Audio")]
    public AudioSource jumpSFX;
    public AudioSource LandSFX;
    public AudioSource gemSFX;
    public List<AudioSource> dashSFX;
    private AudioSource dashAudioParent;

    [Header("Debug")]
    public int remainingDashes;
    public int remainingJumps;

    private GroundDetector groundDetector;
    private SpriteRenderer spriteRenderer;
    private float powerDivident = 200f; // Brings dragDistance to a smaller number so that player doesn't jump too far. Normalize didn't work properly.
    private Rigidbody2D rb;
    private Vector2 direction;

    private bool canGrab = true;
    private bool isGrabbing = false;
    private bool isGrounded = true;


    private void Start()
    {
        remainingJumps = totalJumps;
        remainingDashes = totalDashes;

        foreach (Transform child in transform)
        {
            groundDetector = child.GetComponent<GroundDetector>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        dashAudioParent = dashSFX[0].transform.parent.GetComponent<AudioSource>();
        DragIndicator.OnDrag += HandleInput;

        if (powerDivident <= 0f)
            powerDivident = 100f;

        groundDetector.groundEvent.AddListener(Grounded);
        joystick.walkEvent.AddListener(Walk);
    }

    private void Update()
    {
        canGrab = !isGrabbing;
        Debug.Log("Grounded: " + isGrounded);
    }

    private void HandleInput(Vector2 dragDistance)
    {
        if (isGrabbing)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isGrabbing = false;
        }

        if (dragDistance.normalized.y >= positiveCutOffAngle)
        {
            if (numOfJumps > 0)
                Jump(dragDistance);
        }
        else
        {
            Dash(dragDistance);
        }
    }

    private void Jump(Vector2 dragDistance)
    {
        Debug.Log("JUMP");

        if (remainingJumps < 1)
            return;

        spriteRenderer.flipX = dragDistance.x < 0;

        rb.AddForce((dragDistance / powerDivident) * jumpPower, ForceMode2D.Impulse);
        remainingJumps--;

        jumpSFX.Play();
    }

    private void Dash(Vector2 dragDistance)
    {
        Debug.Log("DASH");

        if(remainingDashes < 1)
            return;

        spriteRenderer.flipX = dragDistance.x < 0;

        Vector2 dash = dashSpeed;

        if (dragDistance.x < 0f) //Handles when swiping other direction
        {
            dash.x = -dash.x;
        }

        if (dash.y < 0f)    //So that player never dashes downwards (may change later)
            dash.y = 0f;

        rb.AddForce(dash, ForceMode2D.Impulse);
        remainingDashes--;

        var rand = Random.Range(0, dashSFX.Count - 1);

        if (dashAudioParent.isPlaying)
        {
            dashAudioParent.Stop();
        }

        dashAudioParent.clip = dashSFX[rand].clip;
        dashAudioParent.Play();


    }

    private void GroundDash(float horizontal) // Old dash. Doesn't use Y axis.
    {
        if (inverseMovement)
            direction = Vector2.left * horizontal;
        else
            direction = Vector2.right * horizontal;

        rb.AddForce(direction * dashSpeed, ForceMode2D.Impulse);
    }

    private void Walk(float horizontal)
    {
        if (Mathf.Abs(joystick.Vertical) >= positiveCutOffAngle || !canWalk)
            return;

        if (inverseMovement)
            direction = Vector2.left * horizontal;
        else
            direction = Vector2.right * horizontal;

        spriteRenderer.flipX = direction.x < 0;

        rb.AddForce(direction * movementSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void Grounded()
    {
        isGrounded = true;
        LandSFX.Play();
        remainingJumps = totalJumps;
        remainingDashes = totalDashes;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        isGrounded = false;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Gem"))
        {
            gemSFX.Play();
            Destroy(col.gameObject);
        }
    }

}
