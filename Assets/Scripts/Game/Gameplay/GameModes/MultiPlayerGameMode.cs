using Framework.Extensions;
using Framework.Signals;
using Game.Configuration;
using Game.Gameplay.Objects;
using Game.Input;
using Game.Network.Objects;
using Game.UI.Pages;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Game.Gameplay.GameModes
{
    [RequireComponent(typeof(Network.NetworkManager))]
    public class MultiPlayerGameMode : GameMode
    {
        public static bool IsHost;
        private static Racket _racket;
        private static int _connectedPlayers;

        private short _hitBoundsMessage = MsgType.Highest + 1;
        private short _ballSettingsMessage = MsgType.Highest + 2;
        private short _startGameMessage = MsgType.Highest + 3;

        private Network.NetworkManager _networkManager;
        private NetworkClient _networkClient;
        private string _ballSettingsName;

        [SerializeField] private Ball _ballPrefab;
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

            _networkClient = IsHost ? _networkManager.StartHost() : _networkManager.StartClient();
            _networkClient.RegisterHandler(_hitBoundsMessage, msg =>
            {
                GameController.Instance.SetGameState(GameState.GameOver);
            });

            _networkClient.RegisterHandler(_ballSettingsMessage, msg =>
            {
                var stringMsg = msg.ReadMessage<StringMessage>();
                if (stringMsg != null)
                {
                    _ballSettingsName = stringMsg.value;
                }
            });

            _networkClient.RegisterHandler(_startGameMessage, msg =>
            {
                ResetRacket(_racket);
                GameController.Instance.NavigationProvider.OpenScreen<PlayPage>();
            });

            SignalsManager.Register(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Register(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }

        public override void Initialize(BallSettings ballSettings)
        {
            base.Initialize(ballSettings);

            ClearBallRoot();

            _ballSettingsName = ballSettings.Name;

            if (IsHost)
            {
                this.WaitUntil(() => _connectedPlayers == 2, () =>
                {
                    NetworkServer.SendToAll(_ballSettingsMessage, new StringMessage(ballSettings.Name));
                    NetworkServer.SendToAll(_startGameMessage, new EmptyMessage());
                    NetworkServer.Spawn(Instantiate(_ballPrefab, BallRoot).gameObject);
                });
            }
        }

        public void RegisterRacket(NetworkRacket networkRacket)
        {
            networkRacket.gameObject.SetActive(false);
            this.WaitForSeconds(1f, () => RegisterPlayer(networkRacket));
        }

        public void RegisterBall(NetworkBall networkBall)
        {
            networkBall.transform.SetParent(BallRoot);
            networkBall.Setup(_ballSettingsName);

            if (IsHost)
            {
                this.WaitForSeconds(GameConfiguration.Instance.BallKickOffDelay, () => networkBall.Object.KickOff());
            }
        }

        public void UnregisterPlayer()
        {
            _connectedPlayers--;
        }

        private void RegisterPlayer(NetworkRacket networkRacket)
        {
            if (IsHost)
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
            networkRacket.Object.gameObject.SetActive(true);

            _connectedPlayers++;
        }

        private void OnHitBoundsAction()
        {
            NetworkServer.SendToAll(_hitBoundsMessage, new EmptyMessage());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsHost)
            {
                _networkManager.StopHost();
            }
            else
            {
                _networkManager.StopClient();
            }

            SignalsManager.Unregister(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Unregister(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }
    }
}