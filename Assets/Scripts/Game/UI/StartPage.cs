using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using JetBrains.Annotations;

namespace Game.UI
{
    public class StartPage : Page<PageModel>
    {
        [UsedImplicitly]
        public void StartSinglePlay()
        {
            GameController.Instance.SetGameState(GameState.SinglePlay);
        }
    }
}