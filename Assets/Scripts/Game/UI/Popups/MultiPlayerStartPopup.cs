using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using Game.Gameplay.GameModes;
using JetBrains.Annotations;

namespace Game.UI.Popups
{
    public class MultiPlayerStartPopup : Popup
    {
        [UsedImplicitly]
        public void StartHost()
        {
            MultiPlayerGameMode.IsHost = true;
            GameController.Instance.SetGameState(GameState.MultiPlay);
            Close();
        }

        [UsedImplicitly]
        public void StartClient()
        {
            MultiPlayerGameMode.IsHost = false;
            GameController.Instance.SetGameState(GameState.MultiPlay);
            Close();
        }
    }
}