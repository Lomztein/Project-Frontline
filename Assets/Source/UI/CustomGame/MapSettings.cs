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
        MatchSettings.Current.MapInfo.Shape = GetShapes()[val];
        MatchSettings.NotifyUpdate(MatchSettings.Current);
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
        var properties = MatchSettings.Current.MapInfo.Shape.GetProperties();
        foreach (var property in properties)
        {
            var control = PropertyControl.GetControlPrefab(property);
            PropertyControl pc = Instantiate(control, PropertyParent).GetComponent<PropertyControl>();
            pc.Assign(property, MatchSettings.Current.MapInfo.Shape);
            pc.OnPropertyChanged += OnPropertyChanged;
        }
    }

    private void OnPropertyChanged(IProperty arg1, IHasProperties arg2, object arg3)
    {
        MatchSettings.NotifyUpdate(MatchSettings.Current);
    }

    private void OnSceneryChanged(int val)
    {
        MatchSettings.Current.MapInfo.SceneryGenerator = GetSceneries()[val];
        MatchSettings.NotifyUpdate(MatchSettings.Current);
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
        Shape.value = GetShapes().ToList().IndexOf(info.Shape); // As you can tell, I litteraly don't care about poor performance when it matters this little.
        Scenery.value = GetSceneries().ToList().IndexOf(info.SceneryGenerator);
    }
}
