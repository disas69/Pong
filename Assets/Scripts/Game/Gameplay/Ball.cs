using Framework.Extensions;
using Framework.Signals;
using Framework.Tools.Gameplay;
using UnityEngine;

namespace Game.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        private static readonly Vector3[] StartVectors = new[]
        {
            new Vector3(1f, 1f), new Vector3(1f, -1f), new Vector3(-1f, -1f), new Vector3(-1f, 1f)
        };

        private Rigidbody _rigidbody;
        private Pool<ParticleSystem> _effectsPool;

        [SerializeField] private float _startSpeed;
        [SerializeField] private float _bouncingSpeed;
        [SerializeField] private int _effectsPoolCapacity;
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private ParticleSystem _destroyEffect;
        [SerializeField] private Signal _playAudioSignal;
        [SerializeField] private Signal _hitRacketSignal;
        [SerializeField] private Signal _hitTopBoundSignal;
        [SerializeField] private Signal _hitBottomBoundSignal;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _effectsPool = new Pool<ParticleSystem>(_hitEffect, transform.parent, _effectsPoolCapacity);
        }

        public void KickOff()
        {
            _rigidbody.velocity = StartVectors[Random.Range(0, StartVectors.Length)] * _startSpeed;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            var hitPoint = otherCollision.contacts[0].point;

            var racket = otherCollision.gameObject.GetComponentInParent<Racket>();
            if (racket != null)
            {
                var hitFactor = CalculateHitFactor(transform.position, otherCollision.transform.position, otherCollision.collider.bounds.size.x);
                racket.React(hitFactor);

                if (racket.RacketType == RacketType.Top)
                {
                    _rigidbody.velocity = new Vector2(hitFactor, -1).normalized * _bouncingSpeed;
                }
                else if (racket.RacketType == RacketType.Bottom)
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

        private void PlayDestroyEffect(Vector3 hitPoint)
        {
            Instantiate(_destroyEffect, hitPoint, Quaternion.identity, transform.parent);
            SignalsManager.Broadcast(_playAudioSignal.Name, "ball_destroy");
        }

        private static float CalculateHitFactor(Vector2 ballPosition, Vector2 platePosition, float plateHeight)
        {
            return (ballPosition.x - platePosition.x) / plateHeight;
        }
    }
}