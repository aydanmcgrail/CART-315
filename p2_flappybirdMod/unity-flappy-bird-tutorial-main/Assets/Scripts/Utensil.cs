using UnityEngine;

public class Utensil : MonoBehaviour
{
    private Camera mainCamera;
    private Transform playerTransform;
    private float speed;
    private float baseHeight;
    private float jumpDelay;
    private float jumpHeight;
    private float jumpDuration;
    private float passedBirdJumpChance;
    private float elapsed;
    private float leftEdge;
    private bool willJump;
    private bool hasPassedBird;
    private bool hasJumped;

    public void Initialize(
        Camera camera,
        Transform player,
        float movementSpeed,
        float restingHeight,
        bool jumps,
        float jumpChanceAfterPassing,
        float delay,
        float height,
        float duration)
    {
        mainCamera = camera;
        playerTransform = player;
        speed = movementSpeed;
        baseHeight = restingHeight;
        willJump = jumps;
        passedBirdJumpChance = jumpChanceAfterPassing;
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

        if (!hasPassedBird && position.x <= playerTransform.position.x) {
            hasPassedBird = true;

            if (!willJump && !hasJumped && Random.value < passedBirdJumpChance) {
                willJump = true;
                jumpDelay = elapsed;
            }
        }

        if (willJump && elapsed >= jumpDelay) {
            float progress = Mathf.Clamp01((elapsed - jumpDelay) / jumpDuration);
            position.y = baseHeight + Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            if (progress >= 1f) {
                willJump = false;
                hasJumped = true;
            }
        }

        transform.position = position;

        if (position.x < leftEdge) {
            Destroy(gameObject);
        }
    }
}
