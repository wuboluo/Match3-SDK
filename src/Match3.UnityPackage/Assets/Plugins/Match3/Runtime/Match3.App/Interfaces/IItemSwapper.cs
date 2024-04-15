using System.Threading;
using Cysharp.Threading.Tasks;
using Match3.Core.Interfaces;

namespace Match3.App.Interfaces
{
    /// 道具交换
    public interface IItemSwapper<in TGridSlot> where TGridSlot : IGridSlot
    {
        /// 交换动画
        UniTask SwapItemsAsync(TGridSlot gridSlot1, TGridSlot gridSlot2, CancellationToken cancellationToken = default);
    }
}