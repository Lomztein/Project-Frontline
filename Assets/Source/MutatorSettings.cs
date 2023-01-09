using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutatorSettings : MonoBehaviour
{
    private IEnumerable<Mutator> _mutators = new List<Mutator>();

    public GameObject MutatorPrefab;
    public Transform MutatorParent;

    private void Awake()
    {
        _mutators = Mutator.LoadMutators();
    }

    private void Start()
    {
        MatchSettings.Current.ClearMutators();
        GenerateToggles();
    }

    public void GenerateToggles ()
    {
        foreach (var m in _mutators)
        {
            GameObject newMutator = Instantiate(MutatorPrefab, MutatorParent);
            Toggle toggle = newMutator.GetComponentInChildren<Toggle>();
            toggle.onValueChanged.AddListener(x => OnToggle(m, x));
            newMutator.GetComponentInChildren<Text>().text = m.Name;
        }
    }

    private void OnToggle(Mutator mutator, bool value)
    {
        if (value == false)
        {
            RemoveMutator(mutator);
        }
        else
        {
            AddMutator(mutator);
        }
    }

    private void AddMutator(Mutator mutator)
    {
        MatchSettings.Current.AddMutator(mutator);
    }

    private void RemoveMutator (Mutator mutator)
    {
        MatchSettings.Current.RemoveMutator(mutator);
    }
}
