using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Vector2 _initialPosition;
    private GameManager _gameManager;
    private PlayerPaddle _playerPaddle;
    private ComputerPaddle _opponentPaddle;
    private Collider2D _collider;

    // =========================
    // BALL SETTINGS
    // =========================

    public float normalSpeed = 5f;
    public float middleSpeed = 1f;

    public float normalSize = 0.125f;
    public float middleSize = 0.25f;

    public float courtWidth = 9f;

    public float minVerticalAngle = 0.10f;
    public float maxVerticalAngle = 0.35f;


    // =========================
    // CRANK BOOST
    // =========================

    private float crankBoost = 0f;


    // =========================
    // SHADOW SETTINGS
    // =========================

    public Transform ballShadow;

    public float shadowNormalSize = 0.10f;
    public float shadowMiddleSize = 0.35f;

    public float shadowCloseDistance = 0.15f;
    public float shadowFarDistance = 1.5f;

    public float shadowNormalOpacity = 1f;
    public float shadowMiddleOpacity = 0.4f;


    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        _rigidbody =
            GetComponent<Rigidbody2D>();

        _collider =
            GetComponent<Collider2D>();

        _initialPosition =
            _rigidbody.position;

        _gameManager =
            FindFirstObjectByType<GameManager>();

        _playerPaddle =
            FindFirstObjectByType<PlayerPaddle>();

        _opponentPaddle =
            FindFirstObjectByType<ComputerPaddle>();
    }


    // =========================
    // START
    // =========================

    private void Start()
    {
        _rigidbody.gravityScale = 0f;
        _rigidbody.linearDamping = 0f;

        ResetBall();
    }

    public void ResetBall()
    {
        _rigidbody.position = _initialPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        transform.localScale = Vector3.one * normalSize;
        crankBoost = 0f;

        float directionX = Random.value < 0.5f ? -1f : 1f;
        float directionY = Random.Range(-maxVerticalAngle, maxVerticalAngle);
        Vector2 direction = new Vector2(directionX, directionY).normalized;

        _rigidbody.linearVelocity = direction * normalSpeed;
    }


    // =========================
    // FIXED UPDATE
    // =========================

    private void FixedUpdate()
    {
        if (_gameManager == null)
        {
            _gameManager = FindFirstObjectByType<GameManager>();
        }

        float halfCourtWidth = courtWidth / 2f;
        float ballRadius = _collider != null ? _collider.bounds.extents.x : 0f;
        float leftScoreBoundary = -halfCourtWidth;
        float rightScoreBoundary = halfCourtWidth;

        if (_playerPaddle == null)
        {
            _playerPaddle = FindFirstObjectByType<PlayerPaddle>();
        }

        if (_opponentPaddle == null)
        {
            _opponentPaddle = FindFirstObjectByType<ComputerPaddle>();
        }

        if (_playerPaddle != null)
        {
            Collider2D paddleCollider = _playerPaddle.GetComponent<Collider2D>();
            if (paddleCollider != null)
            {
                leftScoreBoundary = Mathf.Min(
                    leftScoreBoundary,
                    paddleCollider.bounds.min.x - ballRadius
                );
            }
        }

        if (_opponentPaddle != null)
        {
            Collider2D paddleCollider = _opponentPaddle.GetComponent<Collider2D>();
            if (paddleCollider != null)
            {
                rightScoreBoundary = Mathf.Max(
                    rightScoreBoundary,
                    paddleCollider.bounds.max.x + ballRadius
                );
            }
        }

        if (transform.position.x < leftScoreBoundary)
        {
            if (_gameManager != null)
            {
                _gameManager.OpponentScores();
            }
            else
            {
                ResetBall();
                Debug.LogError("Ball left the court, but no GameManager is in the scene.");
            }

            return;
        }

        if (transform.position.x > rightScoreBoundary)
        {
            if (_gameManager != null)
            {
                _gameManager.PlayerScores();
            }
            else
            {
                ResetBall();
                Debug.LogError("Ball left the court, but no GameManager is in the scene.");
            }

            return;
        }

        float x =
            transform.position.x;

        float quarter =
            courtWidth / 4f;

        float middleStart =
            -quarter;

        float middleEnd =
            quarter;

        float currentSize;
        float currentSpeed;
        float middleAmount;


        // =========================
        // BALL SIZE + SPEED
        // =========================

        if (x >= middleStart &&
            x <= middleEnd)
        {
            middleAmount =
                1f -
                Mathf.Abs(x) /
                quarter;

            middleAmount =
                Mathf.Clamp01(
                    middleAmount
                );

            currentSize =
                Mathf.Lerp(
                    normalSize,
                    middleSize,
                    middleAmount
                );

            currentSpeed =
                Mathf.Lerp(
                    normalSpeed,
                    middleSpeed,
                    middleAmount
                );
        }
        else
        {
            middleAmount = 0f;

            currentSize =
                normalSize;

            currentSpeed =
                normalSpeed;
        }


        // =========================
        // BALL SIZE
        // =========================

        transform.localScale =
            Vector3.one *
            currentSize;


        // =========================
        // APPLY CRANK BOOST
        // =========================

        float targetSpeed =
            currentSpeed +
            crankBoost;


        if (_rigidbody.linearVelocity.sqrMagnitude >
            0.01f)
        {
            Vector2 direction =
                _rigidbody.linearVelocity.normalized;

            _rigidbody.linearVelocity =
                direction *
                targetSpeed;
        }


        // =========================
        // SHADOW
        // =========================

        if (ballShadow != null)
        {
            float shadowSize =
                Mathf.Lerp(
                    shadowNormalSize,
                    shadowMiddleSize,
                    middleAmount
                );

            ballShadow.localScale =
                Vector3.one *
                shadowSize;


            float shadowDistance =
                Mathf.Lerp(
                    shadowCloseDistance,
                    shadowFarDistance,
                    middleAmount
                );

            ballShadow.position =
                transform.position +
                Vector3.down *
                shadowDistance;


            float shadowOpacity =
                Mathf.Lerp(
                    shadowNormalOpacity,
                    shadowMiddleOpacity,
                    middleAmount
                );


            SpriteRenderer shadowRenderer =
                ballShadow.GetComponent<SpriteRenderer>();

            if (shadowRenderer != null)
            {
                Color shadowColor =
                    shadowRenderer.color;

                shadowColor.a =
                    shadowOpacity;

                shadowRenderer.color =
                    shadowColor;
            }
        }
    }


    // =========================
    // COLLISIONS
    // =========================

    private void OnCollisionEnter2D(
        Collision2D collision)
    {
        Paddle paddle =
            collision.gameObject
            .GetComponent<Paddle>();


        // =========================
        // PADDLE COLLISION
        // =========================

        if (paddle != null)
        {
            // Determine which side the ball should go
            float xDirection =
                transform.position.x <
                paddle.transform.position.x
                ? -1f
                : 1f;


            // =========================
            // HIT POSITION
            // =========================

            float paddleHeight =
                paddle.GetComponent<Collider2D>()
                .bounds.size.y;

            float hitPosition =
                (
                    transform.position.y -
                    paddle.transform.position.y
                )
                /
                (paddleHeight / 2f);

            hitPosition =
                Mathf.Clamp(
                    hitPosition,
                    -1f,
                    1f
                );


            // =========================
            // VERTICAL DIRECTION
            // =========================

            float yDirection;

            if (Mathf.Abs(hitPosition) < 0.01f)
            {
                yDirection =
                    Random.value < 0.5f
                    ? minVerticalAngle
                    : -minVerticalAngle;
            }
            else
            {
                yDirection =
                    Mathf.Sign(hitPosition) *
                    Mathf.Lerp(
                        minVerticalAngle,
                        maxVerticalAngle,
                        Mathf.Abs(hitPosition)
                    );
            }


            Vector2 newDirection =
                new Vector2(
                    xDirection,
                    yDirection
                ).normalized;


            // =========================
            // CHECK FOR PLAYER CRANK
            // =========================

            PlayerPaddle playerPaddle =
                collision.gameObject
                .GetComponent<PlayerPaddle>();


            if (playerPaddle != null)
            {
                float crankForce =
                    playerPaddle
                    .GetReleasedCrankForce();

                // Add the new crank boost
                crankBoost =
                    crankForce;
            }
            else
            {
                // Opponent paddle removes
                // the previous crank boost
                crankBoost = 0f;
            }


            // =========================
            // NEW BALL VELOCITY
            // =========================

            float targetSpeed =
                normalSpeed +
                crankBoost;

            _rigidbody.linearVelocity =
                newDirection *
                targetSpeed;

            return;
        }

    }
}