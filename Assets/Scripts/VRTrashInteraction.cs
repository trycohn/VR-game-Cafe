using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRTrashInteraction : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private TrashItem trashItem;
    private Vector3 lastPosition;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        trashItem = GetComponent<TrashItem>();

        if (grabInteractable != null)
        {
            // Подписываемся на события XR Interaction Toolkit
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Когда предмет отпускают, проверяем, в каком контейнере он находится
        CheckForRecycleBin();
    }

    private void CheckForRecycleBin()
    {
        // Используем OverlapBox или Sphere для проверки коллизий
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);


        foreach (var collider in colliders)
        {
            var recycleBin = collider.GetComponent<RecycleBin>();
            if (recycleBin != null)
            {
                recycleBin.ProcessReleasedTrash(trashItem);
                break;
            }
        }

        // Если предмет отпустили не над контейнером, ничего не делаем
    }
}