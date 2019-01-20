using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Carrier))]
public class CrawlerEnemy : MonoBehaviour
{
    [Header("Inscribed")]
    public Transform bodyGraphics;

    [Header("Dynamic")]
    [SerializeField] PixelPerfectMover myMover;
    [SerializeField] MovingTransporter myCarrier;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 down;


    // Use this for initialization
    void Start()
    {
        myMover = GetComponent<PixelPerfectMover>();
    }

    private void FixedUpdate()
    {
        myMover.Move(velocity * Time.fixedDeltaTime);

        // Test can move down
        if (myMover.CollideArea(Vector2Int.RoundToInt(down.normalized), myMover.collisionMask).Count == 0)
        {
            Vector2 newDown = -velocity;

            velocity = down.normalized * velocity.magnitude;
            down = newDown.normalized;
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
}
