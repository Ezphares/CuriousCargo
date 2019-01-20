using UnityEngine;
using System.Collections.Generic;

public class StaticTransporter : MonoBehaviour, ITransporter
{
    [Header("Dynamic")]
    [SerializeField] List<Transportable> connected;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 subPixelOffset;

    private void FixedUpdate()
    {
        Vector2 amount = subPixelOffset + velocity * Time.fixedDeltaTime;
        Vector2Int now = Vector2Int.FloorToInt(amount);
        subPixelOffset = amount - now;

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
