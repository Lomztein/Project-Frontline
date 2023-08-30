using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitTabButton : MonoBehaviour
    {
        public Button Button;
        public UnitPurchaseMenu Menu;
        public UnitInfo.Type Type;

        private void Awake()
        {
            Button.onClick.AddListener(OnClick);
            if (!MatchSettings.Current.SupportsUnitType(Type))
            {
                gameObject.SetActive(false);
            }
        }

        private void OnClick()
        {
            Menu.OpenTab(Type);
        }

        private void Update()
        {
            Button.interactable = Menu.CurrentOpenTab != Type;
        }
    }
}