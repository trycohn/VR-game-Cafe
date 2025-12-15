using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;
using System.Collections.Generic;

public class ChairValidator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform[] targetPositions; // Массив целевых позиций
    [SerializeField] private float positionTolerance = 0.5f;
    [SerializeField] private float rotationTolerance = 500f;
    [SerializeField] private AudioClip placementSuccessSound;

    [Header("Ссылки")]
    public CafeManager cafeManager;

    [Header("Настройки восклицательного знака")]
    public GameObject IndicatorPrefab; // Префаб стрелки
    public float IndicatorShowDelay = 3f; // Через сколько секунд показать стрелку
    public float IndicatorHeightOffset = 0.3f; // Высота стрелки над грязью

    private GameObject arrowIndicator; // Объект стрелки
    private bool IndicatorSpawned = false;

    [Header("Chair Settings")]
    public int chairIndex = 0;

    private XRGrabInteractable grabInteractable;
    private bool isPlacedCorrectly = false;
    private Renderer chairRenderer;
    private Coroutine validationCoroutine;
    private Transform selectedTargetPosition; // Выбранная целевая позиция
    private List<Transform> availableTargets; // Доступные целевые позиции

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        chairRenderer = GetComponent<Renderer>();

        // Инициализируем список доступных целей
        availableTargets = new List<Transform>();
        if (targetPositions != null && targetPositions.Length > 0)
        {
            availableTargets.AddRange(targetPositions);
        }

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnChairReleased);
        }

        Invoke("ShowArrowIndicator", IndicatorShowDelay);
    }

    private void ShowArrowIndicator()
    {
        if (IndicatorSpawned) return;

        if (IndicatorPrefab != null)
        {
            // Создаем стрелку над ведром
            Vector3 arrowPosition = transform.position + Vector3.up * IndicatorHeightOffset;
            arrowIndicator = Instantiate(IndicatorPrefab, arrowPosition, Quaternion.Euler(0, 0, 0));

            // Устанавливаем родителя для корректного позиционирования
            arrowIndicator.transform.SetParent(transform);

            IndicatorSpawned = true;
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnChairReleased);
        }
        if (validationCoroutine != null)
        {
            StopCoroutine(validationCoroutine);
        }
    }

    void OnValidatePlacement()
    {
        if (isPlacedCorrectly || (grabInteractable != null && grabInteractable.isSelected)) return;

        // Проверяем все доступные целевые позиции
        foreach (var target in availableTargets)
        {
            if (target == null) continue;

            float positionDifference = Vector3.Distance(transform.position, target.position);
            float rotationDifference = Quaternion.Angle(transform.rotation, target.rotation);

            if (positionDifference <= positionTolerance && rotationDifference <= rotationTolerance)
            {
                selectedTargetPosition = target; // Запоминаем выбранную позицию
                PlaceCorrectly();
                return; // Выходим после нахождения подходящей позиции
            }
        }
    }

    void PlaceCorrectly()
    {
        if (isPlacedCorrectly) return;

        isPlacedCorrectly = true;

        // Удаляем выбранный объект targetPosition
        if (selectedTargetPosition != null)
        {

            if (arrowIndicator != null)
            {
                Destroy(arrowIndicator);
            }

            Debug.Log($"Удаляем объект маркера: {selectedTargetPosition.name}");
            Destroy(selectedTargetPosition.gameObject);

            // Удаляем из списка доступных целей
            availableTargets.Remove(selectedTargetPosition);
        }

        StartCoroutine(SmoothPlacement());

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        if (placementSuccessSound != null)
        {
            AudioSource.PlayClipAtPoint(placementSuccessSound, transform.position);
        }

        if (cafeManager != null)
        {
            cafeManager.OnChairPlaced(chairIndex);
        }

        Debug.Log($"Chair placed correctly: {gameObject.name}");
    }

    IEnumerator SmoothPlacement()
    {
        if (selectedTargetPosition == null) yield break;

        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 targetPos = selectedTargetPosition.position;
        Quaternion targetRot = selectedTargetPosition.rotation;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPos, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void OnChairReleased(SelectExitEventArgs args)
    {
        if (!isPlacedCorrectly)
        {
            if (validationCoroutine != null)
            {
                StopCoroutine(validationCoroutine);
            }
            validationCoroutine = StartCoroutine(ValidateAfterDelay(0.1f));
        }
    }

    IEnumerator ValidateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnValidatePlacement();
    }

    void Update()
    {
        if (!isPlacedCorrectly && (grabInteractable == null || !grabInteractable.isSelected))
        {
            OnValidatePlacement();
        }
    }

    public bool IsPlacedCorrectly() => isPlacedCorrectly;

    // Метод для добавления целевых позиций вручную (опционально)
    public void AddTargetPosition(Transform newTarget)
    {
        if (availableTargets == null)
            availableTargets = new List<Transform>();

        availableTargets.Add(newTarget);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (targetPositions != null)
        {
            Gizmos.color = Color.green;
            foreach (var target in targetPositions)
            {
                if (target != null)
                {
                    Gizmos.DrawWireCube(target.position, Vector3.one * 0.1f);
                    Gizmos.DrawLine(target.position, target.position + target.forward * 0.2f);
                }
            }
        }
    }
#endif
}