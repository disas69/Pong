using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;

namespace Game.UI
{
    public class ReplayPage : Page<PageModel>
    {
        public void Replay()
        {
            GameController.Instance.Replay();
        }
    }
}