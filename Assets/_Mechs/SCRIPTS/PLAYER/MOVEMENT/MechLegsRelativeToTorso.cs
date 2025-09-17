using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Движение меха в осях ТОРСА:
/// - вход (WASD/левый стик) берется как Vector2 (x=влево/вправо, y=вперед/назад)
/// - вектор сначала поворачивается по Y-углу торса (Yaw Source),
/// - потом применяем к Rigidbody: MovePosition + плавный поворот корпуса ног.
/// Торс (T_Torso) крутится своим отдельным скриптом и камерой, независимо от ног.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Mechs/Movement/Mech Legs Relative To Torso")]
public class MechLegsRelativeToTorso : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input Action: Player/Move (Vector2). Перетащи ссылку из .inputactions.")]
    [SerializeField] private InputActionProperty move;

    [Header("Frame of Reference")]
    [Tooltip("Источник рысканья (Yaw). Обычно T_Torso. Только его угол по Y используется.")]
    [SerializeField] private Transform yawSource; // T_Torso

    [Header("Movement")]
    [Tooltip("Линейная скорость при полном наклоне стика (м/с).")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("Скорость разворота корпуса ног к направлению движения (deg/sec).")]
    [SerializeField] private float turnSpeed = 540f;

    [Tooltip("Мёртвая зона стика. Меньше — игнорируем.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float deadzone = 0.12f;

    [Header("Physics")]
    [Tooltip("(Опционально) Явный Rigidbody корня меха. Если пусто — найдется автоматически.")]
    [SerializeField] private Rigidbody rootRigidbody;

    // внутреннее состояние
    private Rigidbody _rb;
    private Vector3 _lastMoveDir = Vector3.forward;  // чтобы поворот корпуса сохранялся, когда отпускаем стик

    private void Awake()
    {
        // Найти RB: явный → на себе → у родителей → на корне трансформа
        _rb = rootRigidbody
           ?? GetComponent<Rigidbody>()
           ?? GetComponentInParent<Rigidbody>(true)
           ?? (transform.root ? transform.root.GetComponent<Rigidbody>() : null);

        if (_rb == null)
        {
            Debug.LogError("[MechLegsRelativeToTorso] Rigidbody не найден. Положи один RB на корень меха или назначь вручную.");
            enabled = false;
            return;
        }

        // Для ровного движения
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (!yawSource)
        {
            // Попробуем угадать ось торса по имени (на случай если забыли назначить)
            yawSource = FindYawSourceByName();
            if (!yawSource)
                Debug.LogWarning("[MechLegsRelativeToTorso] Yaw Source не задан. Движение будет в мировых осях (как в старом скрипте).");
        }
    }

    private Transform FindYawSourceByName()
    {
        // Небольшая «магия» для автопоиска: ищем дочерние трансформы с подходящим именем
        var root = transform.root ? transform.root : transform;
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            var n = t.name.ToLowerInvariant();
            if (n.Contains("t_torso") || n == "ttorso" || n.Contains("torso"))
                return t;
        }
        return null;
    }

    private void OnEnable() => move.action?.Enable();
    private void OnDisable() => move.action?.Disable();

    private void FixedUpdate()
    {
        var action = move.action;
        if (action == null || _rb == null) return;

        Vector2 v = action.ReadValue<Vector2>();
        float mag = v.magnitude;

        if (mag < deadzone)
        {
            // Нет входа — только плавно довернем корпус к последнему направлению, если хочется.
            RotateHullTowards(_lastMoveDir, 0f); // 0f => только поворот, без сдвига
            return;
        }

        // 1) Локальный вектор из входа (x=стрейф, y=вперед)
        Vector3 localMove = new Vector3(v.x, 0f, v.y).normalized;

        // 2) Плоский поворот по углу Y торса. Игнорируем крены/питч на всякий случай.
        Vector3 worldDir = localMove;
        if (yawSource)
        {
            float yaw = yawSource.eulerAngles.y;
            Quaternion yawOnly = Quaternion.Euler(0f, yaw, 0f);
            worldDir = yawOnly * localMove;
        }

        _lastMoveDir = worldDir; // запомнили для поворота корпуса, когда отпускаем стик

        // 3) Применяем движение к Rigidbody
        float speed = moveSpeed * Mathf.Clamp01(mag);
        Vector3 delta = worldDir * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + delta);

        // 4) Разворачиваем корпус ног к направлению движения
        RotateHullTowards(worldDir, turnSpeed * Time.fixedDeltaTime);
    }

    private void RotateHullTowards(Vector3 worldDir, float maxStepDeg)
    {
        if (worldDir.sqrMagnitude < 0.0001f) return;

        // Целевой поворот только по Y
        Quaternion targetRot = Quaternion.LookRotation(worldDir, Vector3.up);
        Quaternion current = _rb.rotation;

        // Вырезаем наклоны, оставляем рысканье
        Vector3 e = current.eulerAngles;
        current = Quaternion.Euler(0f, e.y, 0f);

        Quaternion newRot = (maxStepDeg > 0f)
            ? Quaternion.RotateTowards(current, targetRot, maxStepDeg)
            : targetRot;

        _rb.MoveRotation(newRot);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
        Gizmos.DrawRay(_rb ? _rb.position + Vector3.up * 0.05f : transform.position, _lastMoveDir * 2f);
    }
#endif
}
