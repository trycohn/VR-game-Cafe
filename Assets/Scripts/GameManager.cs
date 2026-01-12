using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalTrash;           // Общее количество мусора
    public int totalDishes;          // Количество грязной посуды
    private int collectedTrash = 0;  // Сколько собрано мусора (кроме посуды)
    private int collectedDishes = 0; // Сколько собрано посуды

    public TMP_Text uiTrashCounterText;  // Текст для оставшегося мусора
    public TMP_Text uiDishCounterText;   // Текст для оставшейся посуды
    public GameObject taskUI;            // Ссылка на UI, который нужно закрыть при завершении

    public AudioClip audioSource;
    public ParticleSystem completionEffect;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Подсчитываем общее количество мусора и посуды
        var allTrash = GameObject.FindGameObjectsWithTag("Trash");
        totalTrash = 0;
        totalDishes = 0;

        foreach (var trashObj in allTrash)
        {
            var trash = trashObj.GetComponent<TrashItem>();
            if (trash != null)
            {
                if (trash.type == TrashType.Dish)
                    totalDishes++;
                else
                    totalTrash++;
            }
        }

        UpdateUI();

        // Проверяем наличие ссылки на UI
        if (taskUI == null)
        {
            Debug.LogWarning("taskUI не назначен в GameManager!");
        }
    }

    public void CollectTrash(TrashItem item)
    {
        if (item.type == TrashType.Dish)
        {
            collectedDishes++;
        }
        else
        {
            collectedTrash++;
        }

        Destroy(item.gameObject);
        UpdateUI();

        if (collectedTrash >= totalTrash && collectedDishes >= totalDishes)
        {
            TaskCompleted();
        }
    }

    void UpdateUI()
    {
        if (uiTrashCounterText != null)
        {
            uiTrashCounterText.text = $"Осталось мусора: {totalTrash - collectedTrash}";
        }
        else
        {
            Debug.LogError("uiTrashCounterText не назначен!");
        }

        if (uiDishCounterText != null)
        {
            uiDishCounterText.text = $"Осталось посуды: {totalDishes - collectedDishes}";
        }
        else
        {
            Debug.LogError("uiDishCounterText не назначен!");
        }
    }

    void TaskCompleted()
    {
        // Скрываем UI задания
        if (taskUI != null)
        {
            taskUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Попытка скрыть taskUI, но он не назначен!");
        }

        // Проигрываем эффекты
        if (completionEffect != null)
        {
            completionEffect.Play();
        }
        if (audioSource != null)
        {
            AudioSource.PlayClipAtPoint(audioSource, transform.position);
        }

        Debug.Log("Задание выполнено! UI скрыт.");
    }
}