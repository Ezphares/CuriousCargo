using UnityEngine;
using System.Collections;

public class Flipbook : MonoBehaviour
{
    [Header("Inscribed")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float fPS;

    [Header("Dynamic")]
    public float position;
    public int frame;


    private void Start()
    {
        position = 0.0f;
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

}
