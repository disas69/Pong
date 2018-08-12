using Framework.UI.Structure.Base.View;
using Framework.Utils;
using Game.Gameplay;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.UI.Popups
{
    public class MultiPlayerStartPopup : Popup
    {
        [UsedImplicitly]
        public void StartHost()
        {
            if (InternetConnection.IsAvailable())
            {
                Network.NetworkManager.IsHost = true;
                StartMultiPlay();
            }
            else
            {
                LogError("No internet connection!");
            }
        }

        [UsedImplicitly]
        public void StartClient()
        {
            if (InternetConnection.IsAvailable())
            {
                Network.NetworkManager.IsHost = false;
                StartMultiPlay();
            }
            else
            {
                LogError("No internet connection!");
            }
        }

        private void StartMultiPlay()
        {
            GameController.Instance.SetGameState(GameState.MultiPlay);
            Close();
        }

        private void LogError(string errorMessage)
        {
            Debug.Log(string.Format("Network error: {0}", errorMessage));
        }
    }
}