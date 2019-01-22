using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class FollowCamera : MonoBehaviour
{
    [Header("Inscribed")]
    public Transform follow;
    public Tilemap containingMap;
    public SpriteRenderer background;

    [Header("Dynamic")]
    [SerializeField] Camera myCamera;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        if (containingMap)
        {
            containingMap.CompressBounds();
        }

        background.size = containingMap.localBounds.size + Vector3.one * 16.0f;
        background.transform.position = (containingMap.localBounds.center + containingMap.transform.position) + Vector3.forward * background.transform.position.z;
    }


    void Update()
    {
        Vector3 target = transform.position;
        if (follow)
        {
            target.x = follow.position.x;
            target.y = follow.position.y;
        }

        if (containingMap)
        {
            Vector2 halfSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);
            Vector2 mapExtents = containingMap.localBounds.extents;
            Vector2 mapCenterWorld = containingMap.transform.position + containingMap.localBounds.center;
            Vector2 minPos = mapCenterWorld - mapExtents + halfSize;
            Vector2 maxPos = mapCenterWorld + mapExtents - halfSize;

            if (halfSize.x < mapExtents.x)
            {
                target.x = Mathf.Clamp(target.x, minPos.x, maxPos.x);
            }
            else
            {
                target.x = mapCenterWorld.x;
            }

            if (halfSize.y < mapExtents.y)
            {
                target.y = Mathf.Clamp(target.y, minPos.y, maxPos.y);
            }
            else
            {
                target.y = mapCenterWorld.y;
            }
        }

        transform.position = target;
    }
}
