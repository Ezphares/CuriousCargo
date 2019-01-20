using UnityEngine;
using System.Collections;

public class Carriable : MonoBehaviour
{
    [Header("Dynamic")]
    [SerializeField] MovingTransporter myMovingTransporter;
    [SerializeField] Transportable myTransportable;

    private void Awake()
    {
        myMovingTransporter = GetComponent<MovingTransporter>();
        myTransportable = GetComponent<Transportable>();
    }

    public void OnPickup()
    {
        myMovingTransporter.enabled = false;
        myTransportable.SetEnabled(false);
    }

    public void OnDrop()
    {
        myMovingTransporter.enabled = true;
        myTransportable.SetEnabled(true);
    }

}
