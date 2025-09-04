using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BulletDamage : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    public GameObject Owner { get; set; }

    void OnCollisionEnter(Collision c) => Hit(c.collider, c.GetContact(0).point);
    void OnTriggerEnter(Collider other) => Hit(other, transform.position);

    void Hit(Collider col, Vector3 point)
    {
        var h = col.GetComponent<IDamageable>() ?? col.GetComponentInParent<IDamageable>();
        if (h != null && h.IsAlive)
        {
            h.TakeDamage(new DamageInfo
            {
                Source = Owner != null ? Owner : gameObject,
                HitPoint = point,
                Amount = damage,
                Type = DamageType.Bullet,
                Impulse = transform.forward * 2f
            });
        }

        var life = GetComponent<PooledLifetime>();
        if (life != null) life.ReturnToPool();
        else gameObject.SetActive(false);
    }
}
