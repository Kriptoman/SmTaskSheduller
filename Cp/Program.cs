using System;
using System.Threading;
using Sm.Cp.Common;

namespace Sm.Cp
{
    class Program : IDisposable
    {
        private readonly ApplicationSettingManager _manager;
        private readonly PoolFactory _poolFactory;
        private readonly DefaultTaskGenerator _clientGenerator;
        private readonly TimeFactory _timeFactory;

        private Program(string[] args)
        {
            _manager = new ApplicationSettingManager(args);

            _poolFactory = new PoolFactory();
            _clientGenerator = new DefaultTaskGenerator();
            _timeFactory = new TimeFactory();
        }

        public static void Main(string[] args)
        {
            using (var programm = new Program(args))
            {
                programm.Run();
            }
        }

        private void Run()
        {
            var poolCount = _manager.PoolCount;

            if (poolCount <= 0)
            {
                return;
            }

            var pool = _poolFactory.NewPool();
            _clientGenerator.ClientCreated += pool.AddTask;

            for (var i = 1; i < poolCount; i++)
            {
                var newPool = _poolFactory.NewPool();
                pool.TaskDone += newPool.AddTask;
                pool = newPool;
            }

            _clientGenerator.Init();
            Thread.Sleep(_timeFactory.TimeSpan(50000));

            Dispose();
        }

        public void Dispose()
        {
            _clientGenerator.Dispose();
        }
    }
}
