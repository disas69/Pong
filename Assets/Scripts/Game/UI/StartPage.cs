using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;

namespace Game.UI
{
    public class StartPage : Page<PageModel>
    {
        public void StartSinglePlay()
        {
            GameController.Instance.SetGameState(GameState.SinglePlay);
        }
    }
}