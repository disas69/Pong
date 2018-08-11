using Game.Configuration;
using Game.Gameplay.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Network.Objects
{
    public class NetworkBall : NetworkBehaviour
    {
        [SerializeField] private BallSettingsStorage _ballSettingsStorage;

        public Ball Object { get; private set; }

        private void Awake()
        {
            Object = GetComponent<Ball>();

            if (Object == null)
            {
                Debug.LogError(string.Format("NetworkObject<{0}> can be attached only to MonoBehaviour of that type",
                    typeof(Ball).Name));
            }
        }

        public void Setup(string settingsName)
        {
            var ballSettings = _ballSettingsStorage.GetBallSettingsByName(settingsName);
            Object.Setup(ballSettings);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkManager.RegisterBall(this);
        }
    }
}