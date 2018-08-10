using System;
using System.Collections.Generic;
using Framework.Tools.Gameplay;
using Framework.Tools.Singleton;
using Framework.UI;
using Game.Configuration;
using Game.Gameplay.GameModes;
using Game.Input;
using Game.UI;
using UnityEngine;

namespace Game.Gameplay
{
    public enum GameState
    {
        Idle,
        SinglePlay,
        MultiPlay,
        GameOver
    }

    [Serializable]
    public class GameModeSet
    {
        public GameModeType Type;
        public GameMode GameMode;
    }

    [RequireComponent(typeof(PlayerInputHandler))]
    public class GameController : MonoSingleton<GameController>
    {
        private PlayerInputHandler _playerInputHandler;
        private StateMachine<GameState> _gameStateMachine;
        private GameMode _currentGameMode;

        [SerializeField] private List<GameModeSet> _gameModeSets;
        [SerializeField] private NavigationProvider _navigationProvider;
        [SerializeField] private BallsStorage _ballsStorage;

        public GameModeType GameMode
        {
            get { return _currentGameMode.Type; }
        }

        protected override void Awake()
        {
            base.Awake();

            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _gameStateMachine = CreateStateMachine();

            ActivateIdle();
        }

        private StateMachine<GameState> CreateStateMachine()
        {
            var stateMachine = new StateMachine<GameState>(GameState.Idle);
            stateMachine.AddTransition(GameState.Idle, GameState.SinglePlay, ActivateSinglePlay);
            stateMachine.AddTransition(GameState.SinglePlay, GameState.GameOver, ActivateGameOver);
            stateMachine.AddTransition(GameState.GameOver, GameState.SinglePlay, ActivateSinglePlay);
            stateMachine.AddTransition(GameState.Idle, GameState.MultiPlay, ActivateMultiPlay);
            stateMachine.AddTransition(GameState.MultiPlay, GameState.GameOver, ActivateGameOver);
            stateMachine.AddTransition(GameState.GameOver, GameState.MultiPlay, ActivateMultiPlay);

            return stateMachine;
        }

        public void SetGameState(GameState gameState)
        {
            _gameStateMachine.SetState(gameState);
        }

        public void Replay()
        {
            _gameStateMachine.SetState(GameState.SinglePlay);
        }

        private void ActivateIdle()
        {
            _navigationProvider.OpenScreen<StartPage>();
        }

        private void ActivateSinglePlay()
        {
            ActivateGameMode(GameModeType.SinglePlayer);
        }

        private void ActivateMultiPlay()
        {
            ActivateGameMode(GameModeType.MultiPlayer);
        }

        private void ActivateGameOver()
        {
            _navigationProvider.OpenScreen<ReplayPage>();
        }

        private void ActivateGameMode(GameModeType type)
        {
            _navigationProvider.OpenScreen<PlayPage>();

            if (_currentGameMode != null)
            {
                if (_currentGameMode.Type != type)
                {
                    _currentGameMode.Deactivate();
                    Destroy(_currentGameMode.gameObject);

                    var gameMode = GetGameMode(type);
                    if (gameMode != null)
                    {
                        _currentGameMode = Instantiate(gameMode, transform);
                    }
                }
            }
            else
            {
                var gameMode = GetGameMode(type);
                if (gameMode != null)
                {
                    _currentGameMode = Instantiate(gameMode, transform);
                }
            }

            _currentGameMode.Initialize(_ballsStorage.GetNextBall());
            _playerInputHandler.Initialize(_currentGameMode.GetControllableObjects());
        }

        private GameMode GetGameMode(GameModeType type)
        {
            var gameModeSet = _gameModeSets.Find(g => g.Type == type);
            if (gameModeSet != null)
            {
                return gameModeSet.GameMode;
            }

            Debug.LogError(string.Format("Failed to find appropriate game mode: {0}", type));
            return null;
        }
    }
}