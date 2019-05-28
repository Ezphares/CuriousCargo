using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Flipbook))]
public class StaticTransporter : MonoBehaviour, ITransporter
{


    [Header("Dynamic")]
    [SerializeField] Flipbook myFlipbook;
    [SerializeField] List<Transportable> connected;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 subPixelOffset;

    private void Awake()
    {
        myFlipbook = GetComponent<Flipbook>();
    }

    private void FixedUpdate()
    {
        Vector2 amount = subPixelOffset + velocity * Time.fixedDeltaTime;
        Vector2Int now = Vector2Int.FloorToInt(amount);
        subPixelOffset = amount - now;

        myFlipbook.AddDistance(now.magnitude);
        foreach (Transportable transportable in connected)
        {
            transportable.Transport(now);
        }
    }


    public void Connect(Transportable transportable)
    {
        if (!connected.Contains(transportable))
        {
            connected.Add(transportable);
        }
    }

    public void Disconnect(Transportable transportable)
    {
        if (connected.Contains(transportable))
        {
            connected.Remove(transportable);
        }
    }

    public bool IsEnabled()
    {
        return enabled;
    }
}
