using UnityEngine;

public class ProjectileExplosionDamage : MonoBehaviour
{
    [SerializeField] float baseDamage = 60f;
    [SerializeField] float radius = 4f;
    [SerializeField] float impulse = 6f;

    public GameObject Owner { get; set; }
    bool exploded;

    void OnCollisionEnter(Collision c) { if (!exploded) Explode(c.GetContact(0).point); }
    void OnTriggerEnter(Collider _) { if (!exploded) Explode(transform.position); }

    void Explode(Vector3 center)
    {
        exploded = true;

        var hits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide);
        foreach (var col in hits)
        {
            var dmg = col.GetComponent<IDamageable>() ?? col.GetComponentInParent<IDamageable>();
            if (dmg != null && dmg.IsAlive)
            {
                float dist = Vector3.Distance(center, col.ClosestPoint(center));
                float t = Mathf.Clamp01(dist / Mathf.Max(0.001f, radius));
                float amount = Mathf.Lerp(baseDamage, 0f, t);

                dmg.TakeDamage(new DamageInfo
                {
                    Source = Owner != null ? Owner : gameObject,
                    HitPoint = col.ClosestPoint(center),
                    Amount = amount,
                    Type = DamageType.Cannon,   // можно поменять на Explosion при желании
                    Impulse = (col.transform.position - center).normalized * impulse
                });
            }

            if (col.attachedRigidbody)
                col.attachedRigidbody.AddExplosionForce(impulse * 50f, center, radius, 0f, ForceMode.Impulse);
        }

        Destroy(gameObject); // снаряд НЕ из пула
    }
}
