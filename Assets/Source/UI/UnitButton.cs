using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (unit.TryGetComponent<IIconOverride>(out var iconOverride))
            {
                UnitImage.sprite = iconOverride.GetIcon();
            }
            else
            {
                UnitImage.sprite = Iconography.GenerateSprite(prefab, Iconography.DefaultRotation, 64, x => commander.AssignCommander(x));
            }

            UnitTierImage.sprite = TierSprites[(int)unit.Info.UnitTier];

            _unit = unit;
            _prefab = prefab;
            _onClick = onClick;
            _commander = commander;
        }

        private void FixedUpdate()
        {
            bool interactable = _commander.CanAfford(_prefab) && _commander.CanPurchase(_prefab);
            Button.interactable = interactable;
            UnitImage.color = interactable ? ImageInteractableColor : ImageUnInteractableColor;
            UnitTierImage.color = TierColors[(int)_unit.Info.UnitTier] * (interactable ? ImageInteractableColor : ImageUnInteractableColor);
        }

        public GameObject InstantiateTooltip()
        {
            return UnitTooltip.Create(_unit);
        }
    }
}