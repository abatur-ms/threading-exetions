using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Threading.Extensions
{
    public class ThreadPoolTaskScheduler : TaskScheduler
    {
        private readonly IThreadPool<Task> _threadPool;
        
        public ThreadPoolTaskScheduler(IThreadPool<Task> threadPool)
        {
            _threadPool = threadPool;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotSupportedException();
        }

        protected override void QueueTask(Task task)
        {
            _threadPool.Enqueue(ExecuteTask, task);
        }

        private void ExecuteTask(Task task)
        {
            TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}