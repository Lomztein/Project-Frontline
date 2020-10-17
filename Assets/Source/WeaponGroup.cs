using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponGroup : MonoBehaviour, IWeapon
{
    public GameObject[] Weapons;
    private IWeapon[] _weapons;
    private IFireControl _fireControl = new NoFireControl();

    public event Action OnFire;

    private void Start()
    {
        _weapons = Weapons.Select(x => x.GetComponent<IWeapon>()).ToArray();
        IFireControl replacement = GetComponent<IFireControl>();
        if (replacement != null)
        {
            _fireControl = replacement;
        }

        foreach (IWeapon weapon in _weapons)
        {
            weapon.OnFire += OnWeaponFire;
        }
    }

    private void OnWeaponFire()
    {
        OnFire?.Invoke();
    }

    public bool CanFire() => _weapons.All(x => x.CanFire());

    public void TryFire()
    {
        if (CanFire())
        {
            _fireControl.Fire(_weapons.Length, FireControlCallback);
        }
    }

    private void FireControlCallback(int index)
    {
        _weapons[index].TryFire();
    }

    private class NoFireControl : IFireControl
    {
        public void Fire(int amount, Action<int> callback)
        {
            for (int i = 0; i < amount; i++)
            {
                callback(i);
            }
        }
    }
}
