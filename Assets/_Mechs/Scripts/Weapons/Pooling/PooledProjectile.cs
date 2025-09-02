using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PooledProjectile : MonoBehaviour
{
    [SerializeField] private float lifeSeconds = 4f;
    private float t;
    private Rigidbody rb;
    private ProjectilePool pool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pool = GetComponentInParent<ProjectilePool>();
    }

    private void OnEnable()
    {
        t = lifeSeconds;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        t -= Time.deltaTime;
        if (t <= 0f)
            ReturnToPool();
    }

    private void OnCollisionEnter(Collision _) => ReturnToPool();
    private void OnTriggerEnter(Collider _) => ReturnToPool();

    private void ReturnToPool()
    {
        if (pool != null) pool.Despawn(rb);
        else gameObject.SetActive(false);
    }
}
