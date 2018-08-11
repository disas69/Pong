using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using JetBrains.Annotations;

namespace Game.UI.Popups
{
    public class MultiPlayerStartPopup : Popup
    {
        [UsedImplicitly]
        public void StartHost()
        {
            Network.NetworkManager.IsHost = true;
            StartMultiPlay();
        }

        [UsedImplicitly]
        public void StartClient()
        {
            Network.NetworkManager.IsHost = false;
            StartMultiPlay();
        }

        private void StartMultiPlay()
        {
            GameController.Instance.SetGameState(GameState.MultiPlay);
            Close();
        }
    }
}