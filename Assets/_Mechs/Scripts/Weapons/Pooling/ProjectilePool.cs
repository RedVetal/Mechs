// ProjectilePool.cs
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private int capacity = 40; // �� ���������

    private readonly Queue<Rigidbody> pool = new();

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < capacity; i++)
        {
            var rb = Instantiate(bulletPrefab, transform);
            rb.gameObject.SetActive(false);
            pool.Enqueue(rb);
        }
    }

    public Rigidbody Spawn(Vector3 pos, Quaternion rot)
    {
        Rigidbody rb = pool.Count > 0 ? pool.Dequeue() : Instantiate(bulletPrefab, transform);

        // важно: перед активацией отвязать от родителя
        rb.transform.SetParent(null, true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.transform.SetPositionAndRotation(pos, rot);
        rb.gameObject.SetActive(true);

        return rb;
    }

    public void Despawn(Rigidbody rb)
    {
        if (!rb) return;

        // вернуть под пул — для порядка в иерархии
        rb.gameObject.SetActive(false);
        rb.transform.SetParent(transform, true);

        pool.Enqueue(rb);
    }

}
