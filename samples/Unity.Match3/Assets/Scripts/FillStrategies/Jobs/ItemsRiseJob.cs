using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Match3
{
    public class ItemsRiseJob : MoveJob
    {
        private const float FadeDuration = 0.15f;
        private const float DelayDuration = 0.35f;
        private const float IntervalDuration = 0.25f;

        private readonly float _delay;
        private readonly IEnumerable<ItemMoveData> _itemsData;

        public ItemsRiseJob(IEnumerable<ItemMoveData> items, int delayMultiplier = 0, int executionOrder = 0) : base(executionOrder)
        {
            _itemsData = items;
            _delay = delayMultiplier * DelayDuration;
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var itemsSequence = DOTween.Sequence();

            foreach (var itemData in _itemsData)
            {
                var itemMoveTween = CreateItemMoveTween(itemData);
                _ = itemsSequence
                    .Join(CreateItemFadeInTween(itemData.Item))
                    .Join(itemMoveTween).PrependInterval(itemMoveTween.Duration() * IntervalDuration);
            }

            await itemsSequence
                .SetDelay(_delay, false)
                .SetEase(Ease.Flash)
                .WithCancellation(cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Tween CreateItemFadeInTween(UnityItem item)
        {
            item.SpriteRenderer.SetAlpha(0);
            item.SetScale(1);
            item.Show();

            return item.SpriteRenderer.DOFade(1, FadeDuration);
        }
    }
}