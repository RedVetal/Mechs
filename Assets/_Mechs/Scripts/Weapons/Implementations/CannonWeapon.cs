// CannonWeapon.cs
using UnityEngine;

public class CannonWeapon : WeaponBase
{
    [Header("Cannon")]
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float muzzleSpeed = 24f;
    [SerializeField] private Vector3 extraUp = new Vector3(0, 2.5f, 0); // чуть вверх для дуги

    protected override void Awake()
    {
        base.Awake();
        // Пушка: одиночные выстрелы, без магазина и без перезарядки
        // shotsPerSecond = 1 / fireCooldown. Например cooldown 1.0s → 1 выстрел/сек.
        ConfigureModel(shotsPerSec: 1f, magSize: 1, reloadSec: 0f, holdToFire: false);
    }

    protected override void DoFire()
    {
        if (!EnsureMuzzle() || !projectilePrefab) return;

        Rigidbody rb = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        rb.linearVelocity = muzzle.forward * muzzleSpeed + extraUp;

        var exp = rb.GetComponent<ProjectileExplosionDamage>();
        if (exp != null) exp.Owner = owner;
    }

}
