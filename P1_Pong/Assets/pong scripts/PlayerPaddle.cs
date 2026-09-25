using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPaddle : Paddle
{
    private Vector2 _direction;

    private void Update()

    {
        if (Keyboard.current.wKey.isPressed)
        {
            _direction = Vector2.up;
        }
        else if (Keyboard.current.sKey.isPressed)
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
}
