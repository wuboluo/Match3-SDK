using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    public abstract class Job : IJob
    {
        protected Job(int executionOrder)
        {
            ExecutionOrder = executionOrder;
        }

        public int ExecutionOrder { get; }

        public abstract UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}