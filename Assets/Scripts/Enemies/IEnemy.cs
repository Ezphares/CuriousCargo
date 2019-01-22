using UnityEngine;

public interface IEnemy
{
    void SetEnabled(bool enabled);
    Transform GetVisualRoot();
}
