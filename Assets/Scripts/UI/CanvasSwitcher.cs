using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject hintCanvas;
    [SerializeField] private GameObject taskCanvas;

    private void Start()
    {
        // При старте показываем подсказку, скрываем задания
        if (hintCanvas != null)
            hintCanvas.SetActive(true);
        
        if (taskCanvas != null)
            taskCanvas.SetActive(false);
    }

    /// <summary>
    /// Вызывается при нажатии кнопки OK
    /// </summary>
    public void OnOkButtonClicked()
    {
        // Скрываем подсказку
        if (hintCanvas != null)
            hintCanvas.SetActive(false);
        
        // Показываем задания
        if (taskCanvas != null)
            taskCanvas.SetActive(true);
    }
}
