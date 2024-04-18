using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    // Job：可并行的工作单元
    public interface IJob
    {
        /// 指令
        int ExecutionOrder { get; }

        /// 执行
        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}