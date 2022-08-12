using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    public RectTransform TypesParent;
    public GameObject TypePrefab;

    public enum GroupingType { Prefix, Tier, Cost }
    public GroupingType GroupBy;

    private void Start()
    {
        Initialize();
    }

    private void Initialize() 
    {

    }

    private IGrouping<string, GameObject> GetGroupings ()
    {
        switch (GroupBy)
        {
            case GroupingType.Tier: return GroupByTier();
            case GroupingType.Prefix: return GroupByPrefix();
            case GroupingType.Cost: return GroupByCost();
        }
        throw new InvalidOperationException("Unsupported grouping.");
    }

    private IGrouping<string, GameObject> GroupByCost()
    {
        throw new NotImplementedException();
    }

    private IGrouping<string, GameObject> GroupByPrefix()
    {
        throw new NotImplementedException();
    }

    private IGrouping<string, GameObject> GroupByTier()
    {
        throw new NotImplementedException();
    }
}
