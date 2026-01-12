using UnityEngine;

public class NavigationLine : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Например, Main Camera в VR
    public Transform[] targetPoints; // Массив точек назначения

    [Header("Visual Settings")]
    public Material lineMaterial; // Материал для линии
    public float lineWidth = 0.1f; // Ширина линии
    public float lineHeight = 0.1f; // Высота линии над землёй

    [Header("Line Visibility")]
    public bool showLine = false; // Переключение отображения линии

    private LineRenderer lineRenderer;
    private int currentTargetIndex = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
        lineRenderer.enabled = false; // Сначала линия выключена
    }

    void SetupLineRenderer()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Применяем материал
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }

        // Применяем ширину
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    void Update()
    {
        if (!showLine || targetPoints.Length == 0 || currentTargetIndex >= targetPoints.Length)
        {
            lineRenderer.enabled = false;
            return;
        }

        Transform targetPoint = targetPoints[currentTargetIndex];
        if (targetPoint == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        // Позиции линии
        Vector3 playerPos = player.position;
        playerPos.y = lineHeight;

        Vector3 targetPos = targetPoint.position;
        targetPos.y = lineHeight;

        // Включаем линию и обновляем позиции
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, playerPos);
        lineRenderer.SetPosition(1, targetPos);

        // Проверяем, достиг ли игрок точки
        if (Vector3.Distance(player.position, targetPoint.position) < 1.5f)
        {
            currentTargetIndex++;

            // Если больше нет точек — выключаем линию
            if (currentTargetIndex >= targetPoints.Length)
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // Метод для включения линии
    public void EnableLine()
    {
        showLine = true;
        lineRenderer.enabled = true;
    }

    // Метод для выключения линии
    public void DisableLine()
    {
        showLine = false;
        lineRenderer.enabled = false;
    }

    // Метод для переключения видимости линии
    public void ToggleLine()
    {
        showLine = !showLine;
        lineRenderer.enabled = showLine;
    }

    // Метод для установки видимости линии
    public void SetLineVisibility(bool isVisible)
    {
        showLine = isVisible;
        lineRenderer.enabled = isVisible;
    }

    // Метод для сброса маршрута к начальной точке
    public void ResetRoute()
    {
        currentTargetIndex = 0;
        if (showLine && targetPoints.Length > 0)
        {
            lineRenderer.enabled = true;
        }
    }

    // Метод для установки конкретной точки назначения
    public void SetTargetPoint(int index)
    {
        if (index >= 0 && index < targetPoints.Length)
        {
            currentTargetIndex = index;
            if (showLine)
            {
                lineRenderer.enabled = true;
            }
        }
    }

    // Метод для проверки, включена ли линия
    public bool IsLineEnabled()
    {
        return showLine && lineRenderer.enabled;
    }

    // Метод для изменения ширины линии
    public void SetLineWidth(float newWidth)
    {
        lineWidth = newWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    // Метод для изменения высоты линии
    public void SetLineHeight(float newHeight)
    {
        lineHeight = newHeight;
    }

    // Метод для обновления материала линии
    public void SetLineMaterial(Material newMaterial)
    {
        lineMaterial = newMaterial;
        lineRenderer.material = lineMaterial;
    }

    // Метод для обновления массива точек назначения
    public void SetTargetPoints(Transform[] newTargetPoints)
    {
        targetPoints = newTargetPoints;
        currentTargetIndex = 0;
    }

    // Метод для добавления точки назначения
    public void AddTargetPoint(Transform newPoint)
    {
        // Создаем новый массив с дополнительным элементом
        Transform[] newArray = new Transform[targetPoints.Length + 1];
        targetPoints.CopyTo(newArray, 0);
        newArray[targetPoints.Length] = newPoint;
        targetPoints = newArray;
    }

    // Метод для очистки всех точек назначения
    public void ClearTargetPoints()
    {
        targetPoints = new Transform[0];
        lineRenderer.enabled = false;
    }
}