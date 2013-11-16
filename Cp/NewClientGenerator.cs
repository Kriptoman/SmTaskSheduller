using System;
using System.Timers;
using Sm.Cp.Common;

namespace Sm.Cp
{
    public class DefaultTaskGenerator : IDisposable
    {
        private readonly RandomFactory _random;
        private readonly Timer _timer;
        private readonly ApplicationSettingManager _manager;
        private readonly TimeFactory _timeFactory;
        private readonly WorkerTaskFactory _taskFactory;

        public event Action<WorkerTask> ClientCreated = delegate { };

        public DefaultTaskGenerator()
        {
            _manager = new ApplicationSettingManager(null);
            _timeFactory = new TimeFactory();
            _random = new RandomFactory();
            _timer = new Timer();
            _taskFactory = new WorkerTaskFactory();
        }

        public void Init()
        {
            GenerateClient();
        }

        private void GenerateClient()
        {
            var dellay = (-1 * (1.0 / _manager.ClientGeneratorIntensity) * Math.Log(_random.NextDouble())) / _manager.ClientGeneratorFactor;
            var dellayTimeSapn = _timeFactory.TimeSpan(dellay);


            //var dellay = TimeSpan.FromMinutes();
            _timer.Interval = dellayTimeSapn.TotalMilliseconds;
            _timer.Elapsed += OnClientCreatedInternal;
            _timer.Start();
        }

        private void OnClientCreatedInternal(object state, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Elapsed -= OnClientCreatedInternal;
            ClientCreated(_taskFactory.CreateNewTask());
            GenerateClient();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
