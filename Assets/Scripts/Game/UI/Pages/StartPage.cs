using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using Game.UI.Popups;
using JetBrains.Annotations;

namespace Game.UI.Pages
{
    public class StartPage : Page<PageModel>
    {
        [UsedImplicitly]
        public void StartSinglePlay()
        {
            GameController.Instance.SetGameState(GameState.SinglePlay);
        }

        [UsedImplicitly]
        public void OpenMultiPlayerStartPopup()
        {
            GameController.Instance.ShowPopup<MultiPlayerStartPopup>();
        }
    }
}