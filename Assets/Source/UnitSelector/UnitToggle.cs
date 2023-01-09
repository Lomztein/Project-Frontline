using CustomGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class UnitToggle : MonoBehaviour, IHasTooltip
{
    public Image Image;
    public Toggle Toggle;
    public Image Tier;

    public Sprite[] TierSprites;
    public Color[] TierColors;

    public GameObject TooltipPrefab;
    private Unit _unit;

    public void Apply(PlayerSettings player, GameObject unit, Action<GameObject, bool> callback)
    {
        _unit = unit.GetComponent<Unit>();
        Toggle.onValueChanged.AddListener((x) => callback(unit, x));

        Image.sprite = Iconography.GenerateSprite(unit);
        Tier.sprite = TierSprites[(int)_unit.Info.UnitTier];
        Tier.color = TierColors[(int)_unit.Info.UnitTier];

        if (player.UnitAvailable.TryGetValue(unit, out bool value))
        {
            Toggle.isOn = value;
        }
        else
        {
            Toggle.isOn = true;
        }
    }

    public void SetEnabled (bool value)
    {
        Toggle.isOn = value;
    }

    public GameObject InstantiateTooltip()
    {
        return UnitTooltip.Create(_unit);
    }
}
