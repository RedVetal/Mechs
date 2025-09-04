// DamageInfo.cs
using UnityEngine;

public enum DamageType { Bullet, Cannon, Rocket, Explosion }

public struct DamageInfo
{
    public GameObject Source;   // кто нанес (мех/оружие) Ч дл€ логов/ачивок
    public Vector3 HitPoint;    // точка попадани€
    public float Amount;        // числовой урон
    public DamageType Type;     // тип урона
    public Vector3 Impulse;     // импульс/отдача (можно Vector3.zero)
}
