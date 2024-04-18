using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Match3
{
    /// 道具交换动画
    public class Job_SwapSlots : Job
    {
        private const float SwapDuration = 0.2f;

        private readonly Game_SlotComponent _slotComponent1;
        private readonly Game_SlotComponent _slotComponent2;

        public Job_SwapSlots(Game_SlotComponent slotComponent1, Game_SlotComponent slotComponent2, int executionOrder = 0) : base(executionOrder)
        {
            _slotComponent1 = slotComponent1;
            _slotComponent2 = slotComponent2;
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var item1 = _slotComponent1.ItemComponent;
            var item2 = _slotComponent2.ItemComponent;

            var item1WorldPosition = item1.GetWorldPosition();
            var item2WorldPosition = item2.GetWorldPosition();

            await DOTween.Sequence()
                .Join(item1.Transform.DOMove(item2WorldPosition, SwapDuration))
                .Join(item2.Transform.DOMove(item1WorldPosition, SwapDuration))
                .SetEase(Ease.Flash)
                .WithCancellation(cancellationToken);

            item1.SetWorldPosition(item2WorldPosition);
            item2.SetWorldPosition(item1WorldPosition);

            _slotComponent1.SetItem(item2);
            _slotComponent2.SetItem(item1);
        }
    }
}