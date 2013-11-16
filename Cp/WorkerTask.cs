using System;
using System.Threading;
using Cp;

namespace Sm.Cp
{
    public class WorkerTask
    {
        public Guid Id { get; private set; }

        public WorkerTask(Guid id)
        {
            Id = id;
        }

        public void Run(Guid workerId)
        {
            var processingTime = WorkerTaskTimeGenerator.GetProcessingTime(Id, workerId);
            Thread.Sleep(processingTime);
        }
    }
}