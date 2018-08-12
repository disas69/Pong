using System.Collections;
using Framework.Extensions;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Gameplay;
using Game.Tools;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.UI.Pages
{
    public class MultiPlayPage : Page<PageModel>
    {
        private Coroutine _overlayTransitionCoroutine;

        [SerializeField] private TouchHandler _touchHandler;
        [SerializeField] private float _overlayTransitionSpeed;
        [SerializeField] private CanvasGroup _overlay;

        public override void OnEnter()
        {
            base.OnEnter();

            if (!Network.NetworkManager.IsReady)
            {
                ActivateTouchOverlay(true);
                this.WaitUntil(() => Network.NetworkManager.IsReady, () => ActivateTouchOverlay(false));
            }
            else
            {
                ActivateTouchOverlay(false);
            }
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

        private void ActivateTouchOverlay(bool value)
        {
            _touchHandler.gameObject.SetActive(value);
        }

        [UsedImplicitly]
        public void Exit()
        {
            GameController.Instance.SetGameState(GameState.Idle);
        }

        public override void OnExit()
        {
            _overlay.gameObject.SetActive(false);
            this.SafeStopCoroutine(_overlayTransitionCoroutine);
            base.OnExit();
        }
    }
}