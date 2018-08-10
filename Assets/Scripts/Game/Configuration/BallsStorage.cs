using System;
using System.Collections.Generic;
using Game.Gameplay.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Configuration
{
    [CreateAssetMenu(fileName = "BallsStorage", menuName = "Configuration/BallsStorage")]
    public class BallsStorage : ScriptableObject
    {
        [NonSerialized] private int _index = -1;

        public List<Ball> Balls = new List<Ball>();

        public Ball GetNextBall()
        {
            if (Balls.Count > 1)
            {
                int index;
                do
                {
                    index = Random.Range(0, Balls.Count);
                }
                while (index == _index);

                _index = index;
                return Balls[_index];
            }

            return Balls[0];
        }
    }
}