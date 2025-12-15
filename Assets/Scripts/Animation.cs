using UnityEngine;

public class PulsingArrow : MonoBehaviour
{
    [Header("Animation Settings")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Пульсация стрелки
        float scale = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount + 1f;
        transform.localScale = originalScale * scale;
    }
}