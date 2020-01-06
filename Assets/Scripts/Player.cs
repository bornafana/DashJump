using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2 dashSpeed;
    [SerializeField] private float jumpPower;
    public float positiveCutOffAngle;
    public float negativeCutOffAngle;

    public bool inverseMovement;
    public FloatingJoystick joystick;

    private float powerDivident = 200f; // Brings dragDistance to a smaller number so that player doesn't jump too far. Normalize didn't work properly.
    private Rigidbody2D rb;
    private Vector2 direction;

    private bool canGrab = true;
    private bool isGrabbing = false;
    private bool isGrounded = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DragIndicator.OnDrag += HandleInput;

        if (powerDivident <= 0f)
            powerDivident = 100f;

        //joystick.dashEvent.AddListener(Dash);
        joystick.walkEvent.AddListener(Walk);
    }

    private void Update()
    {
        canGrab = !isGrabbing;
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
        rb.AddForce((dragDistance / powerDivident) * jumpPower, ForceMode2D.Impulse);
        Debug.Log("JUMP");
    }

    private void Dash(Vector2 dragDistance)
    {
        Vector2 dash = dashSpeed;

        if (dragDistance.x < 0f)    //Handles when swiping 
            dash.x = -dash.x;

        if (dash.y < 0f)    //So that player never dashes downwards (may change later)
            dash.y = 0f;

        rb.AddForce(dash, ForceMode2D.Impulse);
        Debug.Log("DASH");

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

        rb.AddForce(direction * movementSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        isGrounded = true;

        if (canGrab)
        {
            if (col.collider.CompareTag(grabTag))
            {
                rb.bodyType = RigidbodyType2D.Static;
                isGrabbing = true;
                Debug.Log("Grabbed");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        isGrounded = false;
    }


}
