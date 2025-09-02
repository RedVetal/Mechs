// MachineGunWeapon.cs
using UnityEngine;

public class MachineGunWeapon : WeaponBase
{
    [Header("Machine Gun")]
    [SerializeField] private float bulletSpeed = 48f;

    [Header("Model (MG Defaults)")]
    [SerializeField] private float mgShotsPerSecond = 10f;
    [SerializeField] private int mgMagazineSize = 20;
    [SerializeField] private float mgReloadSeconds = 1.6f;

    [Header("Pooling")]
    [SerializeField] private ProjectilePool pool; // ссылка на пул в сцене

    protected override void Awake()
    {
        base.Awake();
        ConfigureModel(
            shotsPerSec: mgShotsPerSecond,
            magSize: mgMagazineSize,
            reloadSec: mgReloadSeconds,
            holdToFire: true
        );
    }

    protected override void DoFire()
    {
        if (!EnsureMuzzle() || !pool) return;

        var rb = pool.Spawn(muzzle.position, muzzle.rotation);
        rb.linearVelocity = muzzle.forward * bulletSpeed;
    }
}
