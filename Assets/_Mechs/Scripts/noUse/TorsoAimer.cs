//using UnityEngine;
//using UnityEngine.InputSystem;

//public class TorsoAimer : MonoBehaviour
//{
//    [Header("Input (assign from .inputactions)")]
//    public InputActionProperty aim; // Gameplay/Aim

//    [Header("Refs")]
//    public Transform torsoPivot; // узел верхней части (child от базы)

//    [Header("Tuning")]
//    public float rotateSpeed = 360f;
//    public float deadzone = 0.15f;

//    void OnEnable() { var a = aim.action; if (a != null) a.Enable(); }
//    void OnDisable() { var a = aim.action; if (a != null) a.Disable(); }

//    void Update()
//    {
//        if (!torsoPivot) return;
//        var a = aim.action; if (a == null) return;

//        Vector2 v = a.ReadValue<Vector2>();
//        if (v.sqrMagnitude < deadzone * deadzone) return;

//        // (0,1) Ч ЂвперЄд по базеї: считаем локальный yaw
//        float targetYaw = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
//        Quaternion desired = Quaternion.Euler(0f, targetYaw, 0f);

//        torsoPivot.localRotation = Quaternion.RotateTowards(
//            torsoPivot.localRotation, desired, rotateSpeed * Time.deltaTime);
//    }
//}
