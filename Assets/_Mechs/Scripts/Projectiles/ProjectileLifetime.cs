// ProjectileLifetime.cs
using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    [SerializeField, Min(0f)] private float lifeSeconds = 4f;
    private float t;

    private void OnEnable() { t = lifeSeconds; }
    private void Update() { t -= Time.deltaTime; if (t <= 0f) Destroy(gameObject); }

    private void OnCollisionEnter(Collision _) { Destroy(gameObject); }
    private void OnTriggerEnter(Collider _) { Destroy(gameObject); }
}
