using Framework.Extensions;
using Framework.Signals;
using Game.Configuration;
using Game.Data;
using Game.Gameplay.Objects;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay.GameModes
{
    public class SinglePlayerGameMode : GameMode
    {
        private int _hitsCount;
        private Racket _topRacket;
        private Racket _bottomRacket;

        [SerializeField] private Racket _racketPrefab;
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Signal _hitRacketSignal;
        [SerializeField] private Signal _hitTopBoundSignal;
        [SerializeField] private Signal _hitBottomBoundSignal;
        [SerializeField] private Signal _scoreChangeSignal;
        [SerializeField] private Signal _racketsSizeChangeSignal;

        public override GameModeType Type
        {
            get { return GameModeType.SinglePlayer; }
        }

        public override IControllableObject[] GetControllableObjects()
        {
            return new IControllableObject[] {_topRacket, _bottomRacket};
        }

        protected override void Awake()
        {
            base.Awake();

            _topRacket = Instantiate(_racketPrefab, RacketRoot);
            _topRacket.Setup(RacketType.Top);

            _bottomRacket = Instantiate(_racketPrefab, RacketRoot);
            _bottomRacket.Setup(RacketType.Bottom);

            SignalsManager.Register(_hitRacketSignal.Name, OnHitRacketAction);
            SignalsManager.Register(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Register(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }

        public override void Initialize(BallSettings ballSettings)
        {
            _hitsCount = 0;

            ResetRacket(_topRacket);
            ResetRacket(_bottomRacket);
            SetupBall(ballSettings);
        }

        private void OnHitRacketAction()
        {
            _hitsCount++;
            SignalsManager.Broadcast(_scoreChangeSignal.Name, _hitsCount);

            if (_hitsCount % GameConfiguration.Instance.RacketSizeDecreaseRate == 0)
            {
                DecreaseRacketSize(_topRacket);
                DecreaseRacketSize(_bottomRacket);

                SignalsManager.Broadcast(_racketsSizeChangeSignal.Name);
            }
        }

        private void OnHitBoundsAction()
        {
            if (_hitsCount > GameData.Data.BestScore)
            {
                GameData.Data.BestScore = _hitsCount;
                GameData.Save();
            }

            GameController.Instance.SetGameState(GameState.GameOver);
        }

        private void SetupBall(BallSettings ballSettings)
        {
            ClearBallRoot();

            var ballInstance = Instantiate(_ballPrefab, BallRoot);
            ballInstance.Setup(ballSettings);

            this.WaitForSeconds(GameConfiguration.Instance.BallKickOffDelay, () => ballInstance.KickOff());
        }

        private static void DecreaseRacketSize(Racket racket)
        {
            var racketSize = racket.Box.transform.localScale;
            var racketSizeX = Mathf.Clamp(racketSize.x - GameConfiguration.Instance.RacketSizeDecreaseValue,
                GameConfiguration.Instance.RacketMinSize, GameConfiguration.Instance.RacketMaxSize);
            racket.Box.transform.localScale = new Vector3(racketSizeX, racketSize.y, racketSize.z);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SignalsManager.Unregister(_hitRacketSignal.Name, OnHitRacketAction);
            SignalsManager.Unregister(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Unregister(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }
    }
}