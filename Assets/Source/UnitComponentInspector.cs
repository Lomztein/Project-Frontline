using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New UnitComponentInspector", menuName = "ComponentInspector/Unit")]
public class UnitComponentInspector : ComponentInspector
{
    public GameObject UIPrefab;
    public GameObject StatDisplayPrefab;

    public override bool CanInspect(object obj)
        => obj is Unit;

    public override GameObject InstantiateInspectUI(object component)
    {
        GameObject newUIObject = Instantiate(UIPrefab);
        Unit unit = component as Unit;
        Transform statParent = newUIObject.transform.Find("Stats");
        foreach (var stat in unit.GetStats())
        {
            GameObject newStat = Instantiate(StatDisplayPrefab);
            newStat.transform.SetParent(statParent, false);
            newStat.name = stat.Key;
        }
        return newUIObject;
    }

    public override void UpdateInspectorUI(object obj, GameObject uiObject)
    {
        Unit component = obj as Unit;
        uiObject.transform.Find("Health/HealthSlider").GetComponent<Slider>().value = component.Health.CurrentHealth / component.Health.MaxHealth;
        uiObject.transform.Find("Health/HealthText").GetComponent<TMP_Text>().text = $"{Mathf.RoundToInt(component.Health.CurrentHealth)} / {Mathf.RoundToInt(component.Health.MaxHealth)} HP";
        foreach (Transform child in uiObject.transform.Find("Stats"))
        {
            string statName = child.name;
            float? stat = component.GetStat(statName);
            child.Find("Value").GetComponent<TMP_Text>().text = Mathf.RoundToInt(stat ?? 0).ToString();
            child.Find("Name").GetComponent<TMP_Text>().text = statName;
        }
    }
}
