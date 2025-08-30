using UnityEngine;
using UnityEngine.InputSystem;

public class TorsoAimer : MonoBehaviour
{
    // In Inspector: drag the Action Reference **Gameplay/Aim (Vector2)** here.
    // Советы: пока не используй <Mouse>/delta — оставь JIKL и/или <Gamepad>/rightStick.
    [Header("Input")]
    [Tooltip("Input Action Reference: Gameplay/Aim (Vector2). Drag from your .inputactions asset.")]
    [SerializeField] private InputActionProperty aim;

    // Узел башни, который должен вращаться (родитель всех визуальных частей торса/пушки).
    [Header("References")]
    [Tooltip("Transform of the turret pivot (rotating upper body). Usually a child of the mech root.")]
    [SerializeField] private Transform torsoPivot;

    [Header("Turret Settings")]
    [Tooltip("How fast the turret turns to the target world heading (deg/sec).")]
    [SerializeField] private float rotateSpeed = 360f;

    [Tooltip("Dead zone for the right stick/keys.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float deadzone = 0.15f;

    // Целевой МИРОВОЙ курс башни (угол Y в градусах). Мы его запоминаем и держим.
    private float _targetWorldYaw;

    private void Awake()
    {
        if (!torsoPivot) torsoPivot = transform; // на случай, если забыли проставить ссылку
        _targetWorldYaw = torsoPivot.rotation.eulerAngles.y; // стартуем с текущего мирового угла
    }

    private void OnEnable() => aim.action?.Enable();
    private void OnDisable() => aim.action?.Disable();

    private void Update()
    {
        if (!torsoPivot) return;

        // Вектор прицеливания из действия: (0,1)=мировое +Z, (1,0)=мировое +X.
        Vector2 v = aim.action != null ? aim.action.ReadValue<Vector2>() : Vector2.zero;

        // Если есть ввод — обновляем «цель» (мировой курс).
        // Если ввода нет — просто держим предыдущий.
        if (v.sqrMagnitude >= deadzone * deadzone)
            _targetWorldYaw = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;

        // Плавно доворачиваем башню к МИРОВОМУ углу:
        // используем world rotation (rotation), а не localRotation — так ноги могут крутиться независимо.
        Quaternion desiredWorld = Quaternion.Euler(0f, _targetWorldYaw, 0f);
        torsoPivot.rotation = Quaternion.RotateTowards(torsoPivot.rotation, desiredWorld, rotateSpeed * Time.deltaTime);
    }
}
