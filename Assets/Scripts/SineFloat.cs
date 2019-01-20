using UnityEngine;
using System.Collections;

public class SineFloat : MonoBehaviour
{
    [Header("Inscribed")]
    public Vector2 scale = Vector2.right * 8.0f;
    public float frequency = 1.0f;

    [Header("Dynamic")]
    public Vector2 start;
    public float deltaT;

    // Use this for initialization
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        deltaT += Time.deltaTime * frequency;

        transform.position = start + scale * Mathf.Sin(deltaT * 2 * Mathf.PI);
    }
}
