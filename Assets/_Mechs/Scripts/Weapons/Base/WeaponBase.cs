// WeaponBase.cs
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Weapon";
    [SerializeField] private Sprite icon;

    [Header("Muzzle")]
    [SerializeField] protected Transform muzzle;

    [Header("Firing Model")]
    [Tooltip("Выстрелов в секунду при удержании. Для пушки обычно 1 / fireCooldown.")]
    [Min(0.01f)][SerializeField] private float shotsPerSecond = 1f;

    [Tooltip("Сколько выстрелов до авто-перезарядки. Для пушки = 1 (или 999, если без магазина).")]
    [SerializeField] private int magazineSize = 1;

    [Tooltip("Время авто-перезарядки после опустошения магазина. Для пушки обычно 0.")]
    [Min(0f)][SerializeField] private float reloadSeconds = 0f;

    [Tooltip("Нужно ли держать кнопку для непрерывной стрельбы.")]
    [SerializeField] private bool autoFire = false;

    // Runtime
    protected WeaponState state = WeaponState.Ready;
    private float cooldown;          // между выстрелами
    private int ammoInMag;
    private float reloadTimer;

    public string Name => displayName;
    public Sprite Icon => icon;
    public WeaponState State => state;
    public int AmmoCount => ammoInMag;
    public int AmmoMax => magazineSize;

    protected virtual void Awake()
    {
        ammoInMag = Mathf.Max(1, magazineSize);
        cooldown = 0f;
    }

    public virtual void OnSelected() { }
    public virtual void OnDeselected() { }

    public void Tick(bool fireHeld, float dt)
    {
        switch (state)
        {
            case WeaponState.Ready:
                if (cooldown > 0f) { cooldown -= dt; break; }

                bool wantShot = autoFire ? fireHeld : fireHeld; // единая кнопка; логика «по клику»/«по удержанию» задаётся shotsPerSecond/autoFire
                if (wantShot && ammoInMag > 0)
                {
                    DoFire();               // конкретный выстрел
                    ammoInMag--;
                    state = WeaponState.Firing;
                    cooldown = 1f / shotsPerSecond;
                }
                else if (ammoInMag == 0 && reloadSeconds > 0f)
                {
                    BeginReload();
                }
                break;

            case WeaponState.Firing:
                cooldown -= dt;
                if (cooldown <= 0f)
                {
                    // либо в Ready, либо сразу следующая пуля если держим и есть боеприпасы
                    state = WeaponState.Ready;
                }
                break;

            case WeaponState.Reloading:
                reloadTimer -= dt;
                if (reloadTimer <= 0f)
                {
                    ammoInMag = Mathf.Max(1, magazineSize);
                    state = WeaponState.Ready;
                }
                break;
        }

        // Авто-перезарядка, если магазин пуст и не задан reloadSeconds=0
        if (state == WeaponState.Ready && ammoInMag == 0 && reloadSeconds > 0f)
            BeginReload();
    }

    private void BeginReload()
    {
        state = WeaponState.Reloading;
        reloadTimer = reloadSeconds;
    }

    // Реализация конкретного выстрела
    protected abstract void DoFire();

    // Утилита: проверка дульной точки
    protected bool EnsureMuzzle()
    {
        if (muzzle) return true;
        Debug.LogWarning($"{name}: Muzzle is not assigned.");
        return false;
    }

    // Хелперы настройки модели стрельбы из инспектора
    protected void ConfigureModel(float shotsPerSec, int magSize, float reloadSec, bool holdToFire)
    {
        shotsPerSecond = shotsPerSec;
        magazineSize = magSize;
        reloadSeconds = reloadSec;
        autoFire = holdToFire;
    }
}
