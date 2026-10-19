using UnityEngine;

public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    [SerializeField] private float firstFlapMultiplier = 0.35f;
    [SerializeField] private float flapMultiplierIncrease = 0.4f;
    [SerializeField] private float maximumFlapMultiplier = 2f;
    [SerializeField] private float flapComboWindow = 0.55f;
    [SerializeField] private float retreatSpeed = 3f;
    [SerializeField] private float diveAcceleration = 16f;

    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private Transform bottomTriggerTransform;
    private BoxCollider2D bottomTriggerCollider;
    private Vector3 direction;
    private Vector3 startingPosition;
    private int spriteIndex;
    private int flapStreak;
    private float lastFlapTime = float.NegativeInfinity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        startingPosition = transform.position;

        if (mainCamera == null) {
            Debug.LogError("Player requires a camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        CreateBottomTrigger();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(AnimateSprite));
    }

    private void OnEnable()
    {
        Vector3 position = startingPosition;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
        flapStreak = 0;
        lastFlapTime = float.NegativeInfinity;
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetMouseButtonDown(0)) {
            Flap();
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            direction.y -= diveAcceleration * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A)) {
            float leftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero).x;
            float playerHalfWidth = playerCollider != null ? playerCollider.bounds.extents.x : 0f;
            float minimumX = leftEdge + playerHalfWidth;
            float newX = Mathf.Max(transform.position.x - retreatSpeed * Time.deltaTime, minimumX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        // Apply gravity and update the position
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Tilt the bird based on the direction
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void LateUpdate()
    {
        if (bottomTriggerTransform != null) {
            float cameraDepth = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
            Vector3 cameraBottom = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, cameraDepth));
            float triggerHeight = 0.2f;

            bottomTriggerTransform.position = new Vector3(
                cameraBottom.x,
                cameraBottom.y - triggerHeight / 2f,
                transform.position.z);
            bottomTriggerCollider.size = new Vector2(
                mainCamera.orthographicSize * 2f * mainCamera.aspect + 1f,
                triggerHeight);
        }
    }

    private void CreateBottomTrigger()
    {
        GameObject bottomTrigger = new GameObject("Bottom of Scene Trigger");
        bottomTrigger.tag = "Obstacle";
        bottomTrigger.layer = gameObject.layer;
        bottomTriggerTransform = bottomTrigger.transform;

        bottomTriggerCollider = bottomTrigger.AddComponent<BoxCollider2D>();
        bottomTriggerCollider.isTrigger = true;
    }

    private void OnDestroy()
    {
        if (bottomTriggerTransform != null) {
            Destroy(bottomTriggerTransform.gameObject);
        }
    }

    private void Flap()
    {
        if (Time.time - lastFlapTime <= flapComboWindow) {
            flapStreak++;
        } else {
            flapStreak = 1;
        }

        lastFlapTime = Time.time;

        float multiplier = Mathf.Min(
            firstFlapMultiplier + (flapStreak - 1) * flapMultiplierIncrease,
            maximumFlapMultiplier);
        direction.y = Mathf.Max(direction.y, strength * multiplier);
    }

    private void AnimateSprite()
    {
        if (sprites == null || sprites.Length == 0 || spriteRenderer == null) {
            return;
        }

        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            GameManager.Instance.IncreaseScore();
        }
    }

}
