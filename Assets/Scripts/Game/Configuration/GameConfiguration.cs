using Framework.Attributes;
using Framework.Tools.Singleton;
using UnityEngine;

namespace Game.Configuration
{
    [ResourcePath("GameConfiguration")]
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Configuration/GameConfiguration")]
    public class GameConfiguration : ScriptableSingleton<GameConfiguration>
    {
        [Header("Rackets settings")]
        public float RacketMaxSize;
        public float RacketMinSize;
        public float RacketSizeDecreaseValue;
        public int RacketSizeDecreaseRate;

        [Header("Ball settings")]
        public float BallKickOffDelay;
    }
}