using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PixelPerfectMover))]
public class Transportable : MonoBehaviour
{
    [Header("Dynamic")]
    [SerializeField] PixelPerfectMover myMover;
    [SerializeField] ITransporter currentTransporter;

    private void Awake()
    {
        myMover = GetComponent<PixelPerfectMover>();
    }

    private void FixedUpdate()
    {
        if (currentTransporter != null && !currentTransporter.IsEnabled())
        {
            Connect(null);
        }

        List<Collider2D> colliders = myMover.CollideArea(Vector2Int.down, myMover.collisionMask);
        ITransporter newTransporter = null;

        foreach (Collider2D collider in colliders)
        {
            ITransporter connected = collider.GetComponent<ITransporter>();
            if (connected != null)
            {
                newTransporter = connected;
            }
        }

        Connect(newTransporter);
    }

    private void OnDestroy()
    {
        Connect(null);
    }

    void Connect(ITransporter transporter)
    {
        if (transporter != currentTransporter)
        {
            if (currentTransporter != null)
            {
                currentTransporter.Disconnect(this);
            }

            if (transporter != null)
            {
                transporter.Connect(this);
            }

            currentTransporter = transporter;
        }
    }

    public Vector2Int Transport(Vector2 amount)
    {
        return myMover.Move(amount);
    }

    public void SetEnabled(bool enable)
    {
        if (!enable)
        {
            Connect(null);
        }


        enabled = enable;
    }
}
