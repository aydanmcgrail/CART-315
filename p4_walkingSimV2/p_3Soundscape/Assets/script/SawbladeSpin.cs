using UnityEngine;

public class SawbladeSpin : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 2600f;

    void Update()
    {
        transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime, Space.Self);
    }
}
