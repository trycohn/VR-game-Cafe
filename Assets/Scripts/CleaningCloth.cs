using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class CleaningCloth : MonoBehaviour
{
    [Header("Настройки")]
    public int maxWipes = 10; // Макс протираний до высыхания
    public Material dryMaterial;
    public Material wetMaterial;
    public AudioClip wetSound;
    public AudioClip cleanSound;

    [Header("Текущее состояние")]
    public bool isWet = false;
    public int wipesLeft = 0;
    private bool hasBeenPickedUp = false;

    [Header("Настройки звука")]
    [SerializeField] private float cleanSoundCooldown = 1f; // Интервал между проигрываниями звука
    private float lastCleanSoundTime = 0f; // Время последнего проигрывания звука

    [Header("Ссылки")]
    public CafeManager cafeManager; // Ссылка на менеджер уровня

    private Renderer clothRenderer;
    private XRGrabInteractable grabInteractable;

    // Событие для отслеживания состояния
    public System.Action<bool> OnWetnessChanged;

    void Start()
    {
        clothRenderer = GetComponent<Renderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            // Подписываемся на события взятия и отпускания
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        if (clothRenderer != null)
        {
            SetDryMaterial();
        }

        // Инициализируем время последнего звука
        lastCleanSoundTime = -cleanSoundCooldown; // Гарантируем, что первый звук проиграется
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    // Вызывается при взятии тряпки
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!hasBeenPickedUp)
        {
            hasBeenPickedUp = true;

            // Сообщаем менеджеру, что тряпка взята
            if (cafeManager != null)
            {
                cafeManager.MarkTask1Complete();
            }
        }
    }

    // Вызывается при отпускании тряпки
    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("Тряпка отпущена");
    }

    // Вызывается при попадании в ведро
    public void SoakInBucket()
    {
        if (!isWet)
        {
            isWet = true;
            wipesLeft = maxWipes;
            SetWetMaterial();

            if (wetSound != null)
                AudioSource.PlayClipAtPoint(wetSound, transform.position);

            OnWetnessChanged?.Invoke(true);

            Debug.Log("Тряпка смочена. Протираний: " + wipesLeft);
        }
    }

    // Вызывается при протирании грязи
    public bool UseForCleaning()
    {
        if (!isWet || wipesLeft <= 0)
            return false;

        wipesLeft--;

        // Проигрываем звук чистки с интервалом
        if (cleanSound != null && CanPlayCleanSound())
        {
            PlayCleanSoundWithCooldown();
        }

        // Если тряпка высохла
        if (wipesLeft <= 0)
        {
            isWet = false;
            SetDryMaterial();
            OnWetnessChanged?.Invoke(false);
            Debug.Log("Тряпка высохла!");
        }

        return true;
    }

    // Проверяем, можно ли проиграть звук чистки (с учетом интервала)
    private bool CanPlayCleanSound()
    {
        return Time.time - lastCleanSoundTime >= cleanSoundCooldown;
    }

    // Проигрываем звук чистки и обновляем время последнего проигрывания
    private void PlayCleanSoundWithCooldown()
    {
        AudioSource.PlayClipAtPoint(cleanSound, transform.position);
        lastCleanSoundTime = Time.time;

        // Логирование для отладки
        Debug.Log($"Звук чистки проигран. Время: {Time.time}, Следующий через: {cleanSoundCooldown} сек");
    }

    public void PlayCleanSoundIfAllowed()
    {
        if (cleanSound != null && CanPlayCleanSound())
        {
            PlayCleanSoundWithCooldown();
        }
    }

    private void SetWetMaterial()
    {
        if (clothRenderer != null && wetMaterial != null)
            clothRenderer.material = wetMaterial;
    }

    private void SetDryMaterial()
    {
        if (clothRenderer != null && dryMaterial != null)
            clothRenderer.material = dryMaterial;
    }

    // Метод для проверки, готова ли тряпка к использованию
    public bool CanBeUsedForCleaning()
    {
        return isWet && wipesLeft > 0;
    }

    // Метод для проверки, была ли тряпка взята 
    public bool HasBeenPickedUp()
    {
        return hasBeenPickedUp;
    }

    // Метод для настройки интервала звука (можно вызывать извне)
    public void SetCleanSoundCooldown(float newCooldown)
    {
        cleanSoundCooldown = Mathf.Max(0.1f, newCooldown); // Минимальный интервал 0.1 секунды
        Debug.Log($"Интервал звука чистки установлен: {cleanSoundCooldown} сек");
    }

    // Метод для получения текущего интервала
    public float GetCleanSoundCooldown()
    {
        return cleanSoundCooldown;
    }

    // Метод для получения времени до следующего возможного проигрывания звука
    public float GetTimeUntilNextCleanSound()
    {
        float timeSinceLastSound = Time.time - lastCleanSoundTime;
        if (timeSinceLastSound >= cleanSoundCooldown)
        {
            return 0f;
        }
        return cleanSoundCooldown - timeSinceLastSound;
    }
}