using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Pages
{
    public class MultiReplayPage : Page<PageModel>
    {
        [SerializeField] private Text _tapMessage;
        [SerializeField] private Text _waitMessage;

        public override void OnEnter()
        {
            base.OnEnter();

            if (Network.NetworkManager.IsHost)
            {
                _tapMessage.gameObject.SetActive(true);
                _waitMessage.gameObject.SetActive(false);
            }
            else
            {
                _tapMessage.gameObject.SetActive(false);
                _waitMessage.gameObject.SetActive(true);
            }
        }

        [UsedImplicitly]
        public void Replay()
        {
            if (Network.NetworkManager.IsHost)
            {
                GameController.Instance.Replay();
            }
        }

        [UsedImplicitly]
        public void Exit()
        {
            GameController.Instance.SetGameState(GameState.Idle);
        }
    }
}