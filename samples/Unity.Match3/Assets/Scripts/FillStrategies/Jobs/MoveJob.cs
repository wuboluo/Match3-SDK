using System.Runtime.CompilerServices;
using DG.Tweening;

namespace Match3
{
    public abstract class MoveJob : Job
    {
        private const float MoveDuration = 0.25f;

        protected MoveJob(int executionOrder) : base(executionOrder)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static Tween CreateItemMoveTween(ItemMoveData data)
        {
            return data.ItemComponent.Transform.DOPath(data.WorldPositions, MoveDuration);
        }
    }
}