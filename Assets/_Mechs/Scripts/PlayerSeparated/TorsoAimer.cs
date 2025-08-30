using UnityEngine;
using UnityEngine.InputSystem;

public class TorsoAimer : MonoBehaviour
{
    [Header("Input (assign from .inputactions)")]
    public InputActionProperty aim; // Gameplay/Aim (Vector2): JIKL и/или <Gamepad>/rightStick

    [Header("Refs")]
    public Transform torsoPivot; // узел башни (child от Mech)

    [Header("Tuning")]
    public float rotateSpeed = 360f;   // град/с
    public float deadzone = 0.15f;   // мЄртва€ зона стика

    // целевой ћ»–ќ¬ќ… угол башни (в градусах)
    private float _targetWorldYaw;

    void Awake()
    {
        if (!torsoPivot) torsoPivot = transform;
        // стартуем с текущего мирового угла башни
        _targetWorldYaw = torsoPivot.rotation.eulerAngles.y;
    }

    void OnEnable() { var a = aim.action; if (a != null) a.Enable(); }
    void OnDisable() { var a = aim.action; if (a != null) a.Disable(); }

    void Update()
    {
        if (!torsoPivot) return;

        var a = aim.action;
        Vector2 v = (a != null) ? a.ReadValue<Vector2>() : Vector2.zero;

        // ≈сли пользователь двигает правый стик/клавиши Ч обновл€ем ÷≈Ћ№ (в мировых ос€х XZ)
        if (v.sqrMagnitude >= deadzone * deadzone)
        {
            // (0,1) Ч мировое +Z, (1,0) Ч мировое +X
            // ¬нимание: это не camera-relative, а world-relative Ч так проще и стабильно
            _targetWorldYaw = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        }

        // ѕоворачиваем башню к ћ»–ќ¬ќ… цели.
        // ƒаже если ввода нет Ч продолжаем держать направление,
        // компенсиру€ повороты базы.
        Quaternion desiredWorld = Quaternion.Euler(0f, _targetWorldYaw, 0f);
        torsoPivot.rotation = Quaternion.RotateTowards(
            torsoPivot.rotation, desiredWorld, rotateSpeed * Time.deltaTime);
    }
}
