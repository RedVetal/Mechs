// IWeapon.cs
using UnityEngine;

public enum WeaponState { Ready, Firing, Reloading }

public interface IWeapon
{
    string Name { get; }
    Sprite Icon { get; }
    WeaponState State { get; }
    int AmmoCount { get; }
    int AmmoMax { get; }

    // Вызывается менеджером КАЖДЫЙ кадр: fireHeld=true, если кнопка огня зажата
    void Tick(bool fireHeld, float dt);

    // Хуки при выборе/снятии (под эффекты/анимки по желанию)
    void OnSelected();
    void OnDeselected();
}
