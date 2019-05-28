using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignedBody2D))]
public class PassiveObject : MonoBehaviour
{
    [Header("Dynamic")]
    [SerializeField] AlignedBody2D myMover;
    [SerializeField] bool sleeping;
    public Vector2 velocity;

    private void Awake()
    {
        myMover = GetComponent<AlignedBody2D>();
        sleeping = true;
    }

    private void FixedUpdate()
    {
        if (!sleeping)
        {
            velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

            myMover.Move(velocity * Time.fixedDeltaTime);
            if (myMover.CollideArea(velocity.y > 0 ? Vector2Int.up : Vector2Int.down).Count > 0)
            {
                if (velocity.y < 0.0f)
                {
                    sleeping = true;
                }

                velocity.y = 0.0f;
            }
            if (myMover.CollideArea(velocity.x > 0 ? Vector2Int.right : Vector2Int.left).Count > 0)
            {
                velocity.x *= -0.5f;
            }
        }
        else
        {
            velocity = Vector2.zero;
            if (myMover.CollideArea(Vector2Int.down).Count == 0)
            {
                Impulse(Vector2.zero);
            }
        }
    }

    public void Impulse(Vector2 amount)
    {
        myMover.Move(transform.position, false);
        sleeping = false;
        velocity = amount;
        enabled = true;
    }

    public bool IsSleeping()
    {
        return sleeping;
    }
}
