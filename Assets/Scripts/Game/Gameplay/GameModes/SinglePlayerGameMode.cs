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
            return new IControllableObject[] {TopRacket, BottomRacket};
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SignalsManager.Register(_hitRacketSignal.Name, OnHitRacketAction);
            SignalsManager.Register(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Register(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }

        public override void Initialize(Ball ball)
        {
            base.Initialize(ball);
            _hitsCount = 0;
        }

        private void OnHitRacketAction()
        {
            _hitsCount++;
            SignalsManager.Broadcast(_scoreChangeSignal.Name, _hitsCount);

            if (_hitsCount % GameConfiguration.Instance.RacketSizeDecreaseRate == 0)
            {
                DecreaseRacketSize(TopRacket);
                DecreaseRacketSize(BottomRacket);

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

        private static void DecreaseRacketSize(Racket racket)
        {
            var racketSize = racket.Box.transform.localScale;
            var racketSizeX = Mathf.Clamp(racketSize.x - GameConfiguration.Instance.RacketSizeDecreaseValue,
                GameConfiguration.Instance.RacketMinSize, GameConfiguration.Instance.RacketMaxSize);
            racket.Box.transform.localScale = new Vector3(racketSizeX, racketSize.y, racketSize.z);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SignalsManager.Unregister(_hitRacketSignal.Name, OnHitRacketAction);
            SignalsManager.Unregister(_hitTopBoundSignal.Name, OnHitBoundsAction);
            SignalsManager.Unregister(_hitBottomBoundSignal.Name, OnHitBoundsAction);
        }
    }
}