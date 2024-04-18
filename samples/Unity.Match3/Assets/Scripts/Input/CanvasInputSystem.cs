using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3
{
    public class CanvasInputSystem : MonoSingleton<CanvasInputSystem>
    {
        private EventTrigger _eventTrigger;
        private Camera _mainCam;

        public event EventHandler<PointerEventArgs> PointerDown;
        public event EventHandler<PointerEventArgs> PointerDrag;
        public event EventHandler<PointerEventArgs> PointerUp;

        protected override void OnAwake()
        {
            _eventTrigger = GetComponent<EventTrigger>();
            _mainCam = Camera.main;

            Init();
        }

        private void Init()
        {
            var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener(data => { OnPointerDown((PointerEventData)data); });

            var pointerDrag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            pointerDrag.callback.AddListener(data => { OnPointerDrag((PointerEventData)data); });

            var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener(data => { OnPointerUp((PointerEventData)data); });

            _eventTrigger.triggers.Add(pointerDown);
            _eventTrigger.triggers.Add(pointerDrag);
            _eventTrigger.triggers.Add(pointerUp);
        }

        private void OnPointerDown(PointerEventData e)
        {
            PointerDown?.Invoke(this, GetPointerEventArgs(e));
        }

        private void OnPointerDrag(PointerEventData e)
        {
            PointerDrag?.Invoke(this, GetPointerEventArgs(e));
        }

        private void OnPointerUp(PointerEventData e)
        {
            PointerUp?.Invoke(this, GetPointerEventArgs(e));
        }

        private PointerEventArgs GetPointerEventArgs(PointerEventData e)
        {
            return new PointerEventArgs(e.button, GetWorldPosition(e.position));
        }

        private Vector2 GetWorldPosition(Vector2 screenPosition)
        {
            return _mainCam.ScreenToWorldPoint(screenPosition);
        }
    }
}