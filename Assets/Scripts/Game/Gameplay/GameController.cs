using System;
using System.Collections.Generic;
using Framework.Tools.Gameplay;
using Framework.Tools.Singleton;
using Framework.UI;
using Game.Configuration;
using Game.Data;
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

        [SerializeField] private NavigationProvider _navigationProvider;
        [SerializeField] private BallsStorage _ballsStorage;

        [HideInInspector] public List<GameModeSet> GameModeSets;

        public GameModeType GameMode
        {
            get { return _currentGameMode.Type; }
        }

        protected override void Awake()
        {
            base.Awake();

            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _gameStateMachine = CreateStateMachine();
            _navigationProvider.OpenScreen<StartPage>();

            GameData.Load();
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
            if (GameMode == GameModeType.SinglePlayer)
            {
                SetGameState(GameState.SinglePlay);
            }
            else if (GameMode == GameModeType.MultiPlayer)
            {
                SetGameState(GameState.MultiPlay);
            }
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
                    Destroy(_currentGameMode.gameObject);
                    _currentGameMode = GetGameMode(type);
                }
            }
            else
            {
                _currentGameMode = GetGameMode(type);
            }

            _currentGameMode.Initialize(_ballsStorage.GetNextBall());
            _playerInputHandler.Initialize(_currentGameMode.GetControllableObjects());
        }

        private GameMode GetGameMode(GameModeType type)
        {
            var gameModeSet = GameModeSets.Find(g => g.Type == type);
            if (gameModeSet != null && gameModeSet.GameMode != null)
            {
                return Instantiate(gameModeSet.GameMode, transform);
            }

            Debug.LogError(string.Format("Failed to find appropriate game mode: {0}", type));
            return null;
        }

        private void OnDestroy()
        {
            GameData.Save();
        }
    }
}