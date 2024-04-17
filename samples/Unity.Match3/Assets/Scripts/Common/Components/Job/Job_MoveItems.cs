using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Match3
{
    public class Job_MoveItems : Job_BaseMove
    {
        private const float DelayDuration = 0.25f;
        private const float IntervalDuration = 0.25f;

        private readonly float _delay;
        private readonly IEnumerable<ItemMoveData> _itemsData;

        public Job_MoveItems(IEnumerable<ItemMoveData> items, int delayMultiplier = 0, int executionOrder = 0) : base(executionOrder)
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
                    .Join(itemMoveTween)
                    .PrependInterval(itemMoveTween.Duration() * IntervalDuration);
            }

            await itemsSequence
                .SetDelay(_delay, false)
                .SetEase(Ease.Flash)
                .WithCancellation(cancellationToken);
        }
    }
}