using UnityEngine;
using System.Collections;

public interface ITransporter
{
    void Connect(Transportable transportable);
    void Disconnect(Transportable transportable);
    bool IsEnabled();
}
