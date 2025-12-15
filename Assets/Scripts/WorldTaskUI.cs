using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WorldTaskUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI taskTitleText;
    public TextMeshProUGUI[] taskItems;
    [Header("Initial Task Texts")]
    [TextArea] public string[] initialTaskTexts = new string[4];

    [Header("Кнопка запуска NPC")]
    public GameObject startNPCButton; // Кнопка, которая появляется после выполнения всех заданий
    public Button npcButtonComponent; // Компонент Button для кнопки
    public GameObject uiParentToHide; // Родительский объект UI, который нужно скрыть

    [Header("References")]
    public CafeManager cafeManager;

    private bool allTasksCompleted = false;

    void Start()
    {
        InitializeUI();
        SetupEventListeners();

        // Скрываем кнопку запуска NPC в начале
        if (startNPCButton != null)
        {
            startNPCButton.SetActive(false);
        }

        // Настраиваем обработчик клика на кнопке
        if (npcButtonComponent != null)
        {
            npcButtonComponent.onClick.AddListener(OnNPCButtonClicked);
        }
        else if (startNPCButton != null)
        {
            // Если компонент Button не назначен, попробуем найти его
            Button button = startNPCButton.GetComponent<Button>();
            if (button != null)
            {
                npcButtonComponent = button;
                npcButtonComponent.onClick.AddListener(OnNPCButtonClicked);
            }
        }
    }

    private void InitializeUI()
    {
        if (taskTitleText != null)
            taskTitleText.text = "Что осталось сделать:";

        if (taskItems != null && taskItems.Length >= 4)
        {
            for (int i = 0; i < taskItems.Length && i < initialTaskTexts.Length; i++)
            {
                if (taskItems[i] != null && !string.IsNullOrEmpty(initialTaskTexts[i]))
                {
                    taskItems[i].text = initialTaskTexts[i];
                }
            }
        }
    }

    private void SetupEventListeners()
    {
        if (cafeManager != null)
        {
            cafeManager.OnAllTasksCompleted.AddListener(OnAllTasksCompleted);
        }
    }

    void Update()
    {
        // Обновляем только если все задачи ещё не выполнены
        if (!allTasksCompleted)
        {
            UpdateTaskDisplay();
        }
    }

    private void UpdateTaskDisplay()
    {
        if (cafeManager == null || taskItems == null) return;

        // Задача 1: Взять тряпку
        if (taskItems.Length > 0 && taskItems[0] != null)
        {
            string status = cafeManager.isTask1Complete ? "✅" : "☐";
            taskItems[0].text = $"{status} Взять тряпку";
            taskItems[0].color = cafeManager.isTask1Complete ?
                new Color(0.4f, 0.8f, 0.4f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1f);
        }

        // Задача 2: Протереть столы
        if (taskItems.Length > 1 && taskItems[1] != null)
        {
            string status = cafeManager.isTask2Complete ? "☑" : "☐";
            int cleaned = cafeManager.GetCleanedTableSurfaces();
            int total = cafeManager.GetTotalTableSurfaces();
            taskItems[1].text = $"{status} Протереть столы ({cleaned}/{total})";
            taskItems[1].color = cafeManager.isTask2Complete ?
                new Color(0.4f, 0.8f, 0.4f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1f);
        }

        // Задача 3: Расставить стулья
        if (taskItems.Length > 2 && taskItems[2] != null)
        {
            string status = cafeManager.GetTask3Complete() ? "☑" : "☐";
            int placed = cafeManager.GetPlacedChairs();
            int total = cafeManager.GetTotalChairs();
            taskItems[2].text = $"{status} Расставить стулья ({placed}/{total})";
            taskItems[2].color = cafeManager.GetTask3Complete() ?
                new Color(0.4f, 0.8f, 0.4f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1f);
        }

        // Задача 4: Проверить приборы
        if (taskItems.Length > 3 && taskItems[3] != null)
        {
            string status = (cafeManager.dirtyUtensilsCount <= 0) ? "☑" : "☐";
            taskItems[3].text = $"{status} Проверить приборы ({cafeManager.dirtyUtensilsCount} грязных)";
            taskItems[3].color = (cafeManager.dirtyUtensilsCount <= 0) ?
                new Color(0.4f, 0.8f, 0.4f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1f);
        }
    }

    // Метод вызывается при завершении всех задач
    public void OnAllTasksCompleted()
    {
        allTasksCompleted = true;

        Debug.Log("Все задачи выполнены! Показываем кнопку запуска NPC...");

        // Скрываем текущие задачи
        if (taskTitleText != null)
        {
            taskTitleText.gameObject.SetActive(false);
        }

        if (taskItems != null)
        {
            foreach (var item in taskItems)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        // Показываем кнопку запуска NPC
        if (startNPCButton != null)
        {
            startNPCButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Кнопка запуска NPC не назначена в WorldTaskUI!");
        }
    }

    // Обработчик клика на кнопке NPC
    private void OnNPCButtonClicked()
    {
        Debug.Log("Кнопка запуска NPC нажата!");

        // Скрываем UI
        StartCoroutine(HideUIWithDelay());
    }

    // Прячем UI с задержкой для плавности
    private IEnumerator HideUIWithDelay()
    {
        // Можно добавить анимацию исчезновения
        if (uiParentToHide != null)
        {
            CanvasGroup canvasGroup = uiParentToHide.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // Плавное исчезновение
                float duration = 0.5f;
                float elapsed = 0f;
                float startAlpha = canvasGroup.alpha;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                    yield return null;
                }
            }

            // Отключаем UI
            uiParentToHide.SetActive(false);
            Debug.Log($"UI объект {uiParentToHide.name} скрыт.");
        }
        else
        {
            // Если родительский объект не назначен, скрываем весь Canvas или этот объект
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
                Debug.Log($"Canvas {canvas.name} скрыт.");
            }
            else
            {
                gameObject.SetActive(false);
                Debug.Log($"Объект {gameObject.name} скрыт.");
            }
        }
    }

    // Метод для ручного обновления (если нужно)
    public void ForceUpdate()
    {
        if (!allTasksCompleted)
        {
            UpdateTaskDisplay();
        }
    }

    // Очистка при уничтожении объекта
    private void OnDestroy()
    {
        if (npcButtonComponent != null)
        {
            npcButtonComponent.onClick.RemoveListener(OnNPCButtonClicked);
        }
    }
}