using UnityEngine;
using UnityEngine.EventSystems;

namespace SQLite4Unity3d
{
    public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static bool isMouseOver = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOver = false;
        }
    }
}