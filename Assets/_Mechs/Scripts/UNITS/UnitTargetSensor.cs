// Assets/_Mechs/Scripts/ENEMIES_V2/UnitTargetSensor.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitTargetSensor : MonoBehaviour
{
    public enum TargetMode { ByTag, ByTeam, NotMyTeam }

    [Header("Who to target")]
    [SerializeField] private TargetMode mode = TargetMode.ByTag;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private UnitTeamComponent myTeam;    // опционально (проставь на корне)
    [SerializeField] private UnitTeam targetTeam = UnitTeam.Player;

    [Header("Ranges")]
    [SerializeField] private float aggroRange = 25f;
    [SerializeField] private float loseRange = 35f;

    [Header("Vision")]
    [SerializeField] private float fieldOfView = 180f;   // градусы; 360 = всевидящий
    [SerializeField] private bool requireLineOfSight = false;
    [SerializeField] private LayerMask losMask = default; // препятствия

    [Header("Sampling")]
    [SerializeField] private float scanInterval = 0.25f;
    [SerializeField] private Transform eye;              // откуда "смотрим"; если пусто — transform

    float scanTimer;
    Transform current;
    float currentDistance = Mathf.Infinity;

    public Transform Target => current;
    public bool HasTarget => current != null;
    public float Distance => currentDistance;

    void Reset()
    {
        eye = transform;
        myTeam = GetComponent<UnitTeamComponent>();
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            TickSense();
        }
    }

    void TickSense()
    {
        if (current != null)
        {
            currentDistance = Vector3.Distance(transform.position, current.position);
            if (currentDistance > loseRange || !IsVisible(current))
            {
                current = null;
                currentDistance = Mathf.Infinity;
            }
        }

        if (current == null)
        {
            Transform best = FindBestCandidate(out float bestDist);
            if (best != null && bestDist <= aggroRange && IsVisible(best))
            {
                current = best;
                currentDistance = bestDist;
            }
        }
    }

    Transform FindBestCandidate(out float bestDist)
    {
        bestDist = Mathf.Infinity;
        Transform best = null;

        if (mode == TargetMode.ByTag)
        {
            var objs = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (var go in objs)
                Consider(go.transform, ref best, ref bestDist);
        }
        else
        {
            // ищем по командам
            var allTeams = FindObjectsOfType<UnitTeamComponent>();
            foreach (var ut in allTeams)
            {
                if (ut == null) continue;
                if (ut == myTeam) continue;
                bool ok =
                    (mode == TargetMode.ByTeam && ut.Team == targetTeam) ||
                    (mode == TargetMode.NotMyTeam && myTeam != null && ut.Team != myTeam.Team);

                if (!ok) continue;
                Consider(ut.transform, ref best, ref bestDist);
            }
        }
        return best;
    }

    void Consider(Transform t, ref Transform best, ref float bestDist)
    {
        if (t == null) return;
        float d = Vector3.Distance(transform.position, t.position);
        if (d < bestDist && InFOV(t))
        {
            best = t;
            bestDist = d;
        }
    }

    bool InFOV(Transform t)
    {
        if (fieldOfView >= 359.9f) return true;
        Vector3 to = (t.position - transform.position);
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) return true;
        float ang = Vector3.Angle(transform.forward, to);
        return ang <= fieldOfView * 0.5f;
    }

    bool IsVisible(Transform t)
    {
        if (!requireLineOfSight) return true;
        var origin = eye != null ? eye.position : transform.position;
        var target = t.position;
        return !Physics.Linecast(origin, target, losMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
