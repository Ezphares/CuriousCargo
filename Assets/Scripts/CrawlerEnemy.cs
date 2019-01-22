using UnityEngine;
using System.Collections;

public class CrawlerEnemy : MonoBehaviour, IEnemy
{
    [Header("Inscribed")]
    public Transform bodyGraphics;
    public Vector2 targetVelocity;


    [Header("Dynamic")]
    [SerializeField] PixelPerfectMover myMover;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 down;


    // Use this for initialization
    void Start()
    {
        myMover = GetComponent<PixelPerfectMover>();
    }

    private void FixedUpdate()
    {
        Vector2Int delta = myMover.Move(velocity * Time.fixedDeltaTime);

        // Test can move down
        if (delta.magnitude > 0.5f && myMover.CollideArea(Vector2Int.RoundToInt(down.normalized), myMover.collisionMask).Count == 0)
        {
            Vector2 newDown = -velocity;

            velocity = down.normalized * velocity.magnitude;
            down = newDown.normalized;

            // Check if we are flying freely
            if (myMover.CollideArea((Vector2Int.CeilToInt(down) + Vector2Int.CeilToInt(velocity.normalized)) * 2, myMover.collisionMask).Count == 0)
            {
                Debug.Log(Vector2Int.CeilToInt(down));
                Debug.Log(Vector2Int.CeilToInt(velocity.normalized));

                gameObject.SendMessage("OnDrop");
            }

        }

        // Test if hitting wall
        if (myMover.CollideArea(Vector2Int.RoundToInt(velocity.normalized), myMover.collisionMask).Count > 0)
        {
            Vector2 newDown = velocity;

            velocity = -down.normalized * velocity.magnitude;
            down = newDown.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bodyGraphics.rotation = Quaternion.Euler(0.0f, 0.0f, down.y > 0.0f ? 180.0f : 0.0f + down.x * 90);
    }

    public void SetEnabled(bool enable)
    {
        enabled = enable;

        down = Vector2.down;
        velocity = targetVelocity;
    }

    public Transform GetVisualRoot()
    {
        return bodyGraphics;
    }
}
