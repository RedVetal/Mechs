using UnityEngine;
using UnityEngine.InputSystem;

public class MechShooting : MonoBehaviour
{
    [Header("Input (assign from .inputactions)")]
    public InputActionProperty fire; // Gameplay/Fire

    [Header("Refs")]
    public Transform muzzle;            // точка вылета снаряда (child у ствола/торса)
    public Rigidbody projectilePrefab;  // префаб с Rigidbody + Collider

    [Header("Tuning")]
    public float projectileSpeed = 24f;
    public float fireCooldown = 0.12f;

    float _cd;

    void OnEnable() { var a = fire.action; if (a != null) a.Enable(); }
    void OnDisable() { var a = fire.action; if (a != null) a.Disable(); }

    void Update()
    {
        _cd -= Time.deltaTime;
        var a = fire.action; if (a == null) return;

        if (_cd <= 0f && a.WasPerformedThisFrame())
        {
            Shoot();
            _cd = fireCooldown;
        }
    }

    void Shoot()
    {
        if (!muzzle || !projectilePrefab) return;
        var rb = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        rb.linearVelocity = muzzle.forward * projectileSpeed;
    }
}
