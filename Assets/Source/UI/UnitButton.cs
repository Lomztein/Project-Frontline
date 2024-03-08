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

        public GameObject Prefab;
        public Unit Unit;

        private Action<GameObject> _onClick;
        private Commander _commander;

        public Color InteractableImageColor;
        public Color NotInteractableImageColor;
        public Color UnavailableImageColor;

        private void Awake()
        {
            Button.onClick.AddListener(Click);
        }

        public void Click()
        {
            _onClick?.Invoke(Prefab);
        }

        public void Assign (GameObject prefab, Commander commander, Action<GameObject> onIconModelInstantiated, Action<GameObject> onClick)
        {
            Unit unit = prefab.GetComponent<Unit>();
            if (unit.TryGetComponent<IIconOverride>(out var iconOverride))
            {
                UnitImage.sprite = iconOverride.GetIcon();
            }
            else
            {
                UnitImage.sprite = Iconography.GenerateSprite(prefab, Iconography.DefaultRotation, 128, onIconModelInstantiated);
            }

            UnitTierImage.sprite = TierSprites[(int)unit.Info.UnitTier];

            Unit = unit;
            Prefab = prefab;
            _onClick = onClick;
            _commander = commander;
        }

        private void FixedUpdate()
        {
            if (_commander)
            {
                bool canAfford = _commander.CanAfford(Prefab);
                bool canPurchase = _commander.CanPurchase(Prefab);
                bool interactable = canAfford && canPurchase;
                Button.interactable = interactable;
                if (interactable)
                {
                    UnitImage.color = InteractableImageColor;
                }
                else if (!canPurchase)
                {
                    UnitImage.color = UnavailableImageColor;
                }
                else
                {
                    UnitImage.color = NotInteractableImageColor;
                }

                UnitTierImage.color = TierColors[(int)Unit.Info.UnitTier] * (interactable ? InteractableImageColor : NotInteractableImageColor);
            }
        }

        public GameObject InstantiateTooltip()
        {
            return UnitTooltip.Create(Unit, _commander);
        }
    }
}