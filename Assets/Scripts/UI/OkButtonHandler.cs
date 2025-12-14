using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OkButtonHandler : MonoBehaviour
{
    private Button button;
    private CanvasSwitcher switcher;

    private void Awake()
    {
        button = GetComponent<Button>();
        switcher = GetComponentInParent<CanvasSwitcher>();
        
        if (switcher == null)
        {
            // Поиск в родительском Canvas
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                switcher = parentCanvas.GetComponent<CanvasSwitcher>();
            }
        }
    }

    private void Start()
    {
        if (button != null && switcher != null)
        {
            button.onClick.AddListener(switcher.OnOkButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (button != null && switcher != null)
        {
            button.onClick.RemoveListener(switcher.OnOkButtonClicked);
        }
    }
}
