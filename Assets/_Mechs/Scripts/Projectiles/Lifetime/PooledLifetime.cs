using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PooledLifetime : MonoBehaviour
{
    [SerializeField] float lifeSeconds = 4f;

    float t;
    Rigidbody rb;
    ProjectilePool pool;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pool = GetComponentInParent<ProjectilePool>();
    }

    void OnEnable()
    {
        t = lifeSeconds;
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    }

    void Update()
    {
        t -= Time.deltaTime;
        if (t <= 0f) ReturnToPool();
    }

    void OnCollisionEnter(Collision _) => ReturnToPool();
    void OnTriggerEnter(Collider _) => ReturnToPool();

    public void ReturnToPool()
    {
        if (pool != null && rb != null) pool.Despawn(rb);
        else gameObject.SetActive(false);
    }
}
