using UnityEngine;

public class TableSurface : MonoBehaviour
{
    [Header("Настройки")]
    public int totalDirtAreas = 3; // Общее количество грязных областей на этой поверхности

    [Header("Ссылки")]
    public CafeManager cafeManager;
    public ParticleSystem sparkleEffect;

    [Header("Текущее состояние")]
    private int dirtAreasCleaned = 0;
    private bool isSurfaceComplete = false;

    void Start()
    {
        // Изначально все грязные области не очищены
        dirtAreasCleaned = 0;
        isSurfaceComplete = false;
    }

    // Вызывается когда одна из грязных областей на этой поверхности очищена
    public void OnDirtAreaCleaned()
    {
        dirtAreasCleaned++;
        Debug.Log($"Поверхность стола: {dirtAreasCleaned}/{totalDirtAreas} областей очищено");

        // Воспроизводим эффект при каждой очистке
        if (sparkleEffect != null)
        {
            var effect = Instantiate(sparkleEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }

        // Проверяем, все ли области на этой поверхности очищены
        if (dirtAreasCleaned >= totalDirtAreas && !isSurfaceComplete)
        {
            CompleteSurface();
        }
    }

    private void CompleteSurface()
    {
        isSurfaceComplete = true;
        Debug.Log($"Поверхность стола {gameObject.name} полностью очищена!");

        // Сообщаем менеджеру, что одна поверхность очищена
        if (cafeManager != null)
        {
            cafeManager.OnTableSurfaceCleaned();
        }
    }

    // Метод для проверки состояния поверхности
    public bool IsComplete()
    {
        return isSurfaceComplete;
    }

    public int GetCleanedAreas()
    {
        return dirtAreasCleaned;
    }

    public int GetTotalAreas()
    {
        return totalDirtAreas;
    }
}