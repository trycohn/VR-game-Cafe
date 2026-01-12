using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandleXRGrabInteractable : XRGrabInteractable
{
    // переопределяем метод, срабатывающий при захвате объекта
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // вызываем корутину, которой передаем Transform руки (interactorObject - это объект который начал взаимодействие с нашим GrabInteractable, это может быть Direct Interactor, или, например, Ray Interactor)
        StartCoroutine(CancleGrabWhenHandMove(args.interactorObject.transform.parent));
    }

    private IEnumerator CancleGrabWhenHandMove(Transform handTransform)
    {
        while (true)
        {
            // вычисляем дистанцию между ручкой и рукой
            Vector3 distance = this.transform.position - handTransform.position;
            // если рука отклонилась на 0.3 метра в любую из сторон
            if (distance.magnitude > 2f)
            {
                // включаем и отключаем скрипт, таким образом рука оторвется от ручки
                this.enabled = false;
                this.enabled = true;
                yield break;
            }
            yield return null;
        }
    }
}