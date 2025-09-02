using UnityEngine;
using UnityEngine.InputSystem;

public class TorsoAimer : MonoBehaviour
{
    // Absolute aim (Vector2): right stick / JIKL → задаёт абсолютное мировое направление
    [Header("Input")]
    [Tooltip("Input Action Reference: Gameplay/Aim (Vector2). Right stick or keys (e.g., JIKL).")]
    [SerializeField] private InputActionProperty aim;

    // Mouse delta (Vector2): <Mouse>/delta → инкрементально крутит башню по X
    [Tooltip("Input Action Reference: Gameplay/AimMouse (Vector2). <Mouse>/delta.")]
    [SerializeField] private InputActionProperty aimMouse; // новое действие для мыши

    [Header("References")]
    [Tooltip("Transform of the turret pivot (rotating upper body). Usually a child of the mech root.")]
    [SerializeField] private Transform torsoPivot;

    [Header("Turret Settings")]
    [Tooltip("How fast the turret turns toward the target world heading (deg/sec).")]
    [SerializeField] private float rotateSpeed = 360f;

    [Tooltip("Dead zone for absolute stick/keys vector.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float deadzone = 0.15f;

    [Tooltip("Mouse X sensitivity in degrees per pixel (how much yaw per 1px).")]
    [SerializeField] private float mouseYawPerPixel = 0.25f;

    // Целевой МИРОВОЙ курс башни (угол Y, градусы), который башня «держит».
    private float _targetWorldYaw;

    private void Awake()
    {
        if (!torsoPivot) torsoPivot = transform;
        _targetWorldYaw = torsoPivot.rotation.eulerAngles.y;
    }

    private void OnEnable()
    {
        aim.action?.Enable();
        aimMouse.action?.Enable();
    }

    private void OnDisable()
    {
        aim.action?.Disable();
        aimMouse.action?.Disable();
    }

    private void Update()
    {
        if (!torsoPivot) return;

        // 1) Абсолютный ввод (правый стик/клавиши): задаёт МИРОВОЙ угол напрямую
        Vector2 abs = aim.action != null ? aim.action.ReadValue<Vector2>() : Vector2.zero;
        if (abs.sqrMagnitude >= deadzone * deadzone)
        {
            // (0,1) — world +Z; (1,0) — world +X
            _targetWorldYaw = Mathf.Atan2(abs.x, abs.y) * Mathf.Rad2Deg;
        }
        else
        {
            // 2) Если абсолютного ввода нет — берём мышь (delta) и инкрементально добавляем yaw
            Vector2 md = aimMouse.action != null ? aimMouse.action.ReadValue<Vector2>() : Vector2.zero;
            if (md.sqrMagnitude > 0f)
            {
                _targetWorldYaw += md.x * mouseYawPerPixel;     // только горизонт
                // Нормализуем угол, чтобы не рос без конца
                _targetWorldYaw = Mathf.Repeat(_targetWorldYaw, 360f);
            }
        }

        // Плавно доворачиваем башню к МИРОВОМУ углу (держим курс, ноги могут крутиться независимо)
        Quaternion desiredWorld = Quaternion.Euler(0f, _targetWorldYaw, 0f);
        torsoPivot.rotation = Quaternion.RotateTowards(torsoPivot.rotation, desiredWorld, rotateSpeed * Time.deltaTime);
    }
}
