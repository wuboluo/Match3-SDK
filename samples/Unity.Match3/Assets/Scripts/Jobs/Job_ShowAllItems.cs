using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Match3
{
    public class Job_ShowAllItems : Job
    {
        private const float ScaleDuration = 0.5f;
        private readonly List<Game_ItemComponent> _itemComponents;

        public Job_ShowAllItems(List<Game_ItemComponent> itemComponents, int executionOrder = 0) : base(executionOrder)
        {
            _itemComponents = itemComponents;
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var appearItemsSeq = DOTween.Sequence();

            foreach (var item in _itemComponents)
            {
                item.SpriteRenderer.SetAlpha(1);
                item.SetScale(0);
                item.Show();

                _ = appearItemsSeq.Join(item.Transform.DOScale(Vector3.one, ScaleDuration));
            }

            await appearItemsSeq.SetEase(Ease.OutBounce).WithCancellation(cancellationToken);
        }
    }
}