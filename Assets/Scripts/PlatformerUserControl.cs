using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlatformerCharacter))]
public class PlatformerUserControl : MonoBehaviour
{
    [Header("Dynamic")]
    [SerializeField] PlatformerCharacter character;

    private void Awake()
    {
        character = GetComponent<PlatformerCharacter>();
    }

    // Use this for initialization
    void Start()
    {

    }

    private void OnDestroy()
    {
        GameManager.LevelLost();
    }

    // Update is called once per frame
    void Update()
    {
        character.AddWalkInput(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
        {
            character.Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            character.PickUp();
        }

        if (Input.GetButtonDown("Reset"))
        {
            character.Kill();
        }

        if (Input.GetAxis("Vertical") < 0.0f)
        {
            character.Crouch();
        }
        else
        {
            character.Stand();
        }
    }
}
