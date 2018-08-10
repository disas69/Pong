using Framework.Localization;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Data;
using Game.Gameplay;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ReplayPage : Page<PageModel>
    {
        [SerializeField] private Text _bestScoreText;

        public override void OnEnter()
        {
            base.OnEnter();
            _bestScoreText.text = string.Format(LocalizationManager.GetString("best_score"), GameData.Data.BestScore);
        }

        [UsedImplicitly]
        public void Replay()
        {
            GameController.Instance.Replay();
        }
    }
}