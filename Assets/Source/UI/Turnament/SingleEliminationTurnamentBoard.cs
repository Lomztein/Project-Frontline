using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SingleEleminationTurnamentBoard : MonoBehaviour, ITurnamentBoard
{
    public RectTransform BoardParent;

    public GameObject PlayerSlotPrefab;
    public GameObject SpacerSlotPrefab;
    public GameObject SetPrefab;
    public GameObject LinePrefab;

    public float SlotHeight = 100;

    public int TestAmount;
    private void Start()
    {
        Initialize(TurnamentRunner.GetInstance());
    }

    public void HighlightMatch(TurnamentMatchNode matchToHighlight)
    {
    }

    public void Initialize(TurnamentRunner runner)
    {
        BuildUI(runner);
    }

    private void BuildUI(TurnamentRunner runner)
    {
        int numPlayers = runner.PlayerInfos.Length;
        int numSlots = Mathf.NextPowerOfTwo(numPlayers);
        int sets = Mathf.RoundToInt(Mathf.Log(numSlots, 2f)) + 1;
        for (int i = 0; i < sets; i++)
        {
            GameObject newSet = Instantiate(SetPrefab, BoardParent);
            RectTransform setTransform = newSet.GetComponent<RectTransform>();
            int numSetSlots = numSlots / (int)Mathf.Pow(2, i);
            BuildSet(i, numSetSlots, setTransform);
        }
    }

    private void BuildSet(int index, int slots, RectTransform setParent)
    {
        float spacerMultiplier = Mathf.Pow(2, index) - 1;

        for (int i = 0; i < slots; i++)
        {
            TurnamentPlayerNode slotNode = GetPlayerNode(TurnamentRunner.GetInstance().PlayerInfos.Length, index, i);
            GameObject slot = Instantiate(PlayerSlotPrefab, setParent);
            slot.GetComponent<LayoutElement>().preferredHeight = SlotHeight;
            slot.GetComponentInChildren<TMP_Text>().text = slotNode.PlayerInfo?.Name ?? "Empty";

            if (i != slots - 1 && slots != 1)
            {
                GameObject spacer = Instantiate(SpacerSlotPrefab, setParent);
                spacer.GetComponent<LayoutElement>().preferredHeight = spacerMultiplier * slot.GetComponent<RectTransform>().sizeDelta.y;
            }
        }
    }

    private TurnamentPlayerNode GetPlayerNode(int numPlayers, int setIndex, int slotIndexInSet)
    {
        int numSlotsInFirstSet = Mathf.NextPowerOfTwo(numPlayers);
        int index = 0;
        for (int i = 0; i < setIndex; i++)
        {
            index += numSlotsInFirstSet / (int)Mathf.Pow(2, i);
        }

        index += slotIndexInSet;
        var node = TurnamentRunner.GetInstance().PlayerNodes[index];
        return node;
    }
}
