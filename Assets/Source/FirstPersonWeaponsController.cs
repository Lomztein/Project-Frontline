using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Util;

public class FirstPersonWeaponsController : MonoBehaviour, ITeamComponent
{
    public GameObject[] WeaponSlots;
    public IWeapon[] Weapons;
    public WeaponInfo[] WeaponsInfo;
    public GameObject[] WeaponPrefabs;

    public StarterAssetsInputs Inputs;
    private TeamInfo _team;
    private LayerMask _targetLayer;

    [SerializeReference, SR]
    public WeaponAimBehaviour DefaultAimBehaviour;

    public int WeaponLayer;
    public AnimationCurve AimCurve;
    public float AimTime;
    private float _aimFactor;

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
        newWeapon.transform.SetLayerRecursively(WeaponLayer);
        Destroy(newWeapon.GetComponent<InfantryWeaponTurret>());

        Weapons[index] = newWeapon.GetComponent<IWeapon>();
        WeaponsInfo[index] = newWeapon.GetComponent<WeaponInfo>();
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

        if (Weapons.Length == 1)
        {
            WeaponAimBehaviour behaviour = DefaultAimBehaviour;
            if (WeaponsInfo[0] && WeaponsInfo[0].AimBehaviour != null) behaviour = WeaponsInfo[0].AimBehaviour;

            if (fire[1])
                _aimFactor += Time.fixedDeltaTime / AimTime;
            else
                _aimFactor -= Time.fixedDeltaTime / AimTime;
            _aimFactor = Mathf.Clamp01(_aimFactor);

            float t = AimCurve.Evaluate(_aimFactor);
            behaviour.SetAim(Weapons[0], t);
        }
    }

    public void UpdateWeapons ()
    {
        Weapons = new IWeapon[WeaponPrefabs.Length];
        WeaponsInfo = new WeaponInfo[WeaponPrefabs.Length];
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
