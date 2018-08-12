using System.Collections.Generic;
using Game.Gameplay.GameModes;
using Game.Network.Objects;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace Game.Network
{
    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {
        private static MultiPlayerGameMode _multiPlayerGameModeInstance;
        private static int _connectedPlayers;

        [SerializeField] private MultiPlayerGameMode _multiPlayerGameMode;

        public static bool IsHost { get; set; }

        public static bool IsReady
        {
            get { return _connectedPlayers == 2; }
        }

        private void Awake()
        {
            _multiPlayerGameModeInstance = _multiPlayerGameMode;
        }

        public void StartMatch(NetworkMatch.DataResponseDelegate<MatchInfo> onMatchCreateCallback,
            NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> onMatchListCallback)
        {
            StartMatchMaker();

            if (IsHost)
            {
                matchMaker.CreateMatch(string.Format("game_{0}", Random.Range(0, 100)), 2, true, "", "", "", 0, 0, onMatchCreateCallback);
            }
            else
            {
                matchMaker.ListMatches(0, 10, "", true, 0, 0, onMatchListCallback);
            }
        }

        public void JoinMatch(NetworkID networkId, NetworkMatch.DataResponseDelegate<MatchInfo> onJoinedMatchCallback)
        {
            matchMaker.JoinMatch(networkId, "", "", "", 0, 0, onJoinedMatchCallback);
        }

        public void Disconnect()
        {
            if (IsHost)
            {
                StopHost();
            }
            else
            {
                StopClient();
            }
        }

        public static void RegisterPlayer(NetworkRacket networkRacket)
        {
            if (_multiPlayerGameModeInstance != null)
            {
                _multiPlayerGameModeInstance.RegisterRacket(networkRacket, () => _connectedPlayers++);
            }
        }

        public static void RegisterBall(NetworkBall networkBall)
        {
            if (_multiPlayerGameModeInstance != null)
            {
                _multiPlayerGameModeInstance.RegisterBall(networkBall);
            }
        }

        public static void UnregisterPlayer()
        {
            _connectedPlayers--;

            if (_connectedPlayers < 0)
            {
                _connectedPlayers = 0;
            }
        }
    }
}