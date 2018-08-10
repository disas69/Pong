using UnityEngine;

namespace Game.Input
{
    public interface IControllableObject
    {
        BoxCollider Box { get; }
        Vector3 Position { get; set; }
    }
}