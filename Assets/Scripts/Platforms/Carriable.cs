using UnityEngine;
using System.Collections;


// TODO: Too much logic in here. Try to spread it in multiple components

[RequireComponent(typeof(PassiveObject))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class Carriable : MonoBehaviour
{
    public enum SimulationType
    {
        PassiveFall,
        PassiveSleep,
        Enemy,
        Carried,
    }

    [System.Serializable]
    public struct CachedData
    {
        public bool isPlatformEffector;
        public AlignedBody2D.PushBehaviour pushBehaviour;
    }

    [Header("Inscribed")]
    public bool canPickUp = true;
    public float recoverTime;
    
    [Header("Dynamic")]
    [SerializeField] SimulationType currentSimulationType;
    [SerializeField] MovingTransporter myMovingTransporter;
    [SerializeField] Transportable myTransportable;
    [SerializeField] PassiveObject myPassiveObject;
    [SerializeField] Collider2D myCollider;
    [SerializeField] PlatformEffector2D myPlatform;
    [SerializeField] AlignedBody2D myMover;
    [SerializeField] IEnemy myEnemyBehaviour;
    [SerializeField] float recoverTimeLeft;
    [SerializeField] CachedData whileAwake;
    [SerializeField] SpriteRenderer[] renderers;

    private void Awake()
    {
        // Required
        myCollider = GetComponent<Collider2D>();
        myPassiveObject = GetComponent<PassiveObject>();
        myPlatform = GetComponent<PlatformEffector2D>();
        myMover = GetComponent<AlignedBody2D>();

        whileAwake.isPlatformEffector = myPlatform.enabled;
        whileAwake.pushBehaviour = myMover.pushBehaviour;

        // Optional
        myMovingTransporter = GetComponent<MovingTransporter>();
        myTransportable = GetComponent<Transportable>();
        myEnemyBehaviour = GetComponent<IEnemy>();

        SetSimulationType(myEnemyBehaviour != null ? SimulationType.Enemy : SimulationType.PassiveFall);

        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        recoverTimeLeft = -1.0f;
    }

    public void OnPickup()
    {
        SetSimulationType(SimulationType.Carried);
    }

    public void OnDrop()
    {
        SetSimulationType(SimulationType.PassiveFall);
    }

    public void OnRecover()
    {
        if (myEnemyBehaviour != null)
        {
            SetSimulationType(SimulationType.Enemy);
        }
    }

    private void FixedUpdate()
    {
        if (currentSimulationType == SimulationType.PassiveFall)
        {
            if (myPassiveObject.IsSleeping())
            {
                SetSimulationType(SimulationType.PassiveSleep);
            }
        }
        else if (currentSimulationType == SimulationType.PassiveSleep)
        {
            if (myPassiveObject.IsSleeping())
            {
                if (recoverTimeLeft > 0.0f)
                {
                    recoverTimeLeft -= Time.fixedDeltaTime;
                    if (recoverTimeLeft <= 0.0f)
                    {
                        OnRecover();
                    }

                    foreach (SpriteRenderer renderer in renderers)
                    {
                        Color c = renderer.color;
                        c.a = recoverTimeLeft % 0.8f >= 0.4f ? 0.3f : 1.0f;
                        renderer.color = c;
                    }
                }
            }
            else
            {
                SetSimulationType(SimulationType.PassiveFall);
            }
        }
        else
        {
            foreach (SpriteRenderer renderer in renderers)
            {
                Color c = renderer.color;
                c.a = 1.0f;
                renderer.color = c;
            }
        }
    }


    void SetSimulationType(SimulationType simulationType)
    {
        switch (simulationType)
        {
            case SimulationType.Carried:
                myEnemyBehaviour?.SetEnabled(false);
                myPassiveObject.enabled = false;
                myTransportable?.SetEnabled(false);
                if (myMovingTransporter)
                {
                    myMovingTransporter.enabled = false;
                }

                myCollider.enabled = false;
                myPlatform.enabled = false;

                if (myEnemyBehaviour != null)
                {
                    myEnemyBehaviour.GetVisualRoot().transform.rotation = Quaternion.identity;

                    foreach (SpriteRenderer renderer in renderers)
                    {
                        renderer.flipY = true;
                    }
                }

                myMover.pushBehaviour = AlignedBody2D.PushBehaviour.PASSTHROUGH;

                break;

            case SimulationType.PassiveSleep:
            case SimulationType.PassiveFall:
                myEnemyBehaviour?.SetEnabled(false);
                myPassiveObject.enabled = true;
                myTransportable?.SetEnabled(true);
                if (myMovingTransporter)
                {
                    myMovingTransporter.enabled = true;
                }

                if (simulationType == SimulationType.PassiveSleep)
                {
                    recoverTimeLeft = recoverTime;
                }

                myCollider.enabled = true;
                myPlatform.enabled = true;
                myMover.pushBehaviour = AlignedBody2D.PushBehaviour.PASSTHROUGH;

                break;

            case SimulationType.Enemy:
                myEnemyBehaviour?.SetEnabled(true);
                myPassiveObject.enabled = false;
                myTransportable?.SetEnabled(false);
                if (myMovingTransporter)
                {
                    myMovingTransporter.enabled = true;
                }

                myCollider.enabled = true;
                myPlatform.enabled = whileAwake.isPlatformEffector;
                myMover.pushBehaviour = whileAwake.pushBehaviour;

                foreach (SpriteRenderer renderer in renderers)
                {
                    renderer.flipY = false;
                }

                break;
        }

        currentSimulationType = simulationType;
    }

}
