using UnityEngine;

namespace Game.Gameplay.Objects
{
    public class BallView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private ParticleSystem _destroyEffect;

        public ParticleSystem HitEffect
        {
            get { return _hitEffect; }
        }

        public ParticleSystem DestroyEffect
        {
            get { return _destroyEffect; }
        }
    }
}