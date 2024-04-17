using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Match3
{
    public class Job_HideItems : Job
    {
        private const float FadeDuration = 0.15f;
        private const float ScaleDuration = 0.25f;

        private readonly IEnumerable<Game_ItemComponent> _items;

        public Job_HideItems(IEnumerable<Game_ItemComponent> items, int executionOrder = 0) : base(executionOrder)
        {
            _items = items;
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var itemsSequence = DOTween.Sequence();

            foreach (var item in _items)
                _ = itemsSequence
                    .Join(item.Transform.DOScale(Vector3.zero, ScaleDuration))
                    .Join(item.SpriteRenderer.DOFade(0, FadeDuration));

            await itemsSequence.WithCancellation(cancellationToken);

            foreach (var item in _items)
            {
                World.Instance.Root.GetComponent<Game_ItemPoolComponent>().RecycleTileItem(item);
                // item.Hide();
            }
        }
    }
}