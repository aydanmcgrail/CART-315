using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip footstepClip;
    [SerializeField, Min(0.05f)] private float stepInterval = 0.45f;
    [SerializeField, Min(0f)] private float movementThreshold = 0.1f;

    private AudioSource audioSource;
    private PlayerMovement playerMovement;
    private float nextStepTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Start()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerFootsteps requires a parent PlayerMovement component.", this);
            enabled = false;
            return;
        }

        if (footstepClip == null)
        {
            Debug.LogError("PlayerFootsteps requires a footstep AudioClip.", this);
        }
    }

    void Update()
    {
        if (footstepClip == null ||
            playerMovement.MovementInput.sqrMagnitude < movementThreshold * movementThreshold)
        {
            nextStepTime = Time.time;
            return;
        }

        if (Time.time >= nextStepTime)
        {
            audioSource.PlayOneShot(footstepClip);
            nextStepTime = Time.time + stepInterval;
        }
    }
}
