using UnityEngine;

public class NPC_Pathfinder : MonoBehaviour
{
    public Transform[] points; // Сюда перетащим наши точки
    public float speed = 2.0f; // Скорость бега
    public Animator animator;  // Ссылка на аниматор

    private int currentPointIndex = 0; // На какой точку идем сейчас
    private bool isFinished = false;

    void Update()
    {
        // Если маршрут закончен, ничего не делаем
        if (isFinished) return;

        // Если точек нет, выходим, чтобы не было ошибки
        if (points.Length == 0) return;

        // Получаем координаты текущей цели
        Transform target = points[currentPointIndex];

        // 1. Поворачиваемся к цели
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Чтобы он не смотрел в пол или небо
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        // 2. Идем к цели
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // 3. Проверяем, дошли ли мы (расстояние меньше 0.1 метра)
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Переключаемся на следующую точку
            currentPointIndex++;

            // Если точки закончились (это была последняя)
            if (currentPointIndex >= points.Length)
            {
                isFinished = true;
                SitDown(target); // Садимся
            }
        }
    }

    void SitDown(Transform finalPoint)
    {
        // Выравниваем персонажа точно как стоит последняя точка (чтобы ровно сел на стул)
        transform.position = finalPoint.position;
        transform.rotation = finalPoint.rotation;

        // Включаем анимацию сидения
        animator.SetBool("IsSitting", true);

        Debug.Log("Я сел!");
    }
}
