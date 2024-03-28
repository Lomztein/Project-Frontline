using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class MapSettings : MonoBehaviour
{
    public Dropdown Shape;
    public Dropdown Scenery;

    private BattlefieldShape[] _shapeCache;
    public Transform PropertyParent;

    private BattlefieldShape[] GetShapes ()
    {
        if (_shapeCache == null)
        {
            _shapeCache = Resources.LoadAll<BattlefieldShape>("BattlefieldShapes").Select(x => Instantiate(x)).ToArray();
        }
        return _shapeCache;
    }

    public BattlefieldShape GetShape ()
    {
        return GetShapes()[Shape.value];
    }

    private SceneryGenerator[] GetSceneries ()
    {
        return Resources.LoadAll<SceneryGenerator>("SceneryGenerators");
    }

    public SceneryGenerator GetScenery() => GetSceneries()[Scenery.value];

    private void Awake()
    {
        Shape.options = GetShapes().Select(x => new Dropdown.OptionData (x.Name)).ToList();
        Scenery.options = GetSceneries().Select(x => new Dropdown.OptionData (x.Name)).ToList();

        Shape.onValueChanged.AddListener(OnShapeChanged);
        Scenery.onValueChanged.AddListener(OnSceneryChanged);

        UpdateProperties();
    }

    private void OnShapeChanged(int val)
    {
        MatchSetup.Current.MapInfo.Shape = GetShapes()[val];
        MatchSetup.NotifyUpdate(MatchSetup.Current);
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        foreach (Transform trans in PropertyParent)
        {
            PropertyControl pc = trans.GetComponent<PropertyControl>();
            Destroy(trans.gameObject);
            pc.OnPropertyChanged -= OnPropertyChanged;
        }
        var properties = MatchSetup.Current.MapInfo.Shape.GetProperties();
        foreach (var property in properties)
        {
            var control = PropertyControl.GetControlPrefab(property);
            PropertyControl pc = Instantiate(control, PropertyParent).GetComponent<PropertyControl>();
            pc.Assign(property, MatchSetup.Current.MapInfo.Shape);
            pc.OnPropertyChanged += OnPropertyChanged;
        }
    }

    private void OnPropertyChanged(IProperty arg1, IHasProperties arg2, object arg3)
    {
        MatchSetup.NotifyUpdate(MatchSetup.Current);
    }

    private void OnSceneryChanged(int val)
    {
        MatchSetup.Current.MapInfo.SceneryGenerator = GetSceneries()[val];
        MatchSetup.NotifyUpdate(MatchSetup.Current);
    }

    public MapInfo CreateMapInfo()
    {
        MapInfo info = new MapInfo();
        info.Shape = GetShape();
        info.SceneryGenerator = GetScenery();
        return info;
    }

    public void ApplyMapInfo (MapInfo info)
    {
        // Yes, I know this could be done better. No, I don't particularily care.
        Shape.value = Array.IndexOf(GetShapes(), GetShapes().FirstOrDefault(x => x.Name == info.Shape.Name));
        Scenery.value = Array.IndexOf(GetSceneries(), GetSceneries().ToList().IndexOf(info.SceneryGenerator));
    }
}
