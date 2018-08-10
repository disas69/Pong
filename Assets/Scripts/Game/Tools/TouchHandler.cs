using Framework.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Tools
{
    [RequireComponent(typeof(Image))]
    public class TouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private TapDetector _tapDetector;

        [SerializeField] private UnityEvent _onTapEvent;
        [SerializeField] private UnityEvent _onHoldEvent;

        private void Awake()
        {
            _tapDetector = new TapDetector(0.25f, 0.5f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tapDetector.RegisterTouchDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_tapDetector.RegisterTouchUp(eventData))
            {
                _onTapEvent.Invoke();
            }
            else
            {
                _onHoldEvent.Invoke();
            }
        }
    }
}