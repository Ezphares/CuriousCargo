﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Collider2D))]
public class PixelPerfectMover : MonoBehaviour
{
    static bool PUSH_LOCK;


    [Header("Inscribed")]
    public ContactFilter2D collisionMask;
    public ContactFilter2D pushMask;

    [Header("Dynamic")]
    [SerializeField] Collider2D myCollider;
    public Vector2Int position;
    [SerializeField] Vector2 subPixelPositionCounter;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        PUSH_LOCK = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public Vector2Int Move(Vector2 move, bool relative = true)
    {
        Vector2Int start = position;
        if (relative)
        {
            MoveX(move.x);
            MoveY(move.y);
        }
        else
        {
            position = Vector2Int.RoundToInt(move);
            subPixelPositionCounter = move - position;
        }

        transform.position = (Vector3)((Vector2)position) + Vector3.forward * transform.position.z;
        return position - start;
    }

    private void MoveX(float amount)
    {
        amount += subPixelPositionCounter.x;

        Vector2Int direction = amount > 0.0f ? Vector2Int.right : Vector2Int.left;
        while (Mathf.Abs(amount) > 0.5f)
        {
            amount -= direction.x;
            if (!TryMoveFinal(direction))
            {
                amount = 0.0f;
            }

        }

        subPixelPositionCounter.x = amount;
    }

    private void MoveY(float amount)
    {
        amount += subPixelPositionCounter.y;

        Vector2Int direction = amount > 0.0f ? Vector2Int.up : Vector2Int.down;
        while (Mathf.Abs(amount) > 0.5f)
        {
            amount -= direction.y;
            if (!TryMoveFinal(direction))
            {
                amount = 0.0f;
            }
        }

        subPixelPositionCounter.y = amount;
    }

    private bool TryMoveFinal(Vector2Int direction)
    {
        bool success = false;
        if (CollideArea(direction, collisionMask).Count == 0)
        {
            success = true;

            int i = 0;
            List<Collider2D> pushables = CollideArea(direction, pushMask);
            for (i = 0; i < pushables.Count; i++)
            {
                if (PUSH_LOCK)
                {
                    success = false;
                    break;
                }

                // Prevent recursive pushes
                PUSH_LOCK = true;
                pushables[i].GetComponent<PixelPerfectMover>().Move(direction);
                PUSH_LOCK = false;
            }

            position += direction;
        }

        return success;
    }

    public List<Collider2D> CollideArea(Vector2Int offset, ContactFilter2D filter, bool relative = true)
    {
        List<Collider2D> results = new List<Collider2D>();
        Collider2D[] tempResults = new Collider2D[10];

        Vector2 center = (Vector2)(myCollider.bounds.center - transform.position) + offset;
        if (relative)
        {
            center += position;
        }

        Vector2 size = myCollider.bounds.size;
        size.x -= 0.1f;
        size.y -= 0.1f;

        int hits = Physics2D.OverlapBox(center, size, 0.0f, filter, tempResults);
        for (int i = 0; i < hits; i++)
        {
            if (tempResults[i].gameObject == gameObject)
            {
                continue;
            }

            bool hit = true;
            PlatformEffector2D effector = tempResults[i].transform.GetComponent<PlatformEffector2D>();
            if (effector && effector.useOneWay)
            {
                hit &= Vector2.Angle(offset, effector.transform.up) > effector.surfaceArc / 2;
                hit &= Mathf.CeilToInt(myCollider.bounds.min.y) >= Mathf.FloorToInt(tempResults[i].bounds.max.y);

            }

            if (hit)
            {
                results.Add(tempResults[i]);
            }
        }

        return results;
    }
}
