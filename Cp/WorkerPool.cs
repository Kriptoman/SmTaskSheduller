using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sm.Cp.Common;

namespace Sm.Cp
{
    public class WorkerPool : IEnumerable<Worker>, IDisposable
    {
        public Guid Id { get; private set; }

        private readonly int _cacheMaxSize;

        private readonly ConcurrentBag<Worker> _pool;

        private readonly ConcurrentQueue<WorkerTask> _cache;

        private readonly ApplicationLogger _applicationLogger;

        public event Action<WorkerTask> TaskDone = delegate { };

        public event Action<WorkerTask, Guid> TaskRefused = delegate { };

        public WorkerPool(Guid id, int cacheMaxSize, int workersCount)
        {
            _applicationLogger = new ApplicationLogger();

            _cacheMaxSize = cacheMaxSize;
            var workerFactory = new WorkerFactory();

            _pool = new ConcurrentBag<Worker>(Enumerable.Range(0, workersCount).Select(indx => workerFactory.NewWorker(Id)));
            _cache = new ConcurrentQueue<WorkerTask>();

            Id = id;
        }

        public void AddTask(WorkerTask task)
        {
            var emptyWorker = _pool.FirstOrDefault(worker => worker.IsEmpty);
            if (emptyWorker != null)
            {
                emptyWorker.SetTask(task);
                TaskDoneInternal(task, emptyWorker.Id);
            }
            else if (_cache.Count < _cacheMaxSize)
            {
                _cache.Enqueue(task);
            }
            else
            {
                TaskRefused(task, Id);
                _applicationLogger.LogTaskRefusedAsync(task, Id);
            }
        }

        private void TaskDoneInternal(WorkerTask task, Guid workerId)
        {
            Task.Factory.StartNew(() => TaskDone(task));
            _applicationLogger.LogTaskDoneAsync(task, workerId, Id);

            WorkerTask dequeuedTask;

            if (_cache.TryDequeue(out dequeuedTask))
            {
                AddTask(dequeuedTask);
            }
        }

        public IEnumerator<Worker> GetEnumerator()
        {
            return _pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
        }
    }
}