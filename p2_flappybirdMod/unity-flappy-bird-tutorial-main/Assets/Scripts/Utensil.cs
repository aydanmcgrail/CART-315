using UnityEngine;

public class Utensil : MonoBehaviour
{
    private Camera mainCamera;
    private float speed;
    private float baseHeight;
    private float jumpDelay;
    private float jumpHeight;
    private float jumpDuration;
    private float elapsed;
    private float leftEdge;
    private bool willJump;

    public void Initialize(
        Camera camera,
        float movementSpeed,
        float restingHeight,
        bool jumps,
        float delay,
        float height,
        float duration)
    {
        mainCamera = camera;
        speed = movementSpeed;
        baseHeight = restingHeight;
        willJump = jumps;
        jumpDelay = delay;
        jumpHeight = height;
        jumpDuration = duration;
    }

    private void Start()
    {
        leftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero).x - 1f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        Vector3 position = transform.position;
        position.x -= speed * Time.deltaTime;

        if (willJump && elapsed >= jumpDelay) {
            float progress = Mathf.Clamp01((elapsed - jumpDelay) / jumpDuration);
            position.y = baseHeight + Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            if (progress >= 1f) {
                willJump = false;
            }
        }

        transform.position = position;

        if (position.x < leftEdge) {
            Destroy(gameObject);
        }
    }
}
