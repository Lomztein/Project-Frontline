using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class MapSettings : MonoBehaviour
{
    public Text MapWidthText;
    public Slider MapWidthSlider;
    public Text MapHeightText;
    public Slider MapHeightSlider;

    public Vector2 MapWidthMinMax;
    public Vector2 MapHeightMinMax;

    public Dropdown Shape;
    public Dropdown Scenery;

    private BattlefieldShape[] _shapeCache;

    private BattlefieldShape[] GetShapes ()
    {
        if (_shapeCache == null)
        {
            _shapeCache = Resources.LoadAll<BattlefieldShape>("BattlefieldShapes");
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

    private void Update()
    {
        MapWidthText.text = "Map Width: " + Mathf.Lerp(MapWidthMinMax.x, MapWidthMinMax.y, MapWidthSlider.value).ToString("0");
        MapHeightText.text = "Map Height: " + Mathf.Lerp(MapHeightMinMax.x, MapHeightMinMax.y, MapHeightSlider.value).ToString("0");
    }

    private void Awake()
    {
        Shape.options = GetShapes().Select(x => new Dropdown.OptionData (x.Name)).ToList();
        Scenery.options = GetSceneries().Select(x => new Dropdown.OptionData (x.Name)).ToList();

        MapWidthSlider.onValueChanged.AddListener(OnWidthChanged);
        MapHeightSlider.onValueChanged.AddListener(OnHeightChanged);
        Shape.onValueChanged.AddListener(OnShapeChanged);
        Scenery.onValueChanged.AddListener(OnSceneryChanged);
    }

    private void OnWidthChanged (float val)
    {
        MatchSettings.Current.MapInfo.Width = Mathf.Lerp(MapWidthMinMax.x, MapWidthMinMax.y, val);
        MatchSettings.NotifyUpdate(MatchSettings.Current);
    }

    private void OnHeightChanged(float val)
    {
        MatchSettings.Current.MapInfo.Height = Mathf.Lerp(MapHeightMinMax.x, MapHeightMinMax.y, val);
        MatchSettings.NotifyUpdate(MatchSettings.Current);
    }

    private void OnShapeChanged(int val)
    {
        MatchSettings.Current.MapInfo.Shape = GetShapes()[val];
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
        info.Width = Mathf.Lerp(MapWidthMinMax.x, MapWidthMinMax.y, MapWidthSlider.value);
        info.Height = Mathf.Lerp(MapHeightMinMax.x, MapHeightMinMax.y, MapHeightSlider.value);
        return info;
    }

    public void ApplyMapInfo (MapInfo info)
    {
        Shape.value = GetShapes().ToList().IndexOf(info.Shape); // As you can tell, I litteraly don't care about poor performance when it matters this little.
        Scenery.value = GetSceneries().ToList().IndexOf(info.SceneryGenerator);
        MapWidthSlider.value = Mathf.InverseLerp(MapWidthMinMax.x, MapWidthMinMax.y, info.Width);
        MapHeightSlider.value = Mathf.InverseLerp(MapHeightMinMax.x, MapHeightMinMax.y, info.Height);
    }
}
