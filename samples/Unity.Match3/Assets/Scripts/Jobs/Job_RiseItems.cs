using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Match3
{
    /// 道具向上填充Job
    public class Job_RiseItems : Job_BaseMove
    {
        private const float FadeDuration = 0.15f;
        private const float DelayDuration = 0.35f;
        private const float IntervalDuration = 0.25f;

        private readonly float _delay;
        private readonly IEnumerable<ItemMovePathData> _paths;

        public Job_RiseItems(IEnumerable<ItemMovePathData> paths, int delayMultiplier = 0, int executionOrder = 0) : base(executionOrder)
        {
            _paths = paths;
            _delay = delayMultiplier * DelayDuration;
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var seq = DOTween.Sequence();

            foreach (var itemData in _paths)
            {
                var itemMoveTween = ItemPathMovement(itemData);
                _ = seq
                    .Join(AppearItem(itemData.ItemComponent))
                    .Join(itemMoveTween)
                    .PrependInterval(itemMoveTween.Duration() * IntervalDuration);
            }

            await seq
                .SetDelay(_delay, false)
                .SetEase(Ease.Flash)
                .WithCancellation(cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tween AppearItem(Game_ItemComponent itemComponent)
        {
            itemComponent.SpriteRenderer.SetAlpha(0);
            itemComponent.SetScale(1);
            itemComponent.Show();

            return itemComponent.SpriteRenderer.DOFade(1, FadeDuration);
        }
    }
}