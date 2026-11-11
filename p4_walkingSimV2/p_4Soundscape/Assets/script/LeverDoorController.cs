using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeverDoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private AudioSource activationAudioSource;
    [SerializeField, Min(0.1f)] private float interactionDistance = 5f;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string pressTrigger = "Pressed";

    private bool activated;

    void Awake()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
        }

        if (activationAudioSource != null)
        {
            activationAudioSource.playOnAwake = false;
            activationAudioSource.loop = false;
        }
    }

    void Start()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("LeverDoorController requires a Door Animator reference.", this);
            enabled = false;
            return;
        }

        if (leverAnimator == null)
        {
            Debug.LogError("LeverDoorController requires a Lever Animator reference.", this);
            enabled = false;
            return;
        }

        if (interactionCamera == null)
        {
            Debug.LogError("LeverDoorController requires an interaction camera or a MainCamera-tagged camera.", this);
            enabled = false;
            return;
        }

        if (activationAudioSource == null || activationAudioSource.clip == null)
        {
            Debug.LogError("LeverDoorController requires an AudioSource with an activation sound AudioClip.", this);
            enabled = false;
            return;
        }

        if (!HasTrigger(doorAnimator, openTrigger))
        {
            Debug.LogError($"The Door Animator requires a Trigger parameter named '{openTrigger}'.", doorAnimator);
            enabled = false;
            return;
        }

        if (!HasTrigger(leverAnimator, pressTrigger))
        {
            Debug.LogError($"The Lever Animator requires a Trigger parameter named '{pressTrigger}'.", leverAnimator);
            enabled = false;
        }
    }

    private static bool HasTrigger(Animator animator, string triggerName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == triggerName && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        if (activated || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance) &&
            (hit.transform == transform || hit.transform.IsChildOf(transform)))
        {
            activated = true;
            leverAnimator.SetTrigger(pressTrigger);
            doorAnimator.SetTrigger(openTrigger);
            activationAudioSource.PlayOneShot(activationAudioSource.clip);
        }
    }
}
