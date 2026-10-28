using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform Head;
    public Camera cam;
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    public static float RestrictAngle(float angle,float min, float max)
    {
        if (angle > 180)
        {
            angle -= 360;
        }else if (angle < -180)
        {
            angle += 360;
        }

        if (angle < min)
        {
            angle = min;
        }
        else if (angle > max)
        {
            angle = max;
        }
        return angle;
    }

    // Start is called before the first frame update
    void Start()
    {
        LockCursor();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * 2f, 0);

        float horizontal = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        float vertical = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        movementInput = new Vector2(horizontal, vertical);
    }

    void FixedUpdate()
    {
        Vector3 movement = transform.right * movementInput.x + transform.forward * movementInput.y;
        movement = Vector3.ClampMagnitude(movement, 1f) * 10f;

        Vector3 velocity = rb.velocity;
        velocity.x = movement.x;
        velocity.z = movement.z;
        rb.velocity = velocity;
    }

    void LateUpdate()
    {
        float x = Input.GetAxisRaw("Mouse Y") * 2f;
        float y = Head.localEulerAngles.x - x;
        y = RestrictAngle(y, -90, 90);
        Head.localEulerAngles = new Vector3(y, 0, 0);
    }
}
