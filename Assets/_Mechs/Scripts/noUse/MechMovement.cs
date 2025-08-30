//using UnityEngine;
//using UnityEngine.InputSystem;

//[DisallowMultipleComponent]
//public class MechMovement : MonoBehaviour
//{
//    [Header("Input (assign from .inputactions)")]
//    public InputActionProperty move; // Gameplay/Move

//    [Header("Tuning")]
//    public float moveSpeed = 6f;
//    public float turnSpeed = 120f;
//    public float deadzone = 0.1f;

//    Rigidbody _rb;
//    Transform _rbT;

//    void Awake()
//    {
//        // »щем RB на себе, иначе у родителей (корень меха)
//        _rb = GetComponent<Rigidbody>();
//        if (_rb == null) _rb = GetComponentInParent<Rigidbody>();

//        if (_rb == null)
//        {
//            Debug.LogError("[MechMovement] Rigidbody не найден на себе или у родителей. ѕоставь RB на корень меха.");
//            enabled = false; return;
//        }

//        _rbT = _rb.transform;
//        _rb.interpolation = RigidbodyInterpolation.Interpolate;
//        _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//    }

//    void OnEnable() { var a = move.action; if (a != null) a.Enable(); }
//    void OnDisable() { var a = move.action; if (a != null) a.Disable(); }

//    void FixedUpdate()
//    {
//        var a = move.action; if (a == null) return;
//        Vector2 v = a.ReadValue<Vector2>();
//        if (v.sqrMagnitude < deadzone * deadzone) v = Vector2.zero;

//        // “анкова€ схема: Y Ч вперЄд/назад, X Ч поворот на месте
//        if (v.y != 0f)
//        {
//            Vector3 delta = _rbT.forward * (v.y * moveSpeed * Time.fixedDeltaTime);
//            _rb.MovePosition(_rb.position + delta);
//        }

//        if (v.x != 0f)
//        {
//            float yaw = v.x * turnSpeed * Time.fixedDeltaTime;
//            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, yaw, 0f));
//        }
//    }
//}
