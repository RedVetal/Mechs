// HealthComponent.cs
// Повесь HealthComponent на всё, что должно получать урон (мехи, враги, строения).

using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    public float Current { get; private set; }
    public bool IsAlive => Current > 0f;

    public System.Action<float, float> OnHealthChanged; // (cur,max)
    public System.Action OnDeath;

    private void Awake() => Current = maxHealth;

    public void TakeDamage(DamageInfo info)
    {
        if (!IsAlive) return;
        Current = Mathf.Max(0f, Current - info.Amount);
        OnHealthChanged?.Invoke(Current, maxHealth);
        if (Current <= 0f) OnDeath?.Invoke();

        // Импульс по желанию
        if (info.Impulse != Vector3.zero && TryGetComponent<Rigidbody>(out var rb))
            rb.AddForce(info.Impulse, ForceMode.Impulse);
    }
}
