using System;

namespace Sm.Cp.Common
{
    internal class ApplicationLogger
    {
        public void LogTaskDoneAsync(WorkerTask task, Guid workerId, Guid poolId)
        {
            throw new NotImplementedException();
        }

        public void LogTaskRefusedAsync(WorkerTask task, Guid poolId)
        {
            throw new NotImplementedException();
        }
    }
}