using Game.Gameplay.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Network.Objects
{
    public class NetworkRacket : NetworkBehaviour
    {
        /*
        private bool _isReady;
        private Vector3 _lastPosition;
        private float _currentLerpTime;

        [SyncVar] private Vector3 _syncPosition;
        
        [SerializeField] private float _lerpTime = 1;
        [SerializeField] private float _threshold = 0f;
        */

        public Racket Object { get; private set; }

        private void Awake()
        {
            Object = GetComponent<Racket>();

            if (Object == null)
            {
                Debug.LogError(string.Format("NetworkObject<{0}> can be attached only to MonoBehaviour of that type",
                    typeof(Racket).Name));
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkManager.RegisterRacket(this);
        }

        /*
        public void SetReady(bool ready, Vector3 startPosition)
        {
            _isReady = ready;
            _lastPosition = startPosition;
            _syncPosition = startPosition;
        }

        private void Update()
        {
            if (_isReady && !isLocalPlayer)
            {
                LerpPosition();
            }
        }

        private void FixedUpdate()
        {
            if (_isReady)
            {
                TransmitPosition();
            }
        }

        private void LerpPosition()
        {
            _currentLerpTime += Time.deltaTime;
            if (_currentLerpTime > _lerpTime)
            {
                _currentLerpTime = _lerpTime;
            }

            Object.transform.position = Vector3.Lerp(Object.transform.position, _syncPosition, _currentLerpTime / _lerpTime);
        }

        [ClientCallback]
        private void TransmitPosition()
        {
            if (isLocalPlayer && Vector3.Distance(Object.transform.position, _lastPosition) > _threshold)
            {
                CmdProvidePositionToServer(Object.transform.position);
                _lastPosition = Object.transform.position;
            }
        }

        [Command]
        private void CmdProvidePositionToServer(Vector3 position)
        {
            _syncPosition = position;
            _currentLerpTime = 0f;
        }
        */
    }
}