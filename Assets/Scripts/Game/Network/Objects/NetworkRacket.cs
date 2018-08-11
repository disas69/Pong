using Game.Gameplay.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Network.Objects
{
    public class NetworkRacket : NetworkBehaviour
    {
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
    }
}