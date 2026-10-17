using Assets.UIFramework.Panel;
using Assets.UIFrameworkExamples.Scripes.Extras;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    public  class ToastPanelController:PanelController
    {
        [SerializeField] private RectTransform toastRect = null;
        [SerializeField] private float toastDuration = 0.5f;
        [SerializeField] private float toastPause = 2f;
        [SerializeField] private Ease toastEase = Ease.Linear;

        private bool isToalting;

        protected override void AddListeners()
        {
            Signals.Get<PlayerDataUpdatedSignal>().AddListener(OnDataUpdated);
        }

        protected override void RemoveListeners()
        {
            Signals.Get<PlayerDataUpdatedSignal>().RemoveListener(OnDataUpdated);
        }
        private void OnDataUpdated(List<PlayerDateEntry> obj)
        {
            if (isToalting) return;

            StartCoroutine(YieldForDotween());
        }

        private IEnumerator YieldForDotween()
        {
            yield return null;
            isToalting = true;

            Sequence seq = DOTween.Sequence();
            seq.Append(toastRect.DOAnchorPosY(0f, toastDuration).SetEase(toastEase));
            seq.AppendInterval(toastPause);
            seq.Append(toastRect.DOAnchorPosY(toastRect.rect.height, toastDuration).SetEase(toastEase));
            seq.OnComplete(() => isToalting = false);

            seq.Play();

        }
    }
}
