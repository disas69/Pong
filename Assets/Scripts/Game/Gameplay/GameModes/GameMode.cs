using Framework.Extensions;
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
        private Vector3 _defaultTopRacketPosition;
        private Vector3 _defaultBottomRacketPosition;

        [SerializeField] private Racket _topRacket;
        [SerializeField] private Racket _bottomRacket;
        [SerializeField] private Transform _ballRoot;

        public abstract GameModeType Type { get; }

        protected Racket TopRacket
        {
            get { return _topRacket; }
        }

        protected Racket BottomRacket
        {
            get { return _bottomRacket; }
        }

        public abstract IControllableObject[] GetControllableObjects();

        protected virtual void Awake()
        {
            _defaultTopRacketPosition = TopRacket.transform.position;
            _defaultBottomRacketPosition = BottomRacket.transform.position;
        }

        protected virtual void OnEnable()
        {
        }

        public virtual void Initialize(Ball ball)
        {
            SetupBall(ball);
            SetupRackets();
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDisable()
        {
        }

        private void SetupBall(Ball ball)
        {
            var count = _ballRoot.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(_ballRoot.GetChild(i).gameObject);
            }

            var ballInstance = Instantiate(ball, _ballRoot);
            this.WaitForSeconds(GameConfiguration.Instance.BallKickOffDelay, () => ballInstance.KickOff());
        }

        private void SetupRackets()
        {
            TopRacket.transform.position = _defaultTopRacketPosition;
            BottomRacket.transform.position = _defaultBottomRacketPosition;

            var topRacketSize = TopRacket.Box.transform.localScale;
            TopRacket.Box.transform.localScale = new Vector3(GameConfiguration.Instance.RacketMaxSize, topRacketSize.y,
                topRacketSize.z);

            var bottomRacketSize = BottomRacket.Box.transform.localScale;
            BottomRacket.Box.transform.localScale = new Vector3(GameConfiguration.Instance.RacketMaxSize,
                bottomRacketSize.y, bottomRacketSize.z);
        }
    }
}