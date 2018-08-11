using Framework.Input;
using Framework.Utils.Math;
using Game.Camera;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private bool _isDragging;
        private Vector2 _lastDragDelta;
        private Vector2 _currentVelocity;
        private VectorAverager _dragSpeedAverager;
        private float _screenToWorldScaleFactor;
        private float _screenToCanvasScaleFactor;
        private int _pointersCount;
        private IControllableObject[] _controllableObjects; 

        [SerializeField] private Canvas _canvas;
        [SerializeField] private InputEventProvider _inputEventProvider;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private float _maxReleaseSpeed = 20f;
        [SerializeField] private float _slideDecelerationFactor = 1.5f;
        [SerializeField] private float _slideStopThreshold = 1f;

        private void Start()
        {
            var canvasRect = _canvas.GetComponent<RectTransform>().rect;
            _screenToCanvasScaleFactor = canvasRect.width / (float)_cameraController.Camera.pixelWidth;

            _dragSpeedAverager = new VectorAverager(0.1f);
            _screenToWorldScaleFactor = 2 * _cameraController.Camera.orthographicSize / _cameraController.Camera.pixelHeight;

            _inputEventProvider.PointerDown += OnPointerDown;
            _inputEventProvider.BeginDrag += OnBeginDrag;
            _inputEventProvider.Drag += OnDrag;
            _inputEventProvider.PointerUp += OnPointerUp;
        }

        public void Initialize(IControllableObject[] controllableObjects)
        {
            _controllableObjects = controllableObjects;
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            _pointersCount++;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _dragSpeedAverager.Clear();
        }

        private void OnDrag(PointerEventData eventData)
        {
            _lastDragDelta = eventData.delta;
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            _pointersCount--;
            if (_pointersCount == 0 && _isDragging)
            {
                _isDragging = false;

                var maxReleaseSpeed = _maxReleaseSpeed / _screenToCanvasScaleFactor;
                var releaseVelocity = _dragSpeedAverager.Value;
                if (releaseVelocity.magnitude > maxReleaseSpeed)
                {
                    releaseVelocity = releaseVelocity.normalized * maxReleaseSpeed;
                }

                _currentVelocity = releaseVelocity;
            }
        }

        private void Update()
        {
            if (_isDragging)
            {
                _currentVelocity = _lastDragDelta;
                _dragSpeedAverager.AddSample(_currentVelocity);
            }
            else
            {
                var currentSpeed = _currentVelocity.magnitude;
                if (currentSpeed > _slideStopThreshold / _screenToCanvasScaleFactor)
                {
                    _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, _slideDecelerationFactor * currentSpeed * Time.deltaTime);
                }
                else
                {
                    _currentVelocity = Vector2.zero;
                }
            }

            UpdatePositions(_currentVelocity * _screenToWorldScaleFactor);
            _lastDragDelta = Vector2.zero;
        }

        private void UpdatePositions(Vector3 worldspaceDelta)
        {
            if (_controllableObjects != null && _controllableObjects.Length > 0)
            {
                var bounds = _cameraController.Bounds;

                for (int i = 0; i < _controllableObjects.Length; i++)
                {
                    var controllableObject = _controllableObjects[i];
                    if (controllableObject != null && controllableObject.Box != null)
                    {
                        var leftBoundsX = bounds.center.x - bounds.extents.x + controllableObject.Box.transform.localScale.x / 2f;
                        var rightBoundsX = bounds.center.x + bounds.extents.x - controllableObject.Box.transform.localScale.x / 2f;
                        var oldYPosition = controllableObject.Position.y;
                        var newPosition = controllableObject.Position + worldspaceDelta;
                        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundsX, rightBoundsX);
                        newPosition.y = oldYPosition;
                        controllableObject.Position = newPosition;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _inputEventProvider.PointerDown -= OnPointerDown;
            _inputEventProvider.BeginDrag -= OnBeginDrag;
            _inputEventProvider.Drag -= OnDrag;
            _inputEventProvider.PointerUp -= OnPointerUp;
        }
    }
}