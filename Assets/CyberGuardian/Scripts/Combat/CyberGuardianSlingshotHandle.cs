using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberGuardian
{
    public sealed class CyberGuardianSlingshotHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public CyberGuardianLevelController controller;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (controller != null)
            {
                controller.BeginSlingshotDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (controller != null)
            {
                controller.DragSlingshot(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (controller != null)
            {
                controller.ReleaseSlingshot(eventData);
            }
        }
    }
}
