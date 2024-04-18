using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    public class Game_JobComponent : Component
    {
        /// 执行Jobs
        public UniTask Execute(IEnumerable<IJob> jobs, CancellationToken cancellationToken = default)
        {
            return ExecuteJobsAsync(jobs, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async UniTask ExecuteJobsAsync(IEnumerable<IJob> jobs, CancellationToken cancellationToken = default)
        {
            // 根据每个 Job 的 ExecutionOrder 分组，再按照 ExecutionOrder 排序
            // 具有较低 ExecutionOrder 值的 JobGroup 会首先执行
            var jobGroups = jobs.GroupBy(job => job.ExecutionOrder).OrderBy(group => group.Key);

            // 尽管 Group 内的 Job 是并行执行的，但是各个 Group 之间是按顺序执行的
            // 只有当一个 Group 中的所有 Job 都完成后，才会开始执行下一个组中的Job
            foreach (var jobGroup in jobGroups)
            {
                await UniTask.WhenAll(jobGroup.Select(job => job.ExecuteAsync(cancellationToken)));
            }
        }
    }
}