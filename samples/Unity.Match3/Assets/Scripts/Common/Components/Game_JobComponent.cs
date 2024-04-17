using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    public class Game_JobComponent : Component
    {
        /// 执行Jobs
        public UniTask ExecuteJobsAsync(IEnumerable<IJob> jobs, CancellationToken cancellationToken = default)
        {
            return JobsExecutor.ExecuteJobsAsync(jobs, cancellationToken);
        }
    }
}