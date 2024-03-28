using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommanderInfoPanel : MonoBehaviour
{
    public GameObject CommanderInfoPrefab;
    public Transform CommanderInfoParent;

    private WeightedUnitSelector _lastSelector;
    public Text WeightTable;

    public void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        yield return null;

        var commanders = FindObjectsOfType<Commander>();
        foreach (var commander in commanders)
        {
            GameObject newInfo = Instantiate(CommanderInfoPrefab, CommanderInfoParent);
            newInfo.GetComponent<CommanderInfoDisplay>().Commander = commander;
            newInfo.GetComponent<Button>().onClick.AddListener(() => OnClick(commander));
        }
    }

    private void OnClick(Commander commander)
    {
        WeightTable.gameObject.SetActive(false);
        if (_lastSelector != null)
        {
            _lastSelector.WeightDebug = null;
        }
        if (commander.TryGetComponent(out WeightedUnitSelector selector))
        {
            selector.WeightDebug = WeightTable;
            WeightTable.gameObject.SetActive(true);
            _lastSelector = selector;
        }
    }
}
