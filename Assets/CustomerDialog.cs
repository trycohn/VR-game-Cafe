using UnityEngine;
using UnityEngine.UI;

public class CustomerDialog : MonoBehaviour
{
    public GameObject dialogButton;     // Ссылка на кнопку "Поприветствовать"
    public GameObject orderUI;          // Ссылка на панель с заказом
    public Transform npcTransform;      // Ссылка на NPC (для позиционирования)
    public Vector3 orderOffset = new Vector3(0, 1.5f, 0); // Смещение над головой
    
    [Header("Терминал")]
    public OrderTerminal orderTerminal; // Ссылка на терминал заказов

    // Вызывается при нажатии на кнопку "Поприветствовать"
    public void OnGreetButtonClicked()
    {
        // 1. Скрываем кнопку и её родителей
        if (dialogButton != null)
        {
            // Деактивируем весь Canvas кнопки
            Canvas buttonCanvas = dialogButton.GetComponentInParent<Canvas>();
            if (buttonCanvas != null)
                buttonCanvas.gameObject.SetActive(false);
            else
                dialogButton.SetActive(false);
        }

        // 2. Лог в консоль
        Debug.Log("Клиент: - Здравствуйте! - Вот мой заказ.");

        // 3. Показываем панель заказа
        if (orderUI != null)
        {
            // Активируем всех родителей orderUI
            Transform parent = orderUI.transform.parent;
            while (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            }
            
            // Позиционируем над NPC
            Canvas orderCanvas = orderUI.GetComponentInParent<Canvas>();
            if (orderCanvas != null && npcTransform != null)
            {
                orderCanvas.transform.position = npcTransform.position + orderOffset;
            }
            
            orderUI.SetActive(true);
            Debug.Log("Заказ показан: " + orderUI.name);
            
            // Показываем подсказку над терминалом
            if (orderTerminal != null)
            {
                orderTerminal.ShowHint();
            }
            else
            {
                Debug.LogWarning("OrderTerminal не назначен в CustomerDialog!");
            }
        }
        else
        {
            Debug.LogError("orderUI не назначен в CustomerDialog!");
        }
    }
    
    void Update()
    {
        // Поворачиваем заказ к игроку
        if (orderUI != null && orderUI.activeInHierarchy)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Canvas orderCanvas = orderUI.GetComponentInParent<Canvas>();
                if (orderCanvas != null)
                {
                    orderCanvas.transform.LookAt(mainCam.transform);
                    orderCanvas.transform.Rotate(0, 180, 0);
                }
            }
        }
    }
}
