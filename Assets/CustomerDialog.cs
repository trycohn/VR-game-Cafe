using UnityEngine;
using UnityEngine.UI;

public class CustomerDialog : MonoBehaviour
{
    public GameObject dialogButton;     // Ссылка на кнопку "Поприветствовать" (чтобы скрыть её)
    public GameObject orderUI;          // Ссылка на текст/карточку с заказом (чтобы показать её)

    // Эту функцию мы привяжем к кнопке "Поприветствовать"
    public void OnGreetButtonClicked()
    {
        // 1. Прячем кнопку (чтобы нельзя было нажать 2 раза)
        if (dialogButton != null)
            dialogButton.SetActive(false);

        // 2. Пишем в консоль (для проверки)
        Debug.Log("Диалог: - Здравствуйте! - Мне кофе и круассан.");

        // 3. Сразу показываем заказ (без задержек и звуков)
        if (orderUI != null)
            orderUI.SetActive(true);
    }
}
