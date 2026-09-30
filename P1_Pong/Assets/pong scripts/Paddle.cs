using UnityEngine;

public class Paddle : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    protected Vector2 _initialPosition;

    public float speed = 5f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.gravityScale = 0f;
        _initialPosition = _rigidbody.position;
    }

    public virtual void ResetPosition()
    {
        _rigidbody.position = _initialPosition;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }
}
