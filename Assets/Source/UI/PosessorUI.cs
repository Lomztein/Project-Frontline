using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class PosessorUI : MonoBehaviour
{
    public Posessor Posessor;

    public GameObject PosessedUnit;
    private Health _health;

    public Slider HealthSlider;
    public Transform WeaponUIParent;
    public GameObject WeaponUIPrefab;

    private void Start()
    {
        if (PosessedUnit)
        {
            Assign(PosessedUnit);
        }
        Posessor.OnPosess += Posessor_OnPosess;
        Posessor.OnRelease += Posessor_OnRelease;
        gameObject.SetActive(false);
    }

    private void Posessor_OnRelease()
    {
        foreach (Transform child in WeaponUIParent)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }

    private void Posessor_OnPosess(GameObject obj)
    {
        Assign(obj);
    }

    private void FixedUpdate()
    {
        if (PosessedUnit)
        {
            HealthSlider.value = _health.CurrentHealth;
        }
    }

    public void Assign(GameObject unit)
    {
        gameObject.SetActive(true);
        PosessedUnit = unit;
        _health = unit.GetComponent<Health>();
        HealthSlider.maxValue = _health.MaxHealth;
        GenerateWeaponUI(unit.GetComponent<Unit>().GetWeapons());
    }

    private void GenerateWeaponUI(IEnumerable<IWeapon> weapons)
    {
        foreach (var weapon in weapons)
        {
            if (weapon is Component comp)
            {
                string name = comp.name;
                string desc = "";
                string stats = $"{weapon.GetDPSOrOverride()} {weapon.DamageType}-type DPS";
                Texture2D tex;
                if (comp.TryGetComponent(out WeaponInfo info))
                {
                    if (info.Root)
                    {
                        tex = Iconography.GenerateIcon(info.Root, Quaternion.Euler(0f, 90f, 0f), 512);
                    }
                    else
                    {
                        tex = Iconography.GenerateIcon(comp.gameObject, Quaternion.Euler(0f, 90f, 0f), 512);
                    }
                    desc = info.Description;
                    name = info.Name;
                }
                else
                {
                    tex = Iconography.GenerateIcon(comp.gameObject, Quaternion.Euler(0f, 90f, 0f), 512);
                }
                
                GameObject newWeaponUI = Instantiate(WeaponUIPrefab, WeaponUIParent);
                tex = UnityUtils.TrimTransparent(tex);
                Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.one / 2f);
                newWeaponUI.transform.Find("Info/Profile/Image").GetComponent<Image>().sprite = sprite;
                newWeaponUI.transform.Find("Info/Profile/Image").GetComponent<Image>().preserveAspect = true;
                newWeaponUI.transform.Find("Info/Description/Name").GetComponent<TextMeshProUGUI>().text = name;
                newWeaponUI.transform.Find("Info/Description/Description").GetComponent<TextMeshProUGUI>().text = desc;
                newWeaponUI.transform.Find("Info/Description/Stats").GetComponent<TextMeshProUGUI>().text = stats;
                newWeaponUI.transform.Find("Status").GetComponent<PosessorUIWeaponStatus>().Assign(weapon);
            }
        }
    }
}
