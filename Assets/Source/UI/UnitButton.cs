using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class UnitButton : MonoBehaviour, IHasTooltip
    {
        public Image UnitImage;
        public Image UnitTierImage;
        public Button Button;
        public GameObject TooltipPrefab;

        public Sprite[] TierSprites;
        public Color[] TierColors;

        private GameObject _prefab;
        private Unit _unit;

        private Action<GameObject> _onClick;
        private Commander _commander;

        public Color ImageInteractableColor;
        public Color ImageUnInteractableColor;

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _onClick?.Invoke(_prefab);
        }

        public void Assign (GameObject prefab, Commander commander, Action<GameObject> onClick)
        {
            Unit unit = prefab.GetComponent<Unit>();
            UnitImage.sprite = Iconography.GenerateSprite(prefab);
            UnitTierImage.sprite = TierSprites[(int)unit.Info.UnitTier];

            _unit = unit;
            _prefab = prefab;
            _onClick = onClick;
            _commander = commander;
        }

        private void FixedUpdate()
        {
            bool interactable = _commander.Credits >= _unit.Cost;
            Button.interactable = interactable;
            UnitImage.color = interactable ? ImageInteractableColor : ImageUnInteractableColor;
            UnitTierImage.color = TierColors[(int)_unit.Info.UnitTier] * (interactable ? ImageInteractableColor : ImageUnInteractableColor);
        }

        public GameObject InstantiateTooltip()
        {
            GameObject newTooltip = Instantiate(TooltipPrefab);
            newTooltip.transform.Find("Name").GetComponentInChildren<Text>().text = _unit.Name + " - " + _unit.Cost + "$";
            newTooltip.transform.Find("Description").GetComponentInChildren<Text>().text = _unit.Description;
            return newTooltip;
        }
    }
}