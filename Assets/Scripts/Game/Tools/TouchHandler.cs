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
            _tapDetector = new TapDetector(0.5f, 1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tapDetector.RegisterTouchDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var result = _tapDetector.RegisterTouchUp(eventData);
            if (result == TouchResult.Tap)
            {
                _onTapEvent.Invoke();
            }
            else if (result == TouchResult.Hold)
            {
                _onHoldEvent.Invoke();
            }
        }
    }
}