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
            string weaponInfo = WeaponInfoToString();
            if (string.IsNullOrEmpty(weaponInfo))
            {
                newTooltip.transform.Find("Weapons").gameObject.SetActive(false);
            }
            else
            {
                newTooltip.transform.Find("Weapons").GetComponentInChildren<Text>().text = WeaponInfoToString();
            }
            return newTooltip;
        }

        private string WeaponInfoToString ()
        {
            WeaponInfo[] info = _unit.GetWeaponInfo();
            if (info.Length > 0)
            {
                var groups = info.GroupBy(x => x.Name);
                StringBuilder builder = new StringBuilder();
                foreach (var group in groups)
                {
                    int count = group.Count();
                    string prefix = count == 1 ? "" : count + "x ";
                    builder.AppendLine($"<b>{prefix}{group.Key}</b>");
                    builder.AppendLine($"<i>{group.First().Description}</i>");
                }
                return builder.ToString().Trim();
            }
            return null;
        }
    }
}