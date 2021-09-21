using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Util
{
    public class UIHoverChecker : MonoBehaviour
    {
        public GraphicRaycaster Raycaster;
        public EventSystem EventSystem;

        private static UIHoverChecker _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static bool IsOverUI(Vector2 position)
        {
            PointerEventData data = new PointerEventData(_instance.EventSystem)
            {
                position = position
            };

            List<RaycastResult> results = new List<RaycastResult>();

            _instance.Raycaster.Raycast(data, results);
            return results.Any();
        }
    }
}