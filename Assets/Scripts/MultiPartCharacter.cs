using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MultiPartCharacter : MonoBehaviour
{
    [System.Serializable]
    public struct CharacterPart
    {
        public string ID;
        public Transform transform;
        public Vector3 offsetBase, offsetCrouch;
    }


    [Header("Inscribed")]
    public CharacterPart[] parts;
    public GameObject[] disableWhenCrouched;
    public float animationTime = 0.1f;

    [Header("Dynamic")]
    [SerializeField] bool _isCrouched;
    [SerializeField] float crouchDeltaT;
    public bool isCrouched
    {
        get
        {
            return _isCrouched;
        }
        set
        {
            _isCrouched = value;
            UpdateCrouched(value);
        }
    }

    void UpdateCrouched(bool crouched)
    {

        foreach (GameObject part in disableWhenCrouched)
        {
            part.SetActive(!isCrouched);
        }

        crouchDeltaT = Mathf.Max(0.0f, 1.0f - crouchDeltaT);
    }


    // Start is called before the first frame update
    void Start()
    {
        crouchDeltaT = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        crouchDeltaT += Time.deltaTime / animationTime;

        foreach (CharacterPart part in parts)
        {
            Vector3 start = isCrouched ? part.offsetBase : part.offsetCrouch;
            Vector3 end = isCrouched ? part.offsetCrouch : part.offsetBase;

            // part.transform.localPosition = isCrouched ? part.offsetCrouch : part.offsetBase;
            part.transform.localPosition = Vector3.Lerp(start, end, crouchDeltaT);       
        }
    }
}
