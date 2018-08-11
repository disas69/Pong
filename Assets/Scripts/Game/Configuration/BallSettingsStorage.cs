using System;
using System.Collections.Generic;
using Game.Gameplay.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Configuration
{
    [Serializable]
    public class BallSettings
    {
        public string Name;
        public Vector3 Size;
        public float StartSpeed;
        public float BouncingSpeed;
        public BallView View;
    }

    [CreateAssetMenu(fileName = "BallSettingsStorage", menuName = "Configuration/BallSettingsStorage")]
    public class BallSettingsStorage : ScriptableObject
    {
        [NonSerialized] private int _index = -1;

        public List<BallSettings> BallSettings = new List<BallSettings>();

        public BallSettings GetNextBallSettings()
        {
            if (BallSettings.Count > 1)
            {
                int index;
                do
                {
                    index = Random.Range(0, BallSettings.Count);
                }
                while (index == _index);

                _index = index;
                return BallSettings[_index];
            }

            return BallSettings[0];
        }

        public BallSettings GetBallSettingsByName(string settingsName)
        {
            var settings = BallSettings.Find(s => s.Name == settingsName);
            if (settings != null)
            {
                return settings;
            }

            Debug.LogError(string.Format("Failed to find Ball Settings with name {0}", settingsName));
            return null;
        }
    }
}