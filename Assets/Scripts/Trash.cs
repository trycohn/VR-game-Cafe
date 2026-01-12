using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum TrashType
{
    Plastic,
    Paper,
    Dish
}

public class TrashItem : MonoBehaviour
{
    public TrashType type;

    // Кэшированные компоненты
    private Material originalMaterial;
    private Renderer rend;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    // События - используем делегат с пулом для уменьшения аллокаций
    public event System.Action<TrashItem> OnGrabbed;
    public event System.Action<TrashItem> OnReleased;

    // Свойства
    public bool IsHeld { get; private set; }
    public XRGrabInteractable GrabInteractable => grabInteractable;
    public Rigidbody Rigidbody => rb;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (rend != null)
        {
            originalMaterial = rend.material;
        }
    }

    void Start()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(HandleGrabbed);
            grabInteractable.selectExited.AddListener(HandleReleased);
        }
    }

    private void HandleGrabbed(SelectEnterEventArgs args)
    {
        IsHeld = true;
        OnGrabbed?.Invoke(this);
    }

    private void HandleReleased(SelectExitEventArgs args)
    {
        IsHeld = false;
        OnReleased?.Invoke(this);
    }

    public void SetRed()
    {
        if (rend != null)
        {
            rend.material.color = Color.red;
        }
    }

    public void ResetColor()
    {
        if (rend != null && originalMaterial != null)
        {
            rend.material = originalMaterial;
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(HandleGrabbed);
            grabInteractable.selectExited.RemoveListener(HandleReleased);
        }
    }
}