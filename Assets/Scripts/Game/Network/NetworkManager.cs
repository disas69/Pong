using Game.Gameplay.GameModes;
using Game.Network.Objects;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Network
{
    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {
        private static MultiPlayerGameMode _multiPlayer;

        public static bool IsHost;

        [SerializeField] private MultiPlayerGameMode _multiPlayerGameMode;

        private void Awake()
        {
            _multiPlayer = _multiPlayerGameMode;
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            _multiPlayer.UnregisterPlayer();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            _multiPlayer.UnregisterPlayer();
        }

        public static void RegisterRacket(NetworkRacket networkRacket)
        {
            if (_multiPlayer != null)
            {
                _multiPlayer.RegisterRacket(networkRacket);
            }
        }

        public static void RegisterBall(NetworkBall networkBall)
        {
            if (_multiPlayer != null)
            {
                _multiPlayer.RegisterBall(networkBall);
            }
        }
    }
}