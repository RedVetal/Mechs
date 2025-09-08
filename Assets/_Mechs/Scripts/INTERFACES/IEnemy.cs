// Scripts/Enemies/Core/IEnemy.cs
using UnityEngine;

public interface IEnemy
{
    bool IsAlive { get; }
    Transform Target { get; }

    // Поиск/обновление цели (можно вызывать раз в кадр или реже таймером)
    void AcquireTarget();
}
