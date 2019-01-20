using UnityEngine;
using System.Collections;

public class Snap : MonoBehaviour
{
    private void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 pos = rb.position;
        pos.x = Mathf.Round(pos.x * 16.0f) / 16.0f;
        pos.y = Mathf.Round(pos.y * 16.0f) / 16.0f;
        rb.position = pos;
    }
}
