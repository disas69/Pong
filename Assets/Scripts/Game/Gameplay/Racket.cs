using Game.Input;
using UnityEngine;

namespace Game.Gameplay
{
    public enum RacketType
    {
        Top,
        Bottom
    }

    [RequireComponent(typeof(Animator))]
    public class Racket : MonoBehaviour, IControllableObject
    {
        private readonly int _leftReactHash = Animator.StringToHash("LeftReact");
        private readonly int _middleReactHash = Animator.StringToHash("MiddleReact");
        private readonly int _rightReactHash = Animator.StringToHash("RightReact");

        private Animator _animator;

        [SerializeField] private BoxCollider _box;
        [SerializeField] private RacketType _racketType;
        [SerializeField] private float _leftHitReactFactor = -0.25f;
        [SerializeField] private float _rightHitReactFactor = 0.25f;

        public BoxCollider Box
        {
            get { return _box; }
        }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public RacketType RacketType
        {
            get { return _racketType; }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void React(float hitFactor)
        {
            if (hitFactor < _leftHitReactFactor)
            {
                _animator.SetTrigger(_leftReactHash);
            }
            else if (hitFactor > _rightHitReactFactor)
            {
                _animator.SetTrigger(_rightReactHash);
            }
            else
            {
                _animator.SetTrigger(_middleReactHash);
            }
        }
    }
}