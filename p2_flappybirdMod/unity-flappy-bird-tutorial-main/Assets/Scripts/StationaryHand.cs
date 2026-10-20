using UnityEngine;

public class StationaryHand : MonoBehaviour
{
    [SerializeField] private Sprite openHandSprite;
    [SerializeField] private Sprite closedHandSprite;
    [SerializeField] private float horizontalInset = 0.2f;
    [SerializeField] private float horizontalRange = 1.5f;
    [SerializeField] private float centerY;
    [SerializeField] private float verticalRange = 1.5f;
    [SerializeField] private float verticalCycleDuration = 5f;
    [SerializeField] private float spriteChangeInterval = 2f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D handCollider;
    private Camera mainCamera;
    private Vector3 startingPosition;
    private float elapsed;
    private float spriteChangeElapsed;
    private bool isOpen = true;

    private void Awake()
    {
        if (openHandSprite == null || closedHandSprite == null) {
            Debug.LogError("Assign open and closed hand sprites to the StationaryHand component.", this);
            enabled = false;
            return;
        }

        if (horizontalRange < 0f || verticalRange < 0f ||
            verticalCycleDuration <= 0f || spriteChangeInterval <= 0f) {
            Debug.LogError(
                "StationaryHand movement ranges must be non-negative and intervals must be positive.",
                this);
            enabled = false;
            return;
        }

        mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("StationaryHand requires a camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        handCollider = GetComponent<BoxCollider2D>();
        if (handCollider == null) {
            handCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        spriteRenderer.sprite = openHandSprite;
        spriteRenderer.sortingOrder = 3;
        handCollider.isTrigger = true;
        handCollider.size = openHandSprite.bounds.size;

        float cameraDepth = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, cameraDepth));
        startingPosition = new Vector3(leftEdge.x + horizontalInset, centerY, transform.position.z);
        transform.position = startingPosition;
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;
        spriteChangeElapsed += Time.unscaledDeltaTime;

        Vector3 position = transform.position;
        float cyclePosition = elapsed * Mathf.PI * 2f / verticalCycleDuration;
        position.x = startingPosition.x + Mathf.Sin(cyclePosition) * horizontalRange;
        position.y = startingPosition.y + Mathf.Sin(cyclePosition) * verticalRange;
        transform.position = position;

        while (spriteChangeElapsed >= spriteChangeInterval) {
            spriteChangeElapsed -= spriteChangeInterval;
            isOpen = !isOpen;
            spriteRenderer.sprite = isOpen ? openHandSprite : closedHandSprite;
            handCollider.size = spriteRenderer.sprite.bounds.size;
        }
    }
}
