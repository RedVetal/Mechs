using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class MechMovement : MonoBehaviour
{
    [Header("Input (assign from .inputactions)")]
    public InputActionProperty move; // Перетащи сюда Gameplay/Move (Vector2)

    [Header("Tuning")]
    public float moveSpeed = 6f;    // м/с
    public float turnSpeed = 540f;  // град/с — скорость разворота к направлению
    public float deadzone = 0.10f;  // мёртвая зона стика

    private Rigidbody _rb;

    private void Awake()
    {
        // Находим единственный RB меха: на себе → у родителей → на корне
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) _rb = GetComponentInParent<Rigidbody>();
        if (_rb == null && transform.root != null) _rb = transform.root.GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogError("[MechMovement] Rigidbody не найден. Поставь RB на корень меха и помести этот объект внутрь.");
            enabled = false;
            return;
        }

        // Базовые настройки устойчивости
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        var a = move.action; if (a != null) a.Enable();
    }

    private void OnDisable()
    {
        var a = move.action; if (a != null) a.Disable();
    }

    private void FixedUpdate()
    {
        var a = move.action; if (a == null) return;

        Vector2 v = a.ReadValue<Vector2>();
        float mag = v.magnitude;
        if (mag < deadzone) return;

        // Направление в мировых XZ: (x,y) стика → (x, z) движения
        Vector3 dir = new Vector3(v.x, 0f, v.y).normalized;

        // Повернуть корпус к направлению
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));

        // Ехать в этом направлении (скорость можно масштабировать силой наклона стика)
        float speed = moveSpeed * Mathf.Clamp01(mag);
        _rb.MovePosition(_rb.position + dir * (speed * Time.fixedDeltaTime));
    }
}
