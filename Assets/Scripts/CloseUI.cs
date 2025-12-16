using UnityEngine;
using UnityEngine.UI;

public class UIDisabler : MonoBehaviour
{
    [Header("������ �� UI �������")]
    [SerializeField] private GameObject uiToDisable; // UI ������� ����� ���������

    private Button button; // ������������ ������ �� ��������� ������

    void Start()
    {
        // �������� ��������� ������
        button = GetComponent<Button>();

        // ���������, ��� ������ ������������� �� ������
        if (button != null)
        {
            // ��������� ���������� �������
            button.onClick.AddListener(DisableUI);
        }
        else
        {
            // Пробуем найти Button в дочерних объектах
            button = GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(DisableUI);
            }
            else
            {
                Debug.LogWarning("UIDisabler: Button не найден на объекте " + gameObject.name);
            }
        }
    }

    // �����, ������� ���������� ��� ������� ������
    public void DisableUI()
    {
        if (uiToDisable != null)
        {
            uiToDisable.SetActive(false);
            Debug.Log($"UI ������� '{uiToDisable.name}' ��������.");
        }
        else
        {
            Debug.LogWarning("UI ������� ��� ���������� �� ��������!");
        }
    }

    // ������� ��� ����������� �������
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(DisableUI);
        }
    }
}