using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Tools.Gameplay;
using Framework.Tools.Singleton;
using Framework.UI;
using Game.Configuration;
using Game.Data;
using Game.Gameplay.GameModes;
using Game.Input;
using Game.UI.Pages;
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
        [SerializeField] private BallSettingsStorage _ballSettingsStorage;

        [HideInInspector] public List<GameModeSet> GameModeSets;

        public GameModeType GameMode
        {
            get { return _currentGameMode.Type; }
        }

        public NavigationProvider NavigationProvider
        {
            get { return _navigationProvider; }
        }

        protected override void Awake()
        {
            base.Awake();

            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _gameStateMachine = CreateStateMachine();
            NavigationProvider.OpenScreen<StartPage>();

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
            NavigationProvider.OpenScreen<ReplayPage>();
        }

        private void ActivateGameMode(GameModeType type)
        {
            NavigationProvider.OpenScreen<PlayPage>();

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

            _currentGameMode.Initialize(_ballSettingsStorage.GetNextBallSettings());

            var controllableObjects = _currentGameMode.GetControllableObjects();
            if (controllableObjects == null)
            {
                this.WaitUntil(() => (controllableObjects = _currentGameMode.GetControllableObjects()) != null, () =>
                {
                    _playerInputHandler.Initialize(controllableObjects);
                });
            }
            else
            {
                _playerInputHandler.Initialize(controllableObjects);
            }
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