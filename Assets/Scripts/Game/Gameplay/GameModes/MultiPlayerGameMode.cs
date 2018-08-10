using Game.Input;
using UnityEngine;

namespace Game.Gameplay.GameModes
{
    public class MultiPlayerGameMode : GameMode
    {
        public override GameModeType Type
        {
            get { return GameModeType.MultiPlayer; }
        }

        public override IControllableObject[] GetControllableObjects()
        {
            throw new System.NotImplementedException();
        }
    }
}