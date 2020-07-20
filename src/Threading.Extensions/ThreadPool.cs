using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Threading.Extensions
{
    public interface IThreadPool<TState>
    {
        void Enqueue(Action<TState> action, TState state);
    }

    public class ThreadPool<TState> : IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly BlockingCollection<(Action<TState>, TState)> _queue = new BlockingCollection<(Action<TState>, TState)>(new ConcurrentQueue<(Action<TState>, TState)>());

        public ThreadPool(int numberOfThreads, string poolName)
        {
            for (int i = 0; i < numberOfThreads; i++)
            {
                var thread = new Thread(Work)
                {
                    Name = $"{poolName}-{i}"
                };

                thread.Start();
            }
        }

        public void Enqueue(Action<TState> action, TState state)
        {
            _queue.Add((action, state));
        }

        private void Work()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    (Action<TState> action, TState state) = _queue.Take(_cts.Token);

                    try
                    {
                        action(state);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch
                {
                    if (_cts.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _queue.CompleteAdding();
        }
    }
}
