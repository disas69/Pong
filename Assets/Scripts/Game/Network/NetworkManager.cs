using System.Collections.Generic;
using Game.Gameplay.GameModes;
using Game.Network.Objects;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

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

        public void StartMatch(NetworkMatch.DataResponseDelegate<MatchInfo> onMatchCreateCallback, NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> onMatchListCallback)
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

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if (_multiPlayer != null)
            {
                _multiPlayer.UnregisterPlayer();
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            if (_multiPlayer != null)
            {
                _multiPlayer.UnregisterPlayer();
            }
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