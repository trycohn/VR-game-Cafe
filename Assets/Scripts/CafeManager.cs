using UnityEngine;
using UnityEngine.Events;

public class CafeManager : MonoBehaviour
{
    [Header("Состояние задач")]
    public bool isTask1Complete = false; // Взять инструменты
    public bool isTask2Complete = false; // Протереть столы
    public bool isTask3Complete = false; // Расставить стулья

    [Header("Конфигурация")]
    public int totalTableSurfaces = 3; // Общее количество поверхностей для протирки
    public int totalChairs = 7; // Общее количество стульев для расстановки
    public int dirtyUtensilsCount = 3;
    public int totalDirtyUtensils = 3;

    [Header("Ссылки")]
    public WorldTaskUI worldTaskUI; // Ссылка для World UI
    public ParticleSystem celebrationEffect; // Эффект при завершении задачи
    public AudioClip allTasksCompleteSound;  // Звук при завершении ВСЕХ задач

    [Header("События")]
    public UnityEvent OnAllTasksCompleted = new UnityEvent();
    public UnityEvent OnTask2Completed = new UnityEvent(); // Протереть столы
    public UnityEvent OnTask3Completed = new UnityEvent(); // Расставить стулья

    [Header("Текущий прогресс")]
    private int cleanedTableSurfaces = 0;
    private int placedChairs = 0; // Количество размещенных стульев
    private bool[] chairPlacedStatus; // Массив для отслеживания статуса каждого стула
    private bool allTasksCompleted = false; // Флаг для отслеживания завершения всех задач

    void Start()
    {
        InitializeChairTracking();
        UpdateUI();
    }

    private void InitializeChairTracking()
    {
        chairPlacedStatus = new bool[totalChairs];
        for (int i = 0; i < totalChairs; i++)
        {
            chairPlacedStatus[i] = false;
        }
    }

    // Метод вызывается когда стул размещен
    public void OnChairPlaced(int chairIndex)
    {
        if (chairIndex >= 0 && chairIndex < totalChairs && !chairPlacedStatus[chairIndex])
        {
            chairPlacedStatus[chairIndex] = true;
            placedChairs++;

            Debug.Log($"Стульев размещено: {placedChairs}/{totalChairs}");

            // Обновляем UI
            UpdateUI();

            // Проверяем, все ли стулья размещены
            if (placedChairs >= totalChairs && !isTask3Complete)
            {
                CompleteTask3();
            }
        }
    }

    // Метод вызывается когда поверхность стола очищена
    public void OnTableSurfaceCleaned()
    {
        cleanedTableSurfaces++;
        Debug.Log($"Очищено поверхностей столов: {cleanedTableSurfaces}/{totalTableSurfaces}");

        // Обновляем UI
        UpdateUI();

        // Проверяем, все ли поверхности очищены
        if (cleanedTableSurfaces >= totalTableSurfaces && !isTask2Complete)
        {
            CompleteTask2();
        }
    }

    public void MarkTask1Complete()
    {
        if (!isTask1Complete)
        {
            isTask1Complete = true;
            Debug.Log("Task 1 Complete: Take tools");
            UpdateUI();
        }
    }

    private void CompleteTask2()
    {
        isTask2Complete = true;

        // Воспроизводим эффект завершения задачи
        if (celebrationEffect != null)
        {
            var effect = Instantiate(celebrationEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Вызываем событие завершения задачи
        OnTask2Completed?.Invoke();

        Debug.Log("Task 2 Complete: Clean tables");
        UpdateUI();

        // Проверяем выполнение всех задач
        CheckTaskCompletion();
    }

    private void CompleteTask3()
    {
        isTask3Complete = true;

        // Воспроизводим эффект завершения задачи
        if (celebrationEffect != null)
        {
            var effect = Instantiate(celebrationEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Вызываем событие завершения задачи
        OnTask3Completed?.Invoke();

        Debug.Log("Task 3 Complete: Arrange chairs");
        UpdateUI();

        // Проверяем выполнение всех задач
        CheckTaskCompletion();
    }

    public void DecreaseDirtyUtensils()
    {
        dirtyUtensilsCount--;
        UpdateUI();
        CheckTaskCompletion();
    }

    private void CheckTaskCompletion()
    {
        // Проверяем, все ли задачи выполнены, и если еще не вызывали событие
        if (isTask1Complete && isTask2Complete && isTask3Complete && !allTasksCompleted)
        {
            allTasksCompleted = true;

            // Проигрываем звук завершения ВСЕХ задач
            if (allTasksCompleteSound != null)
            {
                AudioSource.PlayClipAtPoint(allTasksCompleteSound, Camera.main.transform.position, 0.2f);
            }

            // Воспроизводим эффект завершения всех задач
            if (celebrationEffect != null)
            {
                var effect = Instantiate(celebrationEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, 3f);
            }

            // Вызываем событие завершения всех задач
            OnAllTasksCompleted?.Invoke();

            // Сообщаем WorldTaskUI, что все задачи выполнены
            if (worldTaskUI != null)
            {
                worldTaskUI.OnAllTasksCompleted();
            }
        }
    }

    private void UpdateUI()
    {
        // Обновляем World UI
        worldTaskUI?.ForceUpdate();
    }

    // Методы для получения текущего прогресса
    public int GetCleanedTableSurfaces() => cleanedTableSurfaces;
    public int GetTotalTableSurfaces() => totalTableSurfaces;
    public bool GetTask2Complete() => isTask2Complete;

    public int GetPlacedChairs() => placedChairs;
    public int GetTotalChairs() => totalChairs;
    public bool GetTask3Complete() => isTask3Complete;
}