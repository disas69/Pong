using System;
using System.Collections.Generic;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay.Objects
{
    public enum RacketType
    {
        Top,
        Bottom
    }

    [Serializable]
    public class RacketSettings
    {
        public RacketType Type;
        public Material Material;
        public RuntimeAnimatorController AnimatorController;
    }

    [RequireComponent(typeof(Animator))]
    public class Racket : MonoBehaviour, IControllableObject
    {
        private readonly int _leftReactHash = Animator.StringToHash("LeftReact");
        private readonly int _middleReactHash = Animator.StringToHash("MiddleReact");
        private readonly int _rightReactHash = Animator.StringToHash("RightReact");

        private Animator _animator;
        private MeshRenderer _meshRenderer;

        [SerializeField] private BoxCollider _box;
        [SerializeField] private float _leftHitReactFactor = -0.25f;
        [SerializeField] private float _rightHitReactFactor = 0.25f;
        [SerializeField] private List<RacketSettings> _racketSettings;

        public RacketType Type { get; private set; }

        public BoxCollider Box
        {
            get { return _box; }
        }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void Setup(RacketType type)
        {
            Type = type;

            var settings = _racketSettings.Find(s => s.Type == Type);
            if (settings != null)
            {
                _meshRenderer.material = settings.Material;
                _animator.runtimeAnimatorController = settings.AnimatorController;
            }
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