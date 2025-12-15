using Unity.Burst.CompilerServices;
using UnityEngine;

public class DirtArea : MonoBehaviour
{
    [Header("Настройки")]
    public int wipesNeeded = 5; // Сколько нужно протираний
    public Material dirtyMaterial;
    public Material cleanMaterial;
    public ParticleSystem cleanParticles;

    [Header("Настройки восклицательного знака")]
    public GameObject IndicatorPrefab; // Префаб стрелки
    public float IndicatorShowDelay = 3f; // Через сколько секунд показать стрелку
    public float IndicatorHeightOffset = 0.3f; // Высота стрелки над грязью

    [Header("Ссылки")]
    public CafeManager cafeManager; // Ссылка на менеджер уровня
    public TableSurface parentTableSurface; // Ссылка на родительскую поверхность стола

    [Header("Настройки подсказки")]
    [SerializeField] private GameObject hintUI; // Перетащите сюда ваш UI GameObject
    [SerializeField] private float hintShowTime = 3f;

    private float hintTimer = 0f;
    private bool hintIsActive = false;

    [Header("Текущее состояние")]
    private bool isCleaned = false;
    private MeshRenderer meshRenderer;
    private int initialWipesNeeded; // Сохраняем начальное значение для расчета прогресса
    private GameObject arrowIndicator; // Объект стрелки
    private bool arrowSpawned = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialWipesNeeded = wipesNeeded; // Сохраняем начальное значение

        if (meshRenderer != null)
        {
            meshRenderer.material = dirtyMaterial;
        }

        // Запускаем таймер для показа стрелки
        Invoke("ShowArrowIndicator", IndicatorShowDelay);
    }

    void Update()
    {
        // Таймер для автоматического скрытия подсказки
        if (hintIsActive)
        {
            hintTimer -= Time.deltaTime;

            if (hintTimer <= 0f)
            {
                HideHint();
            }
        }
    }

    // Метод для показа стрелки над грязной областью
    private void ShowArrowIndicator()
    {
        if (arrowSpawned || isCleaned) return;

        if (IndicatorPrefab != null)
        {
            // Создаем стрелку над грязной областью
            Vector3 arrowPosition = transform.position + Vector3.up * IndicatorHeightOffset;
            arrowIndicator = Instantiate(IndicatorPrefab, arrowPosition, Quaternion.Euler(0, 0, 0));

            // Устанавливаем родителя для корректного позиционирования
            arrowIndicator.transform.SetParent(transform);

            arrowSpawned = true;

            Debug.Log("Стрелка показана над грязной областью");
        }
        else
        {
            Debug.LogWarning("Arrow indicator prefab is not assigned on " + gameObject.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCleaned) return;

        CleaningCloth cloth = other.GetComponent<CleaningCloth>();
        if (cloth != null)
        {
            // Проверяем, мокрая ли тряпка и готова ли она к использованию
            if (cloth.CanBeUsedForCleaning())
            {
                WipeDirt(cloth);
            }
            else
            {
                // Тряпка сухая - подсказка
                ShowHint();
            }
        }
    }

    void ShowHint()
    {
        // Активируем UI объект
        if (hintUI != null)
        {
            hintUI.SetActive(true);
            hintTimer = hintShowTime;
            hintIsActive = true;
        }
    }

    void HideHint()
    {
        // Деактивируем UI объект
        if (hintUI != null)
        {
            hintUI.SetActive(false);
        }
        hintIsActive = false;
        hintTimer = 0f;
    }

    void WipeDirt(CleaningCloth cloth)
    {
        // Используем тряпку - это уменьшит её прочность
        bool clothUsedSuccessfully = cloth.UseForCleaning();

        if (!clothUsedSuccessfully)
            return; // Если тряпка не может быть использована, выходим

        wipesNeeded--; // Уменьшаем счетчик грязи

        // Визуальная обратная связь - грязь становится светлее
        float progress = 1f - ((float)wipesNeeded / initialWipesNeeded);
        Color currentColor = Color.Lerp(
            dirtyMaterial.color,
            cleanMaterial.color,
            progress);

        if (meshRenderer != null)
            meshRenderer.material.color = currentColor;

        Debug.Log($"Осталось протираний для этой области: {wipesNeeded}");

        // Проверяем, очищена ли зона
        if (wipesNeeded <= 0)
        {
            CompleteCleaning();
        }
    }

    void CompleteCleaning()
    {
        isCleaned = true;

        // Удаляем стрелку при очистке
        if (arrowIndicator != null)
        {
            Destroy(arrowIndicator);
        }

        // Сообщаем родительской поверхности, что одна область очищена
        if (parentTableSurface != null)
        {
            parentTableSurface.OnDirtAreaCleaned();
        }

        // Финальные эффекты перед удалением
        if (cleanParticles != null)
        {
            ParticleSystem particles = Instantiate(cleanParticles,
                transform.position, Quaternion.identity);
            Destroy(particles.gameObject, 2f);
        }

        // Удаляем объект через 0.1 секунды (чтобы эффекты успели проиграться)
        Invoke("DestroyDirtArea", 0.1f);
    }

    void DestroyDirtArea()
    {
        Destroy(gameObject);
        Debug.Log("Грязная зона очищена и удалена!");
    }

    // Метод для проверки состояния (может потребоваться извне)
    public bool GetIsCleaned()
    {
        return isCleaned;
    }

    // Метод для ручного удаления стрелки (если нужно извне)
    public void RemoveArrow()
    {
        if (arrowIndicator != null)
        {
            Destroy(arrowIndicator);
            arrowSpawned = false;
        }
    }

    // Метод для принудительного показа стрелки
    public void ForceShowArrow()
    {
        if (!arrowSpawned && !isCleaned)
        {
            ShowArrowIndicator();
        }
    }
}