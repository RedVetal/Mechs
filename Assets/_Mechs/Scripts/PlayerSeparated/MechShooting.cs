using UnityEngine;
using UnityEngine.InputSystem;

public class MechShooting : MonoBehaviour
{
    // In Inspector: drag the Action Reference **Gameplay/Fire (Button)** here.
    [Header("Input")]
    [Tooltip("Input Action Reference: Gameplay/Fire (Button). Drag from your .inputactions asset.")]
    [SerializeField] private InputActionProperty fire;

    [Header("References")]
    [Tooltip("Spawn point at the end of the gun. Its blue Z axis must point along the firing direction.")]
    [SerializeField] private Transform muzzle;

    [Tooltip("Projectile prefab with Rigidbody + Collider. Drag from Project (not from Scene).")]
    [SerializeField] private Rigidbody projectilePrefab;

    [Header("Firing Settings")]
    [Tooltip("Initial velocity of the projectile along Muzzle.forward (m/s).")]
    [SerializeField] private float projectileSpeed = 24f;

    [Tooltip("Cooldown between shots (sec). Use 0 during debugging if needed.")]
    [Min(0f)]
    [SerializeField] private float fireCooldown = 0.12f;

    private float _cd;

    private void OnEnable() => fire.action?.Enable();
    private void OnDisable() => fire.action?.Disable();

    private void Update()
    {
        _cd -= Time.deltaTime;
        var a = fire.action; if (a == null) return;

        // Срабатывает один раз в кадр на «нажатие»: Space / LMB / <Gamepad>/buttonSouth / On-Screen Button.
        if (_cd <= 0f && a.WasPerformedThisFrame())
        {
            Shoot();
            _cd = fireCooldown;
        }
    }

    private void Shoot()
    {
        // Базовые проверки: должна быть точка Muzzle и префаб снаряда с Rigidbody.
        if (!muzzle || !projectilePrefab) return;

        // Создаём копию префаба в позиции/ориентации Muzzle.
        Rigidbody rb = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        // Придаём скорость вдоль мировой оси Z у Muzzle (т.е. по дула).
        rb.linearVelocity = muzzle.forward * projectileSpeed;

        // Подсказки:
        //  - Хотите «лазер» без падения? Выключите Use Gravity в Rigidbody префаба.
        //  - Жизненный цикл пули (самоудаление по таймеру/столкновению) добавим позже.
    }
}
