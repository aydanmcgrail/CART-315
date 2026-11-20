using UnityEngine;

public class SawbladeSpin : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 2600f;
    private float speedMultiplier = 1f;

    void Update()
    {
        transform.Rotate(0f, 0f, degreesPerSecond * speedMultiplier * Time.deltaTime, Space.Self);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp01(multiplier);
    }
}
