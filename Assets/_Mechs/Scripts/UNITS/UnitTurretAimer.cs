// Assets/_Mechs/Scripts/ENEMIES_V2/UnitTurretAimer.cs
using UnityEngine;

[DisallowMultipleComponent]
public class UnitTurretAimer : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private UnitTargetSensor targetProvider;

    [Header("Aiming")]
    [SerializeField] private bool onlyYaw = true;
    [SerializeField] private float yawSpeed = 540f;

    public float AimErrorDeg { get; private set; }

    void Reset()
    {
        if (targetProvider == null)
            targetProvider = GetComponentInParent<UnitTargetSensor>();
    }

    void Update()
    {
        if (targetProvider == null || !targetProvider.HasTarget) { AimErrorDeg = 999f; return; }

        Vector3 to = targetProvider.Target.position - transform.position;
        if (onlyYaw) to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) { AimErrorDeg = 0f; return; }

        var desired = Quaternion.LookRotation(to.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, yawSpeed * Time.deltaTime);

        Vector3 fwd = transform.forward; if (onlyYaw) fwd.y = 0f;
        AimErrorDeg = Vector3.Angle(fwd, to);
    }

    public bool IsAimedWithin(float fovDeg) => AimErrorDeg <= fovDeg;
}
