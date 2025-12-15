using UnityEngine;

public class WaterBucket : MonoBehaviour
{
    [Header("Звук и эффекты")]
    public AudioClip splashSound;
    public ParticleSystem splashEffect;

    [Header("Настройки восклицательного знака")]
    public GameObject IndicatorPrefab; // Префаб стрелки
    public float IndicatorShowDelay = 3f; // Через сколько секунд показать стрелку
    public float IndicatorHeightOffset = 0.3f;

    [Header("Текущее состояние")]
    private GameObject arrowIndicator; // Объект стрелки
    private bool arrowSpawned = false;


    private void Start()
    {
        Invoke("ShowArrowIndicator", IndicatorShowDelay);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли объект тряпкой
        CleaningCloth cloth = other.GetComponent<CleaningCloth>();
        if (cloth != null)
        {
            cloth.SoakInBucket();

            // Визуальная и звуковая обратная связь
            if (splashSound != null)
                AudioSource.PlayClipAtPoint(splashSound, transform.position);

            if (splashEffect != null)
            {
                // Воспроизводим эффект на месте ведра
                var effect = Instantiate(splashEffect, transform.position, Quaternion.Euler(-90, 0, 0));
                Destroy(effect.gameObject, 2f); // Удаляем через 2 секунды
            }

            Debug.Log("Тряпка смочена в ведре");
        }
    }

    private void ShowArrowIndicator()
    {
        if (arrowSpawned) return;

        if (IndicatorPrefab != null)
        {
            // Создаем стрелку над ведром
            Vector3 arrowPosition = transform.position + Vector3.up * IndicatorHeightOffset;
            arrowIndicator = Instantiate(IndicatorPrefab, arrowPosition, Quaternion.Euler(0, 0, 0));

            // Устанавливаем родителя для корректного позиционирования
            arrowIndicator.transform.SetParent(transform);

            arrowSpawned = true;
        }
    }
}