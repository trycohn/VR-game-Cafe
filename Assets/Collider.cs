using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChairColliderStabilizer : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField] private bool freezeCollidersOnThrow = true;
    [SerializeField] private bool useBoxCollider = true;

    private Vector3[] originalColliderSizes;
    private Vector3[] originalColliderCenters;
    private Collider[] allColliders;

    void Start()
    {
        // Сохраняем исходные параметры коллайдеров
        SaveColliderParameters();

        // Подписываемся на события XR Grab Interactable
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnThrow);
        }
    }

    void SaveColliderParameters()
    {
        allColliders = GetComponentsInChildren<Collider>();
        originalColliderSizes = new Vector3[allColliders.Length];
        originalColliderCenters = new Vector3[allColliders.Length];

        for (int i = 0; i < allColliders.Length; i++)
        {
            if (allColliders[i] is BoxCollider box)
            {
                originalColliderSizes[i] = box.size;
                originalColliderCenters[i] = box.center;
            }
            else if (allColliders[i] is SphereCollider sphere)
            {
                originalColliderSizes[i] = Vector3.one * sphere.radius;
                originalColliderCenters[i] = sphere.center;
            }
        }
    }

    void OnThrow(SelectExitEventArgs args)
    {
        if (freezeCollidersOnThrow)
        {
            StartCoroutine(StabilizeCollidersAfterThrow());
        }
    }

    System.Collections.IEnumerator StabilizeCollidersAfterThrow()
    {
        // Ждем немного после броска
        yield return new WaitForSeconds(0.1f);

        // Восстанавливаем коллайдеры
        for (int i = 0; i < allColliders.Length; i++)
        {
            if (allColliders[i] != null)
            {
                if (allColliders[i] is BoxCollider box)
                {
                    box.size = originalColliderSizes[i];
                    box.center = originalColliderCenters[i];
                }
                else if (allColliders[i] is SphereCollider sphere)
                {
                    sphere.radius = originalColliderSizes[i].x;
                    sphere.center = originalColliderCenters[i];
                }
            }
        }
    }

    void LateUpdate()
    {
        // Постоянно проверяем коллайдеры (менее эффективно, но надежно)
        if (freezeCollidersOnThrow)
        {
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null)
                {
                    if (allColliders[i] is BoxCollider box)
                    {
                        box.size = originalColliderSizes[i];
                        box.center = originalColliderCenters[i];
                    }
                    else if (allColliders[i] is SphereCollider sphere)
                    {
                        sphere.radius = originalColliderSizes[i].x;
                        sphere.center = originalColliderCenters[i];
                    }
                }
            }
        }
    }
}