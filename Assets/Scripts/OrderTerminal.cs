using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OrderTerminal : MonoBehaviour
{
    [Header("UI Подсказки")]
    public GameObject hintUI;                    // Подсказка над терминалом
    public Vector3 hintOffset = new Vector3(0, 1f, 0);
    
    [Header("UI Терминала")]
    public GameObject terminalPanel;             // Панель терминала с кнопками
    public Button sendOrderButton;               // Кнопка "Отправить на кухню"
    
    [Header("Кнопки напитков")]
    public Button coffeeButton;                  // Кнопка кофе
    public Button teaButton;                     // Кнопка чая
    public Button juiceButton;                   // Кнопка сока
    
    [Header("Кнопки выпечки")]
    public Button croissantButton;               // Кнопка круассана
    public Button muffinButton;                  // Кнопка маффина
    public Button cakeButton;                    // Кнопка торта
    
    [Header("Визуальная обратная связь")]
    public Image terminalBackground;             // Фон терминала для мигания
    public Color normalColor = Color.white;
    public Color successColor = Color.green;
    public Color errorColor = Color.red;
    public float flashDuration = 0.3f;
    public int flashCount = 3;
    
    [Header("Правильный заказ")]
    public string correctDrink = "coffee";       // Правильный напиток
    public string correctFood = "croissant";     // Правильная выпечка
    
    [Header("Звуки")]
    public AudioClip successSound;
    public AudioClip errorSound;
    
    // Текущий выбор
    private string selectedDrink = "";
    private string selectedFood = "";
    
    // Состояние выбранных кнопок
    private Button currentDrinkButton;
    private Button currentFoodButton;
    private Color defaultButtonColor;
    private Color selectedButtonColor = new Color(0.7f, 1f, 0.7f); // Светло-зелёный
    
    private bool orderCompleted = false;
    private AudioSource audioSource;

    void Start()
    {
        Debug.Log("OrderTerminal: Start() вызван на " + gameObject.name);
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        // Скрываем подсказку в начале
        if (hintUI != null)
            hintUI.SetActive(false);
        
        // Настраиваем кнопки напитков
        if (coffeeButton != null)
        {
            defaultButtonColor = coffeeButton.GetComponent<Image>().color;
            coffeeButton.onClick.AddListener(() => SelectDrink("coffee", coffeeButton));
            Debug.Log("OrderTerminal: CoffeeButton подключена");
        }
        else
            Debug.LogWarning("OrderTerminal: CoffeeButton НЕ назначена!");
            
        if (teaButton != null)
            teaButton.onClick.AddListener(() => SelectDrink("tea", teaButton));
        if (juiceButton != null)
            juiceButton.onClick.AddListener(() => SelectDrink("juice", juiceButton));
        
        // Настраиваем кнопки выпечки
        if (croissantButton != null)
        {
            croissantButton.onClick.AddListener(() => SelectFood("croissant", croissantButton));
            Debug.Log("OrderTerminal: CroissantButton подключена");
        }
        else
            Debug.LogWarning("OrderTerminal: CroissantButton НЕ назначена!");
            
        if (muffinButton != null)
            muffinButton.onClick.AddListener(() => SelectFood("muffin", muffinButton));
        if (cakeButton != null)
            cakeButton.onClick.AddListener(() => SelectFood("cake", cakeButton));
        
        // Настраиваем кнопку отправки
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(SendOrder);
            Debug.Log("OrderTerminal: SendButton подключена");
        }
        else
            Debug.LogWarning("OrderTerminal: SendButton НЕ назначена!");
    }
    
    // ===== ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ КНОПОК (можно вызывать через On Click в Inspector) =====
    
    public void OnCoffeeClick()
    {
        Debug.Log("OnCoffeeClick вызван!");
        SelectDrink("coffee", coffeeButton);
    }
    
    public void OnTeaClick()
    {
        SelectDrink("tea", teaButton);
    }
    
    public void OnJuiceClick()
    {
        SelectDrink("juice", juiceButton);
    }
    
    public void OnCroissantClick()
    {
        Debug.Log("OnCroissantClick вызван!");
        SelectFood("croissant", croissantButton);
    }
    
    public void OnMuffinClick()
    {
        SelectFood("muffin", muffinButton);
    }
    
    public void OnCakeClick()
    {
        SelectFood("cake", cakeButton);
    }
    
    public void OnSendOrderClick()
    {
        Debug.Log("OnSendOrderClick вызван!");
        SendOrder();
    }
    
    // Вызывается из CustomerDialog когда показывается заказ
    public void ShowHint()
    {
        if (hintUI != null)
        {
            // Активируем родителей подсказки
            Transform parent = hintUI.transform.parent;
            while (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            }
            
            // Позиционируем над терминалом
            Canvas hintCanvas = hintUI.GetComponentInParent<Canvas>();
            if (hintCanvas != null)
            {
                hintCanvas.transform.position = transform.position + hintOffset;
            }
            
            hintUI.SetActive(true);
            Debug.Log("Подсказка над терминалом показана");
        }
    }
    
    void SelectDrink(string drink, Button button)
    {
        if (orderCompleted) return;
        
        // Сбрасываем цвет предыдущей кнопки
        if (currentDrinkButton != null)
        {
            currentDrinkButton.GetComponent<Image>().color = defaultButtonColor;
        }
        
        selectedDrink = drink;
        currentDrinkButton = button;
        
        // Подсвечиваем выбранную кнопку
        if (button != null)
        {
            button.GetComponent<Image>().color = selectedButtonColor;
        }
        
        Debug.Log("Выбран напиток: " + drink);
    }
    
    void SelectFood(string food, Button button)
    {
        if (orderCompleted) return;
        
        // Сбрасываем цвет предыдущей кнопки
        if (currentFoodButton != null)
        {
            currentFoodButton.GetComponent<Image>().color = defaultButtonColor;
        }
        
        selectedFood = food;
        currentFoodButton = button;
        
        // Подсвечиваем выбранную кнопку
        if (button != null)
        {
            button.GetComponent<Image>().color = selectedButtonColor;
        }
        
        Debug.Log("Выбрана выпечка: " + food);
    }
    
    void SendOrder()
    {
        if (orderCompleted) return;
        
        // Проверяем, выбрано ли что-то
        if (string.IsNullOrEmpty(selectedDrink) || string.IsNullOrEmpty(selectedFood))
        {
            Debug.Log("Выберите напиток И выпечку!");
            StartCoroutine(FlashTerminal(errorColor));
            return;
        }
        
        // Проверяем правильность заказа
        if (selectedDrink == correctDrink && selectedFood == correctFood)
        {
            // Заказ правильный!
            Debug.Log("Заказ правильный! Отправляем на кухню.");
            orderCompleted = true;
            
            if (successSound != null)
                audioSource.PlayOneShot(successSound);
            
            StartCoroutine(FlashTerminal(successColor));
            
            // Скрываем подсказку
            if (hintUI != null)
            {
                Canvas hintCanvas = hintUI.GetComponentInParent<Canvas>();
                if (hintCanvas != null)
                    hintCanvas.gameObject.SetActive(false);
            }
            
            // Вызываем событие успешного заказа (можно добавить UnityEvent)
            OnOrderSuccess();
        }
        else
        {
            // Заказ неправильный
            Debug.Log("Ошибка! Неправильный заказ. Попробуйте ещё раз.");
            
            if (errorSound != null)
                audioSource.PlayOneShot(errorSound);
            
            StartCoroutine(FlashTerminal(errorColor));
            
            // Сбрасываем выбор
            ResetSelection();
        }
    }
    
    void ResetSelection()
    {
        selectedDrink = "";
        selectedFood = "";
        
        if (currentDrinkButton != null)
        {
            currentDrinkButton.GetComponent<Image>().color = defaultButtonColor;
            currentDrinkButton = null;
        }
        
        if (currentFoodButton != null)
        {
            currentFoodButton.GetComponent<Image>().color = defaultButtonColor;
            currentFoodButton = null;
        }
    }
    
    IEnumerator FlashTerminal(Color flashColor)
    {
        if (terminalBackground == null) yield break;
        
        for (int i = 0; i < flashCount; i++)
        {
            terminalBackground.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            terminalBackground.color = normalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
    
    void OnOrderSuccess()
    {
        // Здесь можно добавить логику после успешного заказа
        // Например, начать готовить напиток, обновить задание и т.д.
        Debug.Log("Заказ принят! Готовим кофе и круассан...");
    }
    
    void Update()
    {
        // Поворачиваем подсказку к игроку
        if (hintUI != null && hintUI.activeInHierarchy)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Canvas hintCanvas = hintUI.GetComponentInParent<Canvas>();
                if (hintCanvas != null)
                {
                    hintCanvas.transform.LookAt(mainCam.transform);
                    hintCanvas.transform.Rotate(0, 180, 0);
                }
            }
        }
    }
    
    // Публичный метод для сброса терминала (если нужно)
    public void ResetTerminal()
    {
        orderCompleted = false;
        ResetSelection();
    }
}

