using System;

public interface IWeapon
{
    event Action OnFire;

    bool CanFire();
    void TryFire(ITarget intendedTarget);
}