using System.Collections.Generic;
using Game.Gameplay;
using UnityEngine;

namespace Game.Configuration
{
    [CreateAssetMenu(fileName = "BallsStorage", menuName = "Configuration/BallsStorage")]
    public class BallsStorage : ScriptableObject
    {
        public List<Ball> Balls = new List<Ball>();

        public Ball GetNextBall()
        {
            return Balls[Random.Range(0, Balls.Count)];
        }
    }
}