using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeverDoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Camera interactionCamera;
    [SerializeField, Min(0.1f)] private float interactionDistance = 5f;
    [SerializeField] private string openTrigger = "Open";

    private bool activated;

    void Awake()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
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

        if (interactionCamera == null)
        {
            Debug.LogError("LeverDoorController requires an interaction camera or a MainCamera-tagged camera.", this);
            enabled = false;
            return;
        }

        bool hasOpenTrigger = false;
        foreach (AnimatorControllerParameter parameter in doorAnimator.parameters)
        {
            if (parameter.name == openTrigger && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                hasOpenTrigger = true;
                break;
            }
        }

        if (!hasOpenTrigger)
        {
            Debug.LogError($"The Door Animator requires a Trigger parameter named '{openTrigger}'.", doorAnimator);
            enabled = false;
        }
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
            doorAnimator.SetTrigger(openTrigger);
        }
    }
}
