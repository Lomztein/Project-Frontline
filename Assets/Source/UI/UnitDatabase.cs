using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public TMP_Dropdown FactionFilterDropdown;
    public TMP_Dropdown GenericFilterDropdown;
    public TMP_Dropdown TeamDropdown;

    public GameObjectFilter[] FactionFilters;
    public GameObjectFilter[] GenericFilters;

    public int SelectedFactionFilter;
    public int SelectedGenericFilter;
    public bool ShowUnusedUnits;

    public UnitInspector UnitInspector;
    public Transform UnitButtonParent;
    public GameObject UnitButtonPrefab;

    private List<UnitButton> _unitButtons = new List<UnitButton>();

    public TeamInfo ButtonIconTeam;
    public TeamInfo ShowWithTeam;

    private Dictionary<Faction, Dictionary<TeamInfo, UnitPalette>> _paletteCache;
    private Dictionary<GameObject, Faction> _unitFactionCache;

    private GameObject _currentUnitPrefab;

    public GameObjectFilter GenerateUnitFilter()
    {
        List<GameObjectFilter> filters = new List<GameObjectFilter>();
        if (SelectedFactionFilter != 0)
        {
            filters.Add(FactionFilters[SelectedFactionFilter - 1]);
        }
        if (SelectedGenericFilter != 0)
        {
            filters.Add(GenericFilters[SelectedGenericFilter - 1]);
        }
        return CompositeGameObjectFilter.Create(filters);
    }

    public void SetFactionFilter(int value)
        => SelectedFactionFilter = value;
    public void SetGenericFilter(int value)
        => SelectedGenericFilter = value;
    public void SetTeamInfo(int value)
        => ShowWithTeam = TeamInfo.LoadTeams()[value];

    public void Start()
    {
        PopulateCaches();
        PopulateDropdowns();
        CreateButtons();
    }

    private void PopulateCaches()
    {
        _paletteCache = new Dictionary<Faction, Dictionary<TeamInfo, UnitPalette>>();
        _unitFactionCache = new Dictionary<GameObject, Faction>();
        foreach (var faction in Faction.LoadFactions())
        {
            _paletteCache.Add(faction, new Dictionary<TeamInfo, UnitPalette>());
            foreach (var teamInfo in TeamInfo.LoadTeams())
            {
                _paletteCache[faction].Add(teamInfo, UnitPalette.GeneratePalette(faction.FactionPalette, teamInfo.TeamPalette));
            }

            foreach (var unit in faction.LoadUnits())
            {
                _unitFactionCache.TryAdd(unit, faction);
            }
        }
    }

    public void UpdateButtonVisibility()
    {
        GameObjectFilter filter = GenerateUnitFilter();
        foreach (UnitButton button in _unitButtons)
        {
            button.gameObject.SetActive(filter.Check(button.Prefab));
        }
    }

    public void PopulateDropdowns()
    {
        Faction[] factions = Faction.LoadFactions().ToArray();
        FactionFilters = factions.Select(x => PredicateGameObjectFilter.Create(y => IsInFaction(y, x))).ToArray();

        FactionFilterDropdown.ClearOptions();
        FactionFilterDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
        FactionFilterDropdown.AddOptions(factions.Select(x => new TMP_Dropdown.OptionData(x.Name)).ToList());

        GenericFilterDropdown.ClearOptions();
        GenericFilterDropdown.options.Add(new TMP_Dropdown.OptionData("All"));
        GenericFilterDropdown.AddOptions(GenericFilters.Select(x => new TMP_Dropdown.OptionData(x.name)).ToList());

        TeamDropdown.options = TeamInfo.LoadTeams().Select(x => new TMP_Dropdown.OptionData(x.Name)).ToList();
    }

    private bool IsInFaction(GameObject unit, Faction faction)
        => faction.LoadUnits().Contains(unit);

    private Faction GetUnitFaction(GameObject unit)
        => _unitFactionCache.ContainsKey(unit) ? _unitFactionCache[unit] : null;

    private UnitPalette GetUnitPalette(GameObject unit, TeamInfo team)
    {
        Faction faction = GetUnitFaction(unit);
        if (faction != null)
        {
            return _paletteCache[faction][team];
        }
        return null;
    }

    public void CreateButtons ()
    {
        GameObject[] unitPrefabs = Resources.LoadAll<GameObject>("Units");
        foreach (GameObject unitPrefab in unitPrefabs)
        {
            UnitPalette palette = GetUnitPalette(unitPrefab, ButtonIconTeam);
            if (palette != null)
            {
                GameObject newButton = GameObject.Instantiate(UnitButtonPrefab, UnitButtonParent);
                newButton.GetComponent<UnitButton>().Assign(unitPrefab, null, x => palette.ApplyTo(x), OnButtonClick);
                _unitButtons.Add(newButton.GetComponent<UnitButton>());
            }

            else if (ShowUnusedUnits)
            {
                GameObject newButton = GameObject.Instantiate(UnitButtonPrefab, UnitButtonParent);
                newButton.GetComponent<UnitButton>().Assign(unitPrefab, null, x => { }, OnButtonClick);
                _unitButtons.Add(newButton.GetComponent<UnitButton>());
            }
        }
    }

    private void OnButtonClick(GameObject @object)
    {
        _currentUnitPrefab = @object;
        GameObject newUnit = UnitInspector.Inspect(@object);
        UnitPalette palette = GetUnitPalette(@object, ShowWithTeam);
        palette.ApplyTo(newUnit);
    }

    public void RespawnUnit()
    {
        OnButtonClick(_currentUnitPrefab);
    }
}
