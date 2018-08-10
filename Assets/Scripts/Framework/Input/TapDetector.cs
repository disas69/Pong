using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Input
{
    public enum TouchResult
    {
        Tap,
        Swipe,
        Hold
    }

    public class TapDetector
    {
        private readonly float _distanceLimit;
        private readonly float _timeLimit;

        private float _touchDownTime;
        private Vector3 _touchDownPosition;

        public TapDetector(float distanceLimit = 0.25f, float timeLimit = 0.25f)
        {
            _distanceLimit = distanceLimit;
            _timeLimit = timeLimit;
        }

        public void RegisterTouchDown(PointerEventData eventData)
        {
            _touchDownTime = Time.time;
            _touchDownPosition = eventData.position;
        }

        public TouchResult RegisterTouchUp(PointerEventData eventData)
        {
            if (Time.time - _touchDownTime < _timeLimit)
            {
                var distance = Vector2.Distance(_touchDownPosition, eventData.position);
                var deltaInInches = distance / Screen.dpi;
                if (deltaInInches < _distanceLimit)
                {
                    return TouchResult.Tap;
                }

                return TouchResult.Swipe;
            }

            return TouchResult.Hold;
        }
    }
}