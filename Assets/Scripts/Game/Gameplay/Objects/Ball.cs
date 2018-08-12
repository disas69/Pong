using System;
using Framework.Extensions;
using Framework.Signals;
using Framework.Tools.Gameplay;
using Game.Configuration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Gameplay.Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        private static readonly Vector3[] StartVectors = new[]
        {
            new Vector3(1f, 1f), new Vector3(1f, -1f), new Vector3(-1f, -1f), new Vector3(-1f, 1f)
        };

        private Rigidbody _rigidbody;
        private BallView _ballView;
        private Pool<ParticleSystem> _effectsPool;
        private float _startSpeed;
        private float _bouncingSpeed;
        private bool _destroyEffectPlayed;

        public event Action Destroyed;

        [SerializeField] private int _effectsPoolCapacity;
        [SerializeField] private Signal _playAudioSignal;
        [SerializeField] private Signal _hitRacketSignal;
        [SerializeField] private Signal _hitTopBoundSignal;
        [SerializeField] private Signal _hitBottomBoundSignal;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Setup(BallSettings ballSettings)
        {
            _startSpeed = ballSettings.StartSpeed;
            _bouncingSpeed = ballSettings.BouncingSpeed;
            _ballView = Instantiate(ballSettings.View, transform);
            _effectsPool = new Pool<ParticleSystem>(_ballView.HitEffect, transform.parent, _effectsPoolCapacity);

            transform.localScale = ballSettings.Size;
        }

        public void KickOff()
        {
            _rigidbody.velocity = StartVectors[Random.Range(0, StartVectors.Length)] * _startSpeed;
        }

        public void PlayDestroyEffect(Vector3 hitPoint)
        {
            if (!_destroyEffectPlayed)
            {
                Instantiate(_ballView.DestroyEffect, hitPoint, Quaternion.identity, transform.parent);
                SignalsManager.Broadcast(_playAudioSignal.Name, "ball_destroy");
                _destroyEffectPlayed = true;
            }
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            var hitPoint = otherCollision.contacts[0].point;

            var racket = otherCollision.gameObject.GetComponentInParent<Racket>();
            if (racket != null)
            {
                var hitFactor = CalculateHitFactor(transform.position, otherCollision.transform.position, otherCollision.collider.bounds.size.x);
                racket.React(hitFactor);

                if (racket.Type == RacketType.Top)
                {
                    _rigidbody.velocity = new Vector2(hitFactor, -1).normalized * _bouncingSpeed;
                }
                else if (racket.Type == RacketType.Bottom)
                {
                    _rigidbody.velocity = new Vector2(hitFactor, 1).normalized * _bouncingSpeed;
                }

                SignalsManager.Broadcast(_hitRacketSignal.Name);
            }
            else
            {
                var bound = otherCollision.gameObject.GetComponent<Bound>();
                if (bound != null)
                {
                    if (HitVerticalBounds(bound.Type))
                    {
                        PlayDestroyEffect(hitPoint);
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            PlayHitEffects(hitPoint);
        }

        private bool HitVerticalBounds(BoundType boundType)
        {
            var hit = false;

            switch (boundType)
            {
                case BoundType.Top:
                {
                    hit = true;
                    SignalsManager.Broadcast(_hitTopBoundSignal.Name);
                    break;
                }
                case BoundType.Bottom:
                {
                    hit = true;
                    SignalsManager.Broadcast(_hitBottomBoundSignal.Name);
                    break;
                }
            }

            return hit;
        }

        private void PlayHitEffects(Vector3 hitPoint)
        {
            var hitEffect = _effectsPool.GetNext();
            hitEffect.transform.position = hitPoint;
            this.WaitForSeconds(hitEffect.main.duration, () => _effectsPool.Return(hitEffect));
            SignalsManager.Broadcast(_playAudioSignal.Name, "ball_hit");
        }
        
        private static float CalculateHitFactor(Vector2 ballPosition, Vector2 platePosition, float plateHeight)
        {
            return (ballPosition.x - platePosition.x) / plateHeight;
        }

        private void OnDestroy()
        {
            Destroyed.SafeInvoke();
        }
    }
}