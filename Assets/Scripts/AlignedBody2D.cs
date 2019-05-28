using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class AlignedBody2D : MonoBehaviour
{
    public enum MaskSelection
    {
        COLLISION,
        PUSH,
    }

    public enum PushBehaviour
    {
        PUSH,
        BLOCKED, 
        PASSTHROUGH,
    }

    static bool PUSH_LOCK;

    [Header("Inscribed")]
    public LayerMask collisionMask;
    public LayerMask pushMask;
    public PushBehaviour pushBehaviour = PushBehaviour.PUSH;

    [Header("Dynamic")]
    [SerializeField] Collider2D myCollider;
    [SerializeField] Vector2Int _position;
    [SerializeField] Vector2 subPixelPositionCounter;
    public Vector2Int position
    {
        get { return _position; }
        set { _position = value; subPixelPositionCounter = Vector2.zero; }
    }

    // Internals
    ContactFilter2D collisionFilter;
    ContactFilter2D pushFilter;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        PUSH_LOCK = false;

        collisionFilter = new ContactFilter2D { useLayerMask = true, layerMask = collisionMask };
        pushFilter = new ContactFilter2D { useLayerMask = true, layerMask = pushMask };
    }

    // Start is called before the first frame update
    void Start()
    {
        _position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public Vector2Int Move(Vector2 move, bool relative = true)
    {
        Vector2Int start = _position;
        if (relative)
        {
            MoveX(move.x);
            MoveY(move.y);
        }
        else
        {
            _position = Vector2Int.RoundToInt(move);
            subPixelPositionCounter = move - _position;
        }

        transform.position = (Vector3)((Vector2)_position) + Vector3.forward * transform.position.z;
        return _position - start;
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
        if (CollideArea(direction, MaskSelection.COLLISION).Count == 0)
        {
            success = true;

            if (pushBehaviour != PushBehaviour.PASSTHROUGH)
            {
                List<Collider2D> pushables = CollideArea(direction, MaskSelection.PUSH);
                for (int i = 0; i < pushables.Count; i++)
                {
                    if (PUSH_LOCK || pushBehaviour == PushBehaviour.BLOCKED)
                    {
                        success = false;
                        break;
                    }

                    // Prevent recursive pushes
                    PUSH_LOCK = true;
                    pushables[i].GetComponent<AlignedBody2D>().Move(direction);
                    PUSH_LOCK = false;
                }
            }

            _position += direction;
        }

        return success;
    }

    public List<Collider2D> CollideArea(Vector2Int offset, MaskSelection maskSelection = MaskSelection.COLLISION, bool relative = true)
    {
        ContactFilter2D filter = maskSelection == MaskSelection.COLLISION ? collisionFilter : pushFilter;

        List<Collider2D> results = new List<Collider2D>();
        Collider2D[] tempResults = new Collider2D[10];

        if(!myCollider)
        {
            return results;
        }

        Vector2 center = (Vector2)(myCollider.bounds.center - transform.position) + offset;
        if (relative)
        {
            center += _position;
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
            if (effector && effector.enabled && effector.useOneWay)
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
