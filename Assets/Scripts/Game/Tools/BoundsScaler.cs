using Game.Camera;
using UnityEngine;

namespace Game.Tools
{
    public class BoundsScaler : MonoBehaviour
    {
        [SerializeField] private CameraController _camera;
        [SerializeField] private BoxCollider _leftBounds;
        [SerializeField] private BoxCollider _rightBounds;
        [SerializeField] private BoxCollider _topBounds;
        [SerializeField] private BoxCollider _bottomBounds;
        [SerializeField] private bool _executeInUpdate;

        private void Awake()
        {
            var bounds = _camera.Bounds;
            CalculateBoundsSize(bounds);
            CalculateBoundsPositions(bounds);
        }

        private void Update()
        {
            if (_executeInUpdate)
            {
                var bounds = _camera.Bounds;
                CalculateBoundsSize(bounds);
                CalculateBoundsPositions(bounds);
            }
        }

        private void CalculateBoundsSize(Bounds bounds)
        {
            var hight = bounds.extents.y * 2f;
            _leftBounds.size = new Vector3(_leftBounds.size.x, hight, _leftBounds.size.z);
            _rightBounds.size = new Vector3(_rightBounds.size.x, hight, _rightBounds.size.z);

            var width = bounds.extents.x * 2f;
            _topBounds.size = new Vector3(width, _topBounds.size.y, _topBounds.size.z);
            _bottomBounds.size = new Vector3(width, _bottomBounds.size.y, _bottomBounds.size.z);
        }

        private void CalculateBoundsPositions(Bounds bounds)
        {
            var leftPosition = new Vector3((bounds.center.x - bounds.extents.x) - _leftBounds.size.x / 2f, 0f, 0f);
            _leftBounds.transform.position = leftPosition;

            var rightPosition = new Vector3((bounds.center.x + bounds.extents.x) + _rightBounds.size.x / 2f, 0f, 0f);
            _rightBounds.transform.position = rightPosition;

            var topPosition = new Vector3(0f, (bounds.center.y + bounds.extents.y) + _topBounds.size.y / 2f, 0f);
            _topBounds.transform.position = topPosition;

            var bottomPosition = new Vector3(0f, (bounds.center.y - bounds.extents.y) - _bottomBounds.size.y / 2f, 0f);
            _bottomBounds.transform.position = bottomPosition;
        }
    }
}