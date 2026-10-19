using UnityEngine;

public class Pipes : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public float speed = 5f;
    public float gap = 3f;

    [SerializeField] private Sprite openHandSprite;
    [SerializeField] private float extraOpening = 5f;
    [SerializeField] private float revealDistance = 7f;
    [SerializeField] private float reachDuration = 0.7f;

    private float leftEdge;
    private Player player;
    private SpriteRenderer topRenderer;
    private SpriteRenderer bottomRenderer;
    private BoxCollider2D topCollider;
    private BoxCollider2D bottomCollider;
    private Vector3 topStartingPosition;
    private Vector3 bottomStartingPosition;
    private Vector3 topReachedPosition;
    private Vector3 bottomReachedPosition;
    private float reachTimer;
    private bool reaching;
    private bool reached;
    private bool handRevealEnabled;

    private void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("Pipe obstacles require a camera tagged MainCamera.", this);
            enabled = false;
            return;
        }

        leftEdge = mainCamera.ScreenToWorldPoint(Vector3.zero).x - 1f;
        player = FindFirstObjectByType<Player>();
        topRenderer = top.GetComponent<SpriteRenderer>();
        bottomRenderer = bottom.GetComponent<SpriteRenderer>();
        topCollider = top.GetComponent<BoxCollider2D>();
        bottomCollider = bottom.GetComponent<BoxCollider2D>();

        if (player == null || topRenderer == null || bottomRenderer == null ||
            topRenderer.sprite == null || bottomRenderer.sprite == null) {
            Debug.LogError("Pipe obstacles require a Player and top/bottom sprites in the scene.", this);
            enabled = false;
            return;
        }

        if (openHandSprite == null) {
            Debug.LogError("Assign the open hand sprite to the Pipes component.", this);
            top.position += Vector3.up * gap / 2f;
            bottom.position += Vector3.down * gap / 2f;
            return;
        }

        if (extraOpening < 0f || reachDuration <= 0f) {
            Debug.LogError("Pipe hand reach settings must use a non-negative opening and positive duration.", this);
            enabled = false;
            return;
        }

        handRevealEnabled = true;
        float handHeight = topRenderer.sprite.bounds.size.y;
        SetColliderSize(topCollider, topRenderer.sprite);
        SetColliderSize(bottomCollider, bottomRenderer.sprite);
        float targetCenterOffset = (gap + handHeight) / 2f;
        float startingCenterOffset = (gap + extraOpening + handHeight) / 2f;

        topStartingPosition = new Vector3(top.localPosition.x, startingCenterOffset, top.localPosition.z);
        bottomStartingPosition = new Vector3(bottom.localPosition.x, -startingCenterOffset, bottom.localPosition.z);
        topReachedPosition = new Vector3(top.localPosition.x, targetCenterOffset, top.localPosition.z);
        bottomReachedPosition = new Vector3(bottom.localPosition.x, -targetCenterOffset, bottom.localPosition.z);

        top.localPosition = topStartingPosition;
        bottom.localPosition = bottomStartingPosition;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (handRevealEnabled && !reaching && !reached &&
            transform.position.x - player.transform.position.x <= revealDistance) {
            reaching = true;
        }

        if (reaching) {
            reachTimer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(reachTimer / reachDuration));
            top.localPosition = Vector3.Lerp(topStartingPosition, topReachedPosition, progress);
            bottom.localPosition = Vector3.Lerp(bottomStartingPosition, bottomReachedPosition, progress);

            if (progress >= 1f) {
                reaching = false;
                reached = true;
                topRenderer.sprite = openHandSprite;
                bottomRenderer.sprite = openHandSprite;
                SetColliderSize(topCollider, openHandSprite);
                SetColliderSize(bottomCollider, openHandSprite);
            }
        }

        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }

    private static void SetColliderSize(BoxCollider2D boxCollider, Sprite sprite)
    {
        if (boxCollider != null) {
            boxCollider.size = sprite.bounds.size;
        }
    }

}
