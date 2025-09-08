// Assets/_Mechs/Scripts/ENEMIES_V2/UnitWeaponDriver.cs
using UnityEngine;

[DisallowMultipleComponent]
public class UnitWeaponDriver : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private UnitTargetSensor targetProvider;
    [SerializeField] private UnitTurretAimer aimInfo;         // можно оставить пустым дл€ Ђбезбашенныхї
    [SerializeField] private WeaponBase weapon;          // Cannon/MG
    [SerializeField] private Transform rayOrigin;       // дл€ LoS; если пусто Ч transform

    [Header("Fire logic")]
    [SerializeField] private float fireRange = 18f;
    [SerializeField] private float fireFOV = 20f;            // требуема€ точность (если есть aimInfo)
    [SerializeField] private bool requireLineOfSight = false;
    [SerializeField] private LayerMask losMask = default;

    void Reset()
    {
        if (targetProvider == null) targetProvider = GetComponentInParent<UnitTargetSensor>();
        if (rayOrigin == null) rayOrigin = transform;
    }

    void Start()
    {
        // синхронизируем дистанцию движени€, если на корне есть мотор
        var motor = GetComponentInParent<UnitBodyMotor>();
        if (motor != null) motor.SetDesiredRange(fireRange);
    }

    void Update()
    {
        bool wantFire = false;

        if (weapon != null && targetProvider != null && targetProvider.HasTarget)
        {
            float dist = targetProvider.Distance;

            if (dist <= fireRange)
            {
                bool aimed = (aimInfo == null) || aimInfo.IsAimedWithin(fireFOV);
                bool hasLoS = !requireLineOfSight || ClearLoS(targetProvider.Target);

                wantFire = aimed && hasLoS;
            }
        }

        if (weapon != null)
            weapon.Tick(wantFire, Time.deltaTime);
    }

    bool ClearLoS(Transform t)
    {
        var origin = rayOrigin != null ? rayOrigin.position : transform.position;
        return !Physics.Linecast(origin, t.position, losMask, QueryTriggerInteraction.Ignore);
    }

    public void SetFireRange(float value) => fireRange = Mathf.Max(0f, value);
}
