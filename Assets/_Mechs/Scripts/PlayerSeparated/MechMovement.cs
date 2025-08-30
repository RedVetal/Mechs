using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class MechMovement : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Input Action Reference: Gameplay/Move (Vector2). Drag from your .inputactions asset.")]
    [SerializeField] private InputActionProperty move;

    [Header("Movement Settings")]
    [Tooltip("Linear speed (m/s) at full stick tilt.")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("How fast body rotates to the movement direction (deg/sec).")]
    [SerializeField] private float turnSpeed = 540f;

    [Tooltip("Dead zone for the stick. Values below are ignored.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float deadzone = 0.10f;

    [Header("Physics (optional)")]
    [Tooltip("(Optional) Explicit mech root Rigidbody. Leave empty to auto-find on self/parents/root.")]
    [SerializeField] private Rigidbody rootRigidbody;

    private Rigidbody _rb;

    private void Awake()
    {
        // 1) Если явно указан rootRigidbody — используем его,
        // 2) иначе ищем на себе → у родителей (вкл. неактивных) → на корне
        _rb = rootRigidbody
           ?? GetComponent<Rigidbody>()
           ?? GetComponentInParent<Rigidbody>(true)
           ?? (transform.root != null ? transform.root.GetComponent<Rigidbody>() : null);

        if (_rb == null)
        {
            Debug.LogError("[MechMovement] Rigidbody not found. Put a single RB on the mech root and place this object under it (or assign 'rootRigidbody').");
            enabled = false; return;
        }

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable() => move.action?.Enable();
    private void OnDisable() => move.action?.Disable();

    private void FixedUpdate()
    {
        var a = move.action; if (a == null) return;
        Vector2 v = a.ReadValue<Vector2>();
        float mag = v.magnitude;
        if (mag < deadzone) return;

        Vector3 dir = new Vector3(v.x, 0f, v.y).normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));

        float speed = moveSpeed * Mathf.Clamp01(mag);
        _rb.MovePosition(_rb.position + dir * (speed * Time.fixedDeltaTime));
    }
}
