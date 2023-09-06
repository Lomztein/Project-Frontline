using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class FirstPersonWeaponsController : MonoBehaviour, ITeamComponent
{
    public GameObject[] WeaponSlots;
    public IWeapon[] Weapons;
    public GameObject[] WeaponPrefabs;

    public StarterAssetsInputs Inputs;
    private TeamInfo _team;
    private LayerMask _targetLayer;

    private void Awake()
    {
        UpdateWeapons();
    }

    public void SetWeapon(int index, GameObject prefab)
    {
        if (WeaponSlots[index].transform.childCount > 0)
        {
            var child = WeaponSlots[index].transform.GetChild(0);
            Destroy(child.gameObject);
        }
        GameObject newWeapon = Instantiate(prefab, WeaponSlots[index].transform);
        newWeapon.SetActive(true);
        _team.ApplyTeam(newWeapon);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = Vector3.one;
        Destroy(newWeapon.GetComponent<InfantryWeaponTurret>());

        Weapons[index] = newWeapon.GetComponent<IWeapon>();
    }

    private void FixedUpdate()
    {
        bool[] fire = new bool[]
        {
            Inputs.PrimaryFire, Inputs.SecondaryFire,
        };

        float len = Mathf.Min(fire.Length, Weapons.Length);
        for (int i = 0; i < len; i++)
        {
            if (Weapons[i] != null && fire[i])
            {
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                ITarget target = new PositionTarget(ray.GetPoint(1000));
                Vector3 weaponLookAt = target.GetCenter(); ;

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _targetLayer))
                {
                    target = hit.collider.gameObject.CompareTag("Terrain") ? target : new ColliderTarget(hit.collider);
                    weaponLookAt = hit.point;
                }

                Vector3 dir = (weaponLookAt - WeaponSlots[i].transform.position).normalized;
                WeaponSlots[i].transform.rotation = Quaternion.Slerp(WeaponSlots[i].transform.rotation, Quaternion.LookRotation(dir), Time.fixedDeltaTime);
                Weapons[i].TryFire(target);
            }
        }
    }

    public void UpdateWeapons ()
    {
        Weapons = new IWeapon[WeaponPrefabs.Length];
        int min = Mathf.Min(Weapons.Length, WeaponSlots.Length);
        for (int i = 0; i < min; i++)
        {
            if (WeaponPrefabs[i])
            {
                SetWeapon(i, WeaponPrefabs[i]);
                WeaponPrefabs[i] = null;
            }
        }
    }

    public void SetTeam(TeamInfo team)
    {
        _team = team;
        _targetLayer = team.GetOtherLayerMasks();
    }
}
