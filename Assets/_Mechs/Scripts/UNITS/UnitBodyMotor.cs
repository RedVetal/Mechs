// Assets/_Mechs/Scripts/ENEMIES_V2/UnitBodyMotor.cs
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class UnitBodyMotor : MonoBehaviour
{
    public enum BodyRotationMode { None, FaceTarget, FaceMovement }
    public enum StrafeMode { None, Left, Right }

    [Header("Links")]
    [SerializeField] private UnitTargetSensor targetProvider;
    [SerializeField] private NavMeshAgent agent;

    [Header("Range control")]
    [SerializeField] private float desiredRange = 15f;
    [SerializeField] private float rangeSlack = 1.5f;

    [Header("Motion")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float moveSpeed = 5f;    // прокинем в agent.speed
    [SerializeField] private float turnSpeed = 360f;  // прокинем в agent.angularSpeed

    [Header("Body rotation")]
    [SerializeField] private BodyRotationMode rotationMode = BodyRotationMode.FaceMovement;

    [Header("Strafe / Orbit")]
    [SerializeField] private StrafeMode strafe = StrafeMode.Right;
    [SerializeField] private float lateralOffset = 4f;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        if (targetProvider == null) targetProvider = GetComponent<UnitTargetSensor>();
    }

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        agent.stoppingDistance = 0.25f; // маленькое, для стабильного pathing
        agent.updateRotation = (rotationMode == BodyRotationMode.FaceMovement);
        agent.autoBraking = false; // плавнее орбита
    }

    void Update()
    {
        if (!canMove || agent == null) return;

        var hasTarget = targetProvider != null && targetProvider.HasTarget;
        if (!hasTarget)
        {
            agent.isStopped = true;
            return;
        }

        var tgt = targetProvider.Target;
        Vector3 to = tgt.position - transform.position;
        float d = to.magnitude;
        if (d < 0.001f) { agent.isStopped = true; return; }

        Vector3 dir = to / d;
        Vector3 lat = Vector3.zero;
        if (strafe != StrafeMode.None)
        {
            lat = (strafe == StrafeMode.Left)
                ? Vector3.Cross(Vector3.up, dir).normalized
                : Vector3.Cross(dir, Vector3.up).normalized;
        }

        Vector3 ringPoint = tgt.position - dir * desiredRange;
        Vector3 dest;

        if (d > desiredRange + rangeSlack) dest = ringPoint + lat * (lateralOffset * 0.5f);
        else if (d < desiredRange - rangeSlack) dest = ringPoint + lat * (lateralOffset * 0.5f);
        else dest = ringPoint + lat * lateralOffset;

        agent.isStopped = false;

        if (NavMesh.SamplePosition(dest, out var hit, 1.5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(dest);

        if (rotationMode == BodyRotationMode.FaceTarget)
        {
            Vector3 planar = dir; planar.y = 0f;
            if (planar.sqrMagnitude > 0.001f)
            {
                var toRot = Quaternion.LookRotation(planar);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, turnSpeed * Time.deltaTime);
            }
        }
        else if (rotationMode == BodyRotationMode.None)
        {
            agent.updateRotation = false; // не вращаем корпус вообще
        }
        else // FaceMovement
        {
            agent.updateRotation = true;
        }
    }

    // Удобно синхронизировать из драйвера оружия
    public void SetDesiredRange(float value)
    {
        desiredRange = Mathf.Max(0f, value);
        // stoppingDistance не трогаем
    }

    void OnValidate()
    {
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = turnSpeed;
        }
    }
}
