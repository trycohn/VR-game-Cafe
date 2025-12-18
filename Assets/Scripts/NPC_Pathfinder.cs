using UnityEngine;

public class NPC_Pathfinder : MonoBehaviour
{
    [Header("Settings")]
    public Transform[] points;       // Точки маршрута
    public float speed = 2.0f;       // Скорость
    public Animator animator;        // Аниматор
    [Header("Dialog")]
    public GameObject dialogButton; // Сюда перетащишь кнопку "Поприветствовать"
    public Vector3 buttonOffset = new Vector3(0, 1.5f, 0); // Смещение кнопки над NPC
    public bool followPlayer = true; // Кнопка поворачивается к игроку

    [Header("Behavior")]
    // Если true — NPC ждет кнопку. Если false — бежит сразу при старте.
    // Поставь эту галочку в Инспекторе, если хочешь, чтобы он ждал!
    public bool waitForButton = true;

    private int currentPointIndex = 0;
    private bool isFinished = false;
    private bool canMove = false;    // Внутренний флаг

    void Start()
    {
        // Если забыл привязать аниматор в инспекторе, ищем его сами
        if (animator == null)
            animator = GetComponent<Animator>();

        // Проверка назначения dialogButton
        if (dialogButton == null)
            Debug.LogWarning($"[{gameObject.name}] dialogButton не назначен! Кнопка не появится когда NPC сядет.");
        else
            Debug.Log($"[{gameObject.name}] dialogButton назначен: {dialogButton.name}, активен: {dialogButton.activeSelf}");

        // Проверка точек маршрута
        if (points == null || points.Length == 0)
            Debug.LogError($"[{gameObject.name}] Точки маршрута не заданы! NPC не сможет идти.");
        else
            Debug.Log($"[{gameObject.name}] Точек маршрута: {points.Length}");

        // Если НЕ ждем кнопку, то сразу разрешаем идти
        if (!waitForButton)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
            // Можно принудительно выключить анимацию ходьбы, если она была включена
            if (animator != null) animator.SetBool("IsWalking", false);
        }
    }

    void Update()
    {
        // Поворачиваем Canvas к игроку (если кнопка активна)
        if (isFinished && followPlayer && dialogButton != null && dialogButton.activeInHierarchy)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                // Поворачиваем родительский Canvas, а не саму кнопку
                Canvas canvas = dialogButton.GetComponentInParent<Canvas>();
                Transform toRotate = canvas != null ? canvas.transform : dialogButton.transform;
                
                toRotate.LookAt(mainCam.transform);
                // Инвертируем, чтобы текст был читаемым
                toRotate.Rotate(0, 180, 0);
            }
        }

        // 1. Если стоим (ждем кнопку) или уже пришли — ничего не делаем
        if (!canMove || isFinished) return;

        // 2. Защита от пустых точек
        if (points == null || points.Length == 0) return;

        // 3. Двигаемся к текущей точке
        Transform target = points[currentPointIndex];
        MoveToTarget(target);

        // 4. Проверяем, дошли ли
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex++;

            // Если точки закончились
            if (currentPointIndex >= points.Length)
            {
                isFinished = true;
                SitDown(target);
            }
        }
    }

    void MoveToTarget(Transform target)
    {
        // Поворот
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        // Движение
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void SitDown(Transform finalPoint)
    {
        Debug.Log("КОМАНДА САДИТЬСЯ!");
        transform.position = finalPoint.position;
        transform.rotation = finalPoint.rotation;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsSitting", true);
        }

        Debug.Log("Я сел!");
        
        // Включаем кнопку диалога
        if (dialogButton != null)
        {
            // Активируем всех родителей (если они неактивны)
            Transform parent = dialogButton.transform.parent;
            while (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            }
            
            // Позиционируем кнопку (или её Canvas) над NPC
            // Берём корневой Canvas для позиционирования
            Canvas canvas = dialogButton.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.transform.position = transform.position + buttonOffset;
            }
            else
            {
                dialogButton.transform.position = transform.position + buttonOffset;
            }
            
            dialogButton.SetActive(true);
            Debug.Log("Кнопка диалога АКТИВИРОВАНА: " + dialogButton.name);
        }
        else
        {
            Debug.LogError("dialogButton НЕ НАЗНАЧЕН в инспекторе! Перетащи кнопку 'Поприветствовать' в поле Dialog Button на NPC.");
        }
    }

    // --- ЭТУ ФУНКЦИЮ ВЕШАЙ НА КНОПКУ "ПРИНЯТЬ" ---
    public void StartRunning()
    {
        canMove = true;
        // Если у тебя есть параметр для ходьбы в аниматоре, раскомментируй:
        if(animator != null) animator.SetBool("IsWalking", true);
    }
}
