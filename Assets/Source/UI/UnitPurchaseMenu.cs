using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class UnitPurchaseMenu : MonoBehaviour
    {
        public Commander Commander;

        public GameObject ButtonPrefab;
        public Transform ButtonParent;

        public UnitInfo.Type CurrentOpenTab;
        public UnitPlacement Placement;

        private void Start()
        {
            if (Commander)
            {
                Commander.OnEliminated += Commander_OnEliminated;
            }
            UpdateActive();
            if (gameObject.activeSelf)
            {
                OpenTab(CurrentOpenTab);
            }
        }

        public void UpdateActive ()
        {
            gameObject.SetActive(Commander != null && Commander.isActiveAndEnabled && !Commander.Eliminated);
        }

        private void Commander_OnEliminated(Commander obj)
        {
            UpdateActive();
        }

        public void OpenTab(UnitInfo.Type unitType)
        {
            ClearButtons();

            GameObject[] units = Commander.UnitSource.GetAvailableUnitPrefabs(Commander.Faction).Where(x => x.GetComponent<Unit>().Info.UnitType == unitType).ToArray();
            Array.Sort(units, new UnitComparer());

            foreach (GameObject unit in units)
            {
                InstantiateButton(unit);
            }
            CurrentOpenTab = unitType;
        }



        private void ClearButtons()
        {
            foreach (Transform child in ButtonParent)
            {
                Destroy(child.gameObject);
            }
        }

        private UnitButton InstantiateButton(GameObject unitPrefab)
        {
            GameObject newObj = Instantiate(ButtonPrefab, ButtonParent);
            UnitButton button = newObj.GetComponent<UnitButton>();
            button.Assign(unitPrefab, Commander, OnButtonClick);
            return button;
        }

        private void OnButtonClick(GameObject unitPrefab)
        {
            Placement.TakeUnit(unitPrefab, Commander);
        }
    }
}