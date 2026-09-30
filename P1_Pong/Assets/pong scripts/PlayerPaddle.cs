using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPaddle : Paddle
{
    private float _direction;

    // =========================
    // CRANK SETTINGS
    // =========================

    public float maxCrankForce = 3f;
    public float crankChargeSpeed = 5f;

    public float crankReleaseWindow = 0.15f;

    public float crankPullback = 0.8f;
    public float crankPullbackSpeed = 4f;
    public float crankReturnSpeed = 12f;


    // =========================
    // CRANK VARIABLES
    // =========================

    private float currentCrankForce = 0f;

    private float releasedCrankForce = 0f;
    private float releaseTimer = 0f;

    private float normalXPosition;

    private bool isCranking = false;


    // =========================
    // START
    // =========================

    private void Start()
    {
        normalXPosition = _initialPosition.x;
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
        _direction = 0f;
        currentCrankForce = 0f;
        releasedCrankForce = 0f;
        releaseTimer = 0f;
        isCranking = false;
    }


    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        // =========================
        // CRANK
        // =========================

        if (Keyboard.current.spaceKey.isPressed)
        {
            isCranking = true;

            // Charge crank force
            currentCrankForce +=
                crankChargeSpeed * Time.deltaTime;

            currentCrankForce =
                Mathf.Clamp(
                    currentCrankForce,
                    0f,
                    maxCrankForce
                );

            // Disable W/S while cranking
            _direction = 0f;
        }
        else
        {
            // =========================
            // NORMAL MOVEMENT
            // =========================

            if (Keyboard.current.wKey.isPressed)
            {
                _direction = 1f;
            }
            else if (Keyboard.current.sKey.isPressed)
            {
                _direction = -1f;
            }
            else
            {
                _direction = 0f;
            }
        }


        // =========================
        // RELEASE SPACE
        // =========================

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            ReleaseCrank();
        }


        // =========================
        // RELEASE TIMER
        // =========================

        if (releaseTimer > 0f)
        {
            releaseTimer -= Time.deltaTime;

            if (releaseTimer <= 0f)
            {
                releasedCrankForce = 0f;
            }
        }
    }


    // =========================
    // RELEASE CRANK
    // =========================

    private void ReleaseCrank()
    {
        isCranking = false;

        releasedCrankForce =
            currentCrankForce;

        currentCrankForce = 0f;

        releaseTimer =
            crankReleaseWindow;

        Debug.Log(
            "CRANK RELEASED! Force = " +
            releasedCrankForce
        );
    }


    // =========================
    // FIXED UPDATE
    // =========================

    private void FixedUpdate()
    {
        Vector2 targetPosition =
            _rigidbody.position;


        // =========================
        // VERTICAL MOVEMENT
        // =========================

        if (!isCranking)
        {
            targetPosition.y +=
                _direction *
                speed *
                Time.fixedDeltaTime;
        }


        // =========================
        // CRANK MOVEMENT
        // =========================

        if (isCranking)
        {
            float targetX =
                normalXPosition -
                crankPullback;

            targetPosition.x =
                Mathf.MoveTowards(
                    _rigidbody.position.x,
                    targetX,
                    crankPullbackSpeed *
                    Time.fixedDeltaTime
                );
        }
        else
        {
            // Quickly return to normal position
            targetPosition.x =
                Mathf.MoveTowards(
                    _rigidbody.position.x,
                    normalXPosition,
                    crankReturnSpeed *
                    Time.fixedDeltaTime
                );
        }


        _rigidbody.MovePosition(targetPosition);
    }


    // =========================
    // CRANK FORCE FOR BALL
    // =========================

    public float GetReleasedCrankForce()
    {
        if (releaseTimer <= 0f)
        {
            return 0f;
        }

        float force =
            releasedCrankForce;

        // Consume the force
        releasedCrankForce = 0f;
        releaseTimer = 0f;

        return force;
    }
}

