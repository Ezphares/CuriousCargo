using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PixelPerfectMover))]
public class MovingTransporter : MonoBehaviour, ITransporter
{
    [Header("Dynamic")]
    [SerializeField] PixelPerfectMover myMover;
    [SerializeField] List<Transportable> connected;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2Int cachedPosition;

    private void Awake()
    {
        myMover = GetComponent<PixelPerfectMover>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 delta = myMover.position - cachedPosition;
        delta.y = Mathf.Min(delta.y, 0.0f);
        cachedPosition = myMover.position;

        foreach(Transportable transportable in connected)
        {
            transportable.Transport(delta);
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
