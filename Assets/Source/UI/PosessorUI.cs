using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class PosessorUI : MonoBehaviour
{
    public static PosessorUI Instance;

    public Posessor Posessor;

    public GameObject PosessedUnit;
    private Health _health;

    public Slider HealthSlider;
    public Transform WeaponUIParent;
    public GameObject WeaponUIPrefab;
    public Image TargetingImage;
    public Text[] WeaponAmmoTexts;

    private void Awake()
    {
        Instance = this;
    }

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

    private void Posessor_OnRelease(GameObject obj)
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
        _health = unit.GetComponentInChildren<Health>();
        HealthSlider.maxValue = _health.MaxHealth;
        FirstPersonWeaponsController fpw = unit.GetComponentInParent<FirstPersonWeaponsController>();
        if (fpw != null)
        {
            StartCoroutine(DelayedGenerateUI(fpw.Weapons));
        }
        else
        {
            GenerateWeaponUI(unit.GetComponent<AIController>().Weapons);
        }
    }

    private IEnumerator DelayedGenerateUI(IEnumerable<IWeapon> weapons)
    {
        yield return new WaitForFixedUpdate();
        GenerateWeaponUI(weapons);
    }

    private void GenerateWeaponUI(IEnumerable<IWeapon> weapons)
    {
        foreach (var text in WeaponAmmoTexts)
            text.text = "";

        int index = 0;
        foreach (var weapon in weapons)
        {
            if (weapon is Component comp)
            {
                string name = comp.name;
                string desc = "";
                string stats = $"{weapon.GetDPSOrOverride()} {weapon.Modifier} DPS";
                Texture2D tex;
                GameObject model;

                if (comp.TryGetComponent(out WeaponInfo info))
                {
                    if (info.Model)
                    {
                        model = info.Model;
                    }
                    else
                    {
                        model = comp.gameObject;
                    }

                    desc = info.Description;
                    name = info.Name;
                }
                else
                {
                    model = comp.gameObject;
                }

                tex = Iconography.GenerateIcon(model, Quaternion.Euler(0f, 90f, 0f), 256, null);

                GameObject newWeaponUI = Instantiate(WeaponUIPrefab, WeaponUIParent);
                tex = UnityUtils.TrimTransparent(tex);
                Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.one / 2f);
                newWeaponUI.transform.Find("Info/Profile/Image").GetComponent<Image>().sprite = sprite;
                newWeaponUI.transform.Find("Info/Profile/Image").GetComponent<Image>().preserveAspect = true;
                newWeaponUI.transform.Find("Info/Description/Name").GetComponent<TextMeshProUGUI>().text = name;
                newWeaponUI.transform.Find("Info/Description/Description").GetComponent<TextMeshProUGUI>().text = desc;
                newWeaponUI.transform.Find("Info/Description/Stats").GetComponent<TextMeshProUGUI>().text = stats;
                newWeaponUI.transform.Find("Status").GetComponent<PosessorUIWeaponStatus>().Assign(weapon, index);
                index++;
            }
        }
    }
}
