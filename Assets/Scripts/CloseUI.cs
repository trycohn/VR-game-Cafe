using UnityEngine;
using UnityEngine.UI;

public class UIDisabler : MonoBehaviour
{
    [Header("Ссылка на UI элемент")]
    [SerializeField] private GameObject uiToDisable; // UI который нужно отключить

    private Button button; // Кэшированная ссылка на компонент кнопки

    void Start()
    {
        // Получаем компонент кнопки
        button = GetComponent<Button>();

        // Проверяем, что скрипт действительно на кнопке
        if (button != null)
        {
            // Добавляем обработчик нажатия
            button.onClick.AddListener(DisableUI);
        }
        else
        {
            Debug.LogError("Скрипт UIDisabler должен быть на объекте с компонентом Button!");
        }
    }

    // Метод, который вызывается при нажатии кнопки
    public void DisableUI()
    {
        if (uiToDisable != null)
        {
            uiToDisable.SetActive(false);
            Debug.Log($"UI элемент '{uiToDisable.name}' отключен.");
        }
        else
        {
            Debug.LogWarning("UI элемент для отключения не назначен!");
        }
    }

    // Очистка при уничтожении объекта
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(DisableUI);
        }
    }
}