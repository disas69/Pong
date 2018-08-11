using Game.Gameplay.GameModes;
using Game.Network.Objects;
using UnityEngine.Networking;

namespace Game.Network
{
    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {
        private static MultiPlayerGameMode _multiPlayerGameMode;

        public MultiPlayerGameMode MultiPlayerGameMode;

        private void Awake()
        {
            _multiPlayerGameMode = MultiPlayerGameMode;
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            _multiPlayerGameMode.UnregisterPlayer();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            _multiPlayerGameMode.UnregisterPlayer();
        }

        public static void RegisterRacket(NetworkRacket networkRacket)
        {
            if (_multiPlayerGameMode != null)
            {
                _multiPlayerGameMode.RegisterRacket(networkRacket);
            }
        }

        public static void RegisterBall(NetworkBall networkBall)
        {
            if (_multiPlayerGameMode != null)
            {
                _multiPlayerGameMode.RegisterBall(networkBall);
            }
        }
    }
}