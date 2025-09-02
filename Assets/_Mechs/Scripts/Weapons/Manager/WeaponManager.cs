// WeaponManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [Tooltip("Player/Fire (Button)")]
    [SerializeField] private InputActionProperty fireAction;

    [Tooltip("Player/SwitchWeapon (Button)")]
    [SerializeField] private InputActionProperty switchAction;

    [Header("Weapons")]
    [SerializeField] private List<WeaponBase> weapons = new(); // удобно в инспекторе
    private int currentIndex = 0;
    private IWeapon current;

    private void OnEnable()
    {
        fireAction.action?.Enable();
        switchAction.action?.Enable();
    }
    private void OnDisable()
    {
        fireAction.action?.Disable();
        switchAction.action?.Disable();
    }

    private void Start()
    {
        // фильтруем null и приводим к интерфейсу
        weapons.RemoveAll(w => w == null);
        if (weapons.Count > 0)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, weapons.Count - 1);
            current = weapons[currentIndex];
            (current as WeaponBase)?.OnSelected();
        }
        else
        {
            Debug.LogWarning("WeaponManager: weapons list is empty.");
        }
    }

    private void Update()
    {
        if (current == null) return;

        // 1) Переключение (по новому Input System)
        if (switchAction.action != null && switchAction.action.WasPerformedThisFrame())
            SwitchNext();

        // 2) Стрельба
        bool fireHeld = fireAction.action != null && fireAction.action.IsPressed();
        current.Tick(fireHeld, Time.deltaTime);
    }

    private void SwitchNext()
    {
        if (weapons.Count <= 1) return;

        (current as WeaponBase)?.OnDeselected();

        currentIndex = (currentIndex + 1) % weapons.Count;
        current = weapons[currentIndex];

        (current as WeaponBase)?.OnSelected();
        // тут же можно дернуть обновление UI (иконка/название/магазин)
    }

    // Опционально: публичные методы для UI
    public void SwitchWeaponUI() => SwitchNext();
    public string CurrentName => current?.Name ?? "";
    public int CurrentAmmo => current?.AmmoCount ?? 0;
    public int CurrentAmmoMax => current?.AmmoMax ?? 0;
    public WeaponState CurrentState => current?.State ?? WeaponState.Ready;
}
