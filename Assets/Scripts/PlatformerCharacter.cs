﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

[RequireComponent(typeof(AlignedBody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Inscribed")]
    public float walkSpeed;
    public float jumpVelocity;
    public float fallGravityModifier = 1.25f;
    public float jumpLeeway = 0.1f;
    public int standRequiredHeadroom = 16;
    public Transform carryPivot;
    public Collider2D carryCollider;

    [Header("Dynamic")]
    [SerializeField] Vector2 pendingMovement;
    [SerializeField] AlignedBody2D myMover;
    [SerializeField] MultiPartCharacter myMultipart;
    [SerializeField] Vector2 velocity;
    [SerializeField] float walkDirection;
    [SerializeField] bool grounded;
    [SerializeField] bool shouldJump;
    [SerializeField] bool shouldCrouch;
    [SerializeField] bool sustainedJump;
    [SerializeField] bool shouldPickUp;
    [SerializeField] bool shouldThrow;
    [SerializeField] Carriable carried;
    [SerializeField] Vector3 facing;
    [SerializeField] float currentJumpLeeway;

    private void Awake()
    {
        myMover = GetComponent<AlignedBody2D>();
        myMultipart = GetComponent<MultiPartCharacter>();
    }

    private void Start()
    {
        walkDirection = 0.0f;
    }

    private void FixedUpdate()
    {
        if (walkDirection != 0.0f && Mathf.Sign(walkDirection) != Mathf.Sign(facing.x))
        {
            facing = (Vector3.right * walkDirection).normalized;

            Vector3 scale = transform.localScale;
            scale.x = facing.x;
            transform.localScale = scale;
        }

        grounded = myMover.CollideArea(Vector2Int.down).Count > 0;
                    
        if (myMover.CollideArea(velocity.y > 0.0f ? Vector2Int.up : Vector2Int.down).Count > 0)
        {
            velocity.y = 0.0f;
        }

        if (myMover.CollideArea(velocity.x > 0.0f ? Vector2Int.right : Vector2Int.left).Count > 0)
        {
            velocity.x = 0.0f;
        }

        // Disable switching crouch mode while in the air
        if (!grounded)
        {
            shouldCrouch = myMultipart.isCrouched;
        }

        if (shouldCrouch || myMultipart.isCrouched)
        {
            velocity.x = 0.0f;
            walkDirection = 0.0f;
        }

        bool crouchChanged = false;
        if (myMultipart.isCrouched != shouldCrouch)
        {
            if (shouldCrouch)
            {
                myMultipart.isCrouched = true;

            }
            else if (CanStand())
            {
                myMultipart.isCrouched = false;
            }
            crouchChanged = myMultipart.isCrouched == shouldCrouch;
        }


        sustainedJump &= Input.GetButton("Jump") && velocity.y > 0.0f;
        float gravityModifier = sustainedJump ? 1.0f : fallGravityModifier;

        velocity.y += Physics2D.gravity.y * gravityModifier * Time.fixedDeltaTime;

        if (grounded)
        {
            currentJumpLeeway = 1.0f;
        }
        else
        {
            currentJumpLeeway -= Time.fixedDeltaTime / jumpLeeway;
        }


        if (shouldJump && currentJumpLeeway > 0.0f && !myMultipart.isCrouched)
        {
            velocity.y = jumpVelocity;
            sustainedJump = true;
            grounded = false;
            currentJumpLeeway = 0.0f;
        }
        shouldJump = false;

        if (grounded)
        {
            velocity.x = walkDirection * walkSpeed;
        }
        else
        {
            if (myMultipart.isCrouched)
            {

            }
            else
            {
                velocity.x = Mathf.Clamp(velocity.x + walkDirection * walkSpeed * 15.0f * Time.fixedDeltaTime,
                    -walkSpeed, walkSpeed);
            }
        }

        // Don't change crouch and pickup same frame, colliders are not updated
        if (!crouchChanged)
        {
            if (shouldPickUp)
            {
                TryPickUp();

                shouldPickUp = false;
            }

            if (shouldThrow)
            {
                TryThrow();

                shouldThrow = false;
            }
        }

        myMover.Move(velocity * Time.fixedDeltaTime);
    }

    private void Update()
    {
        bool crushed = myMover.CollideArea(Vector2Int.zero).Count > 0;

        if (crushed)
        {
            if (carried)
            {
                TryThrow();
            }
            else if (!myMultipart.isCrouched)
            {
                myMultipart.isCrouched = true;
            }
            else
            {
                Kill();
            }
        }
    }

    public void AddWalkInput(float amount)
    {
        walkDirection = amount;
    }

    public void Move(Vector2 movement)
    {
        myMover.Move(movement);
    }

    public void Kill()
    {
        foreach (MultiPartCharacter.CharacterPart part in myMultipart.parts)
        {
            Rigidbody2D prb = part.transform.gameObject.AddComponent<Rigidbody2D>();
            prb.AddForce(Random.insideUnitCircle * 32f + Vector2.up * 64f, ForceMode2D.Impulse);
            prb.AddTorque(Random.Range(1f, 10f), ForceMode2D.Impulse);
            part.transform.parent = null;
        }
        Destroy(carryCollider.gameObject);
        Destroy(gameObject);

    }

    void TryPickUp()
    {
        if (myMultipart.isCrouched && carried == null)
        {
            // Overhead area is blocked
            if (myMover.CollideArea(Vector2Int.up * 16).Count > 0)
            {
                return;
            }

            foreach (Collider2D collider in myMover.CollideArea(Vector2Int.down))
            {
                Carriable carriable = collider.GetComponent<Carriable>();
                if (!carriable)
                {
                    continue;
                }

                carryCollider.enabled = true;

                carried = carriable;
                carried.OnPickup();
                carried.transform.parent = carryPivot;
                carried.transform.localPosition = Vector2.zero;

                break;
            }
        }
    }

    void TryThrow()
    {
        carryCollider.enabled = false;

        carried.GetComponent<PassiveObject>().Impulse(velocity + new Vector2(64.0f * facing.x, 50.0f));
        carried.GetComponent<Collider2D>().enabled = true;
        carried.transform.parent = null;
        carried.OnDrop();
        carried = null;

    }

    public void Jump()
    {
        shouldJump = true;
    }

    public void PickUp()
    {
        if (carried)
        {
            shouldThrow = true;
        }
        else
        {
            shouldPickUp = true;
        }
    }

    public void Crouch()
    {
        shouldCrouch = true;
    }

    public void Stand()
    {
        shouldCrouch = false;
    }

    bool CanStand()
    {
        return myMover.CollideArea(Vector2Int.up * standRequiredHeadroom).Count == 0;
    }

    public Vector2 GetMovement(bool consume = true)
    {
        Vector2 temp = pendingMovement;
        if (consume)
        {
            pendingMovement = Vector2.zero;
        }

        return temp;
    }
}
