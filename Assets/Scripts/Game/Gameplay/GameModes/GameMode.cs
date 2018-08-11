using Game.Configuration;
using Game.Gameplay.Objects;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay.GameModes
{
    public enum GameModeType
    {
        SinglePlayer,
        MultiPlayer
    }

    public abstract class GameMode : MonoBehaviour
    {
        [SerializeField] private Vector3 _defaultTopRacketPosition;
        [SerializeField] private Vector3 _defaultBottomRacketPosition;
        [SerializeField] private Transform _racketRoot;
        [SerializeField] private Transform _ballRoot;

        public abstract GameModeType Type { get; }
        
        protected Transform RacketRoot
        {
            get { return _racketRoot; }
        }

        protected Transform BallRoot
        {
            get { return _ballRoot; }
        }

        public abstract IControllableObject[] GetControllableObjects();

        protected virtual void Awake()
        {
        }

        public virtual void Initialize(BallSettings ballSettings)
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected void ClearBallRoot()
        {
            var count = BallRoot.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(BallRoot.GetChild(i).gameObject);
            }
        }

        protected Vector3 GetStartPosition(RacketType type)
        {
            if (type == RacketType.Top)
            {
                return _defaultTopRacketPosition;
            }

            return _defaultBottomRacketPosition;
        }

        protected virtual void ResetRacket(Racket racket)
        {
            racket.transform.SetParent(RacketRoot);

            racket.transform.position = racket.Type == RacketType.Top
                ? _defaultTopRacketPosition
                : _defaultBottomRacketPosition;

            var racketSize = racket.Box.transform.localScale;
            racket.Box.transform.localScale = new Vector3(GameConfiguration.Instance.RacketMaxSize, racketSize.y,
                racketSize.z);
        }
    }
}