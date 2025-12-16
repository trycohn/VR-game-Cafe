using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [Header("Визуальные настройки")]
    public Color hoverColor = Color.yellow; // Цвет при наведении
    private Color originalColor; // Исходный цвет
    private Renderer objectRenderer; // Рендерер объекта


    void Start()
    {
        // Получаем рендерер и сохраняем исходный цвет
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;

    }

    void OnMouseEnter()
    {
        // Меняем цвет
        if (objectRenderer != null)
            objectRenderer.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        // Возвращаем исходный цвет
        if (objectRenderer != null)
            objectRenderer.material.color = originalColor;
    }
}