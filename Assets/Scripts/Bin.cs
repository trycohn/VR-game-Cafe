using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RecycleBin : MonoBehaviour
{
    public TrashType acceptedType;
    public Transform respawnPoint;
    public AudioClip wrongBinSound;

    [Header("Materials")]
    public Material correctMaterial;
    public Material incorrectMaterial;

    // Кэшированные компоненты
    private Renderer rend;
    private Material originalMaterial;
    private AudioSource audioSource;

    // Оптимизированные коллекции
    private HashSet<TrashItem> trashItemsInBin = new HashSet<TrashItem>();
    private List<TrashItem> itemsToProcessBuffer = new List<TrashItem>(10); // Предварительно выделенная память

    // Оптимизация Update
    private float checkTimer = 0f;
    private const float CHECK_INTERVAL = 0.2f;

    // Статический список всех контейнеров
    private static List<RecycleBin> allBins = new List<RecycleBin>();

    public static IReadOnlyList<RecycleBin> AllBins => allBins;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
            rend.enabled = false;
        }

        // Регистрируем контейнер в статическом списке
        allBins.Add(this);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 10f;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= CHECK_INTERVAL)
        {
            checkTimer = 0f;
            CheckForReleasedItemsInBin();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Используем TryGetComponent для оптимизации
        if (other.TryGetComponent<TrashItem>(out var trash) && trashItemsInBin.Add(trash))
        {
            trash.OnReleased += HandleTrashReleased;
            UpdateContainerMaterial();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<TrashItem>(out var trash) && trashItemsInBin.Remove(trash))
        {
            trash.OnReleased -= HandleTrashReleased;

            if (trashItemsInBin.Count == 0)
            {
                if (rend != null) rend.enabled = false;
            }
            else
            {
                UpdateContainerMaterial();
            }
        }
    }

    private void HandleTrashReleased(TrashItem trash)
    {
        if (trashItemsInBin.Contains(trash))
        {
            ProcessTrashItem(trash);
        }
    }

    private void CheckForReleasedItemsInBin()
    {
        if (trashItemsInBin.Count == 0) return;

        // Используем предварительно выделенный буфер
        itemsToProcessBuffer.Clear();

        // Быстрая проверка через foreach
        foreach (var trash in trashItemsInBin)
        {
            if (trash != null && !trash.IsHeld)
            {
                itemsToProcessBuffer.Add(trash);
            }
        }

        // Обрабатываем собранные предметы
        for (int i = 0; i < itemsToProcessBuffer.Count; i++)
        {
            ProcessTrashItem(itemsToProcessBuffer[i]);
        }
    }

    private void ProcessTrashItem(TrashItem trash)
    {
        if (!trashItemsInBin.Remove(trash)) return;

        trash.OnReleased -= HandleTrashReleased;

        if (trash.type == acceptedType)
        {
            GameManager.Instance?.CollectTrash(trash);

            Destroy(trash.gameObject);
        }
        else
        {
            TeleportTrashItem(trash);

            if (wrongBinSound != null)
            {
                audioSource?.PlayOneShot(wrongBinSound);
            }
        }

        // Обновляем видимость контейнера
        if (trashItemsInBin.Count == 0)
        {
            if (rend != null) rend.enabled = false;
        }
        else
        {
            UpdateContainerMaterial();
        }
    }

    private void UpdateContainerMaterial()
    {
        if (rend == null) return;

        rend.enabled = true;

        bool hasWrongItem = false;
        foreach (var item in trashItemsInBin)
        {
            if (item != null && item.type != acceptedType)
            {
                hasWrongItem = true;
                break;
            }
        }

        rend.material = hasWrongItem
            ? (incorrectMaterial ?? originalMaterial)
            : (correctMaterial ?? originalMaterial);
    }

    // Этот метод вызывается, когда предмет отпускают
    public void ProcessReleasedTrash(TrashItem trash)
    {
        if (trashItemsInBin.Contains(trash))
        {
            // Убираем предмет из списка
            trashItemsInBin.Remove(trash);

            if (trash.type == acceptedType)
            {
                // Правильный контейнер
                GameManager.Instance.CollectTrash(trash);
                // Уничтожаем предмет после сбора
                Destroy(trash.gameObject);
            }
            else
            {
                // Неправильный контейнер - телепортируем предмет
                TeleportTrashItem(trash);

                // Проигрываем звук
                if (wrongBinSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(wrongBinSound);
                }
            }

            // Обновляем видимость контейнера после обработки предмета
            if (trashItemsInBin.Count == 0)
            {
                if (rend != null)
                {
                    rend.enabled = false;
                }
            }
            else
            {
                UpdateContainerMaterial();
            }
        }
    }

    private void TeleportTrashItem(TrashItem trash)
    {
        if (respawnPoint == null)
        {
            Destroy(trash.gameObject);
            return;
        }

        // Кэшированные компоненты для оптимизации
        var rb = trash.Rigidbody;
        var grabInteractable = trash.GrabInteractable;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        trash.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (grabInteractable != null)
        {
            StartCoroutine(EnableGrabInteractable(grabInteractable));
        }

        trash.ResetColor();
    }

    private System.Collections.IEnumerator EnableGrabInteractable(XRGrabInteractable grabInteractable)
    {
        yield return null;
        grabInteractable.enabled = true;
    }

    public void ShowCorrect()
    {
        if (rend != null && correctMaterial != null)
        {
            rend.material = correctMaterial;
            rend.enabled = true;
        }
    }

    public void ShowIncorrect()
    {
        if (rend != null && incorrectMaterial != null)
        {
            rend.material = incorrectMaterial;
            rend.enabled = true;
        }
    }

    public void HideContainer()
    {
        if (rend != null) rend.enabled = false;
    }

    private void OnDestroy()
    {
        // Отписываемся от всех предметов
        foreach (var trash in trashItemsInBin)
        {
            if (trash != null)
            {
                trash.OnReleased -= HandleTrashReleased;
            }
        }

        trashItemsInBin.Clear();

        // Удаляем контейнер из статического списка
        allBins.Remove(this);
    }
}