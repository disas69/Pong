using System.Collections.Generic;
using Framework.Extensions;
using Framework.Signals;
using Game.Configuration;
using Game.Gameplay.Objects;
using Game.Input;
using Game.Network;
using Game.Network.Objects;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;

namespace Game.Gameplay.GameModes
{
    [RequireComponent(typeof(Network.NetworkManager))]
    public class MultiPlayerGameMode : GameMode
    {
        private static Racket _racket;
        private static int _connectedPlayers;

        private Network.NetworkManager _networkManager;
        private NetworkClient _networkClient;
        private string _ballSettingsName;

        [SerializeField] private NetworkBall _ballPrefab;
        [SerializeField] private Signal _hitTopBoundSignal;
        [SerializeField] private Signal _hitBottomBoundSignal;

        public override GameModeType Type
        {
            get { return GameModeType.MultiPlayer; }
        }

        public override IControllableObject[] GetControllableObjects()
        {
            if (_racket == null)
            {
                return null;
            }

            return new IControllableObject[] {_racket};
        }

        protected override void Awake()
        {
            base.Awake();

            _connectedPlayers = 0;
            _networkManager = GetComponent<Network.NetworkManager>();
            _networkManager.StartMatch(OnMatchCreate, OnMatchList);

            SignalsManager.Register(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Register(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }

        private void OnMatchCreate(bool success, string extendedinfo, MatchInfo responsedata)
        {
            if (success)
            {
                _networkClient = _networkManager.StartHost(responsedata);
                RegisterMessageHandlers(_networkClient);
            }
            else
            {
                LogErrorAndQuit("Failed to create a match");
            }
        }

        private void OnMatchList(bool success, string extendedinfo, List<MatchInfoSnapshot> responsedata)
        {
            if (success)
            {
                if (responsedata.Count > 0)
                {
                    _networkManager.JoinMatch(responsedata[0].networkId, OnJoinedMatch);
                }
                else
                {
                    LogErrorAndQuit("Failed to find available matches");
                }
            }
            else
            {
                LogErrorAndQuit("Failed to get available matches list");
            }
        }

        private void OnJoinedMatch(bool success, string extendedinfo, MatchInfo responsedata)
        {
            if (success)
            {
                _networkClient = _networkManager.StartClient(responsedata);
                RegisterMessageHandlers(_networkClient);
            }
            else
            {
                LogErrorAndQuit("Failed to join the match");
            }
        }

        private void RegisterMessageHandlers(NetworkClient networkClient)
        {
            networkClient.RegisterHandler(NetworkMessages.GameOverMessage,
                msg => { GameController.Instance.SetGameState(GameState.GameOver); });

            networkClient.RegisterHandler(NetworkMessages.BallSettingsMessage, msg =>
            {
                var stringMsg = msg.ReadMessage<StringMessage>();
                if (stringMsg != null)
                {
                    _ballSettingsName = stringMsg.value;
                }
            });

            networkClient.RegisterHandler(NetworkMessages.StartGameMessage, msg =>
            {
                ResetRacket(_racket);
                GameController.Instance.Replay();
            });
        }

        public override void Initialize(BallSettings ballSettings)
        {
            base.Initialize(ballSettings);

            ClearBallRoot();

            if (Network.NetworkManager.IsHost)
            {
                _ballSettingsName = ballSettings.Name;

                this.WaitUntil(() => _connectedPlayers == 2, () =>
                {
                    NetworkServer.SendToAll(NetworkMessages.BallSettingsMessage, new StringMessage(ballSettings.Name));
                    NetworkServer.SendToAll(NetworkMessages.StartGameMessage, new EmptyMessage());
                    NetworkServer.Spawn(Instantiate(_ballPrefab, BallRoot).gameObject);
                });
            }
        }

        public void RegisterBall(NetworkBall networkBall)
        {
            networkBall.transform.SetParent(BallRoot);
            networkBall.Setup(_ballSettingsName);

            if (Network.NetworkManager.IsHost)
            {
                this.WaitForSeconds(GameConfiguration.Instance.BallKickOffDelay, () => networkBall.Object.KickOff());
            }
        }

        public void RegisterRacket(NetworkRacket networkRacket)
        {
            networkRacket.gameObject.SetActive(false);
            this.WaitForSeconds(1f, () => RegisterPlayer(networkRacket));
        }

        private void RegisterPlayer(NetworkRacket networkRacket)
        {
            if (Network.NetworkManager.IsHost)
            {
                if (networkRacket.isLocalPlayer)
                {
                    networkRacket.Object.Setup(RacketType.Bottom);
                    _racket = networkRacket.Object;
                }
                else
                {
                    networkRacket.Object.Setup(RacketType.Top);
                }
            }
            else
            {
                if (networkRacket.isLocalPlayer)
                {
                    networkRacket.Object.Setup(RacketType.Top);
                    _racket = networkRacket.Object;
                }
                else
                {
                    networkRacket.Object.Setup(RacketType.Bottom);
                }
            }

            ResetRacket(networkRacket.Object);
            networkRacket.gameObject.SetActive(true);
            //networkRacket.SetReady(true, networkRacket.Object.Position);

            _connectedPlayers++;
        }

        public void UnregisterPlayer()
        {
            _connectedPlayers--;
        }

        private void OnHitBoundsAction()
        {
            NetworkServer.SendToAll(NetworkMessages.GameOverMessage, new EmptyMessage());
        }

        private void LogErrorAndQuit(string errorMessage)
        {
            Debug.Log(string.Format("Network error: {0}", errorMessage));
            GameController.Instance.SetGameState(GameState.Idle);
        }

        public override void Deactivate()
        {
            if (_networkManager.isNetworkActive)
            {
                _networkManager.Disconnect();
                this.WaitUntil(() => !_networkManager.isNetworkActive, () => Deactivated = true);
            }
            else
            {
                Deactivated = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SignalsManager.Unregister(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Unregister(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }
    }
}