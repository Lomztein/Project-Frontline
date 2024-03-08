using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New UnitFactoryComponentInspector", menuName = "ComponentInspector/UnitFactory")]
public class UnitFactoryComponentInspector : ComponentInspector
{
    public GameObject UIPrefab;

    public override bool CanInspect(object obj)
        => obj is UnitFactory;

    public override GameObject InstantiateInspectUI(object component)
        => Instantiate(UIPrefab);

    public override void UpdateInspectorUI(object obj, GameObject uiObject)
    {
        UnitFactory component = obj as UnitFactory;
        float progress = Mathf.InverseLerp(component.NextProductionTime - component.ProductionTime, component.NextProductionTime, Time.time);
        float time = Mathf.Lerp(component.ProductionTime, 0f, progress);
        
        uiObject.transform.Find("Slider/NextSlider").GetComponent<Slider>().value = progress;
        uiObject.transform.Find("Slider/NextText").GetComponent<TMP_Text>().text = $"{Mathf.RoundToInt(time)} seconds.";
    }
}
