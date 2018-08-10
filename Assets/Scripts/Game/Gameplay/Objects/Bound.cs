using UnityEngine;

namespace Game.Gameplay.Objects
{
    public enum BoundType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [RequireComponent(typeof(BoxCollider))]
    public class Bound : MonoBehaviour
    {
        [SerializeField] private BoundType _type;

        public BoundType Type
        {
            get { return _type; }
        }
    }
}