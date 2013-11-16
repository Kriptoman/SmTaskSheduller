using System;

namespace Sm.Cp
{
    public class Worker
    {
        public Worker(Guid id)
        {
            Id = id;
            IsEmpty = true;
        }


        public void SetTask(WorkerTask task)
        {
            IsEmpty = false;
            task.Run(Id);
            IsEmpty = true;
        }

        public bool IsEmpty
        {
            get;
            private set;
        }

        public Guid Id { get; private set; }
    }
}
