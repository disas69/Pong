using System.Collections;
using Framework.Extensions;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayPage : Page<PageModel>
    {
        private Coroutine _overlayTransitionCoroutine;

        [SerializeField] private Text _scoreText;
        [SerializeField] private float _overlayTransitionSpeed;
        [SerializeField] private CanvasGroup _overlay;

        public override void OnEnter()
        {
            base.OnEnter();
            _scoreText.gameObject.SetActive(false);
        }

        protected override IEnumerator InTransition()
        {
            _overlayTransitionCoroutine = StartCoroutine(ShowOverlay());
            yield return _overlayTransitionCoroutine;
        }

        private IEnumerator ShowOverlay()
        {
            _overlay.gameObject.SetActive(true);
            _overlay.alpha = 1f;

            while (_overlay.alpha > 0f)
            {
                _overlay.alpha -= _overlayTransitionSpeed * 2f * Time.deltaTime;
                yield return null;
            }

            _overlay.alpha = 0f;
            _overlay.gameObject.SetActive(false);
            _overlayTransitionCoroutine = null;
        }

        [UsedImplicitly]
        public void OnScoreChanged(int score)
        {
            _scoreText.gameObject.SetActive(false);
            _scoreText.text = score.ToString();
            _scoreText.gameObject.SetActive(true);
        }

        public override void OnExit()
        {
            _overlay.gameObject.SetActive(false);
            this.SafeStopCoroutine(_overlayTransitionCoroutine);
            base.OnExit();
        }
    }
}