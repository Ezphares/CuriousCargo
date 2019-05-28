using UnityEngine;
using System.Collections;

public class Flipbook : MonoBehaviour
{
    [Header("Inscribed")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float fPS;
    public float distancePerFrame = 1.0f;

    [Header("Dynamic")]
    public float position;
    public float distanceTracker;
    public int frame;


    private void Start()
    {
        position = 0.0f;
        distanceTracker = 0.0f;
        frame = 0;
    }

    private void Update()
    {
        position += Time.deltaTime * fPS;

        while (position > 1.0f)
        {
            position -= 1.0f;
            frame = (frame + 1) % sprites.Length;
        }

        spriteRenderer.sprite = sprites[frame];

    }

    public void AddDistance(float amount)
    {
        distanceTracker += amount / distancePerFrame;

        while (distanceTracker > 0.5f)
        {
            distanceTracker -= 1.0f;
            frame = (frame + 1) % sprites.Length;
        }
    }

}
