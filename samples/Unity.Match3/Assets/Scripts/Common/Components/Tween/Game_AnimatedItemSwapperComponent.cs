using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Match3
{
    /// 道具交换动画
    public class Game_AnimatedItemSwapperComponent : Component
    {
        private const float SwapDuration = 0.2f;

        public async UniTask SwapItemsAsync(Game_SlotComponent gridSlot1, Game_SlotComponent gridSlot2, CancellationToken cancellationToken = default)
        {
            var item1 = gridSlot1.ItemComponent;
            var item2 = gridSlot2.ItemComponent;

            var item1WorldPosition = item1.GetWorldPosition();
            var item2WorldPosition = item2.GetWorldPosition();

            await DOTween.Sequence()
                .Join(item1.Transform.DOMove(item2WorldPosition, SwapDuration))
                .Join(item2.Transform.DOMove(item1WorldPosition, SwapDuration))
                .SetEase(Ease.Flash)
                .WithCancellation(cancellationToken);

            item1.SetWorldPosition(item2WorldPosition);
            item2.SetWorldPosition(item1WorldPosition);

            gridSlot1.SetItem(item2);
            gridSlot2.SetItem(item1);
        }
    }
}