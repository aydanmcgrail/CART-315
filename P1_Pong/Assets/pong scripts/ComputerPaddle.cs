using UnityEngine;

public class ComputerPaddle : Paddle
{
    public Transform ball;

    public float deadZone = 0.2f;
    public float acceleration = 20f;

    private void FixedUpdate()
    {
        if (ball == null)
            return;

        float difference = ball.position.y - transform.position.y;

        float targetVelocity = 0f;

        if (difference > deadZone)
        {
            targetVelocity = speed;
        }
        else if (difference < -deadZone)
        {
            targetVelocity = -speed;
        }

        float currentVelocity = _rigidbody.linearVelocity.y;

        float newVelocity = Mathf.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        _rigidbody.linearVelocity =
            new Vector2(0f, newVelocity);
    }
}


/*using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerPaddle : Paddle
{
    private Vector2 _direction;

    private void Update()

    {
        if (Keyboard.current.oKey.isPressed)
        {
            _direction = Vector2.up;
        }
        else if (Keyboard.current.lKey.isPressed)
        {
            _direction = Vector2.down;
        }
        else
        {
            _direction = Vector2.zero;
        }
    }
    private void FixedUpdate()
    {
        if (_direction.sqrMagnitude != 0)
        {
            _rigidbody.AddForce(_direction * this.speed);
        }
    }
}*/

