using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Confluent.Kafka;
using Microsoft.Extensions.Logging;


namespace HardwareService
{
    public class PerfHelper
    {
        private Stopwatch watch = new Stopwatch();
        private ILogger _logger;
        private int messages = 0;
        private BackgroundWorker _thread;

        public PerfHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void StartProcessingCounters(Null id )
        {

        }

        public void EndProcessingCounters(Null id)
        {
            messages++;
        }

        public void  Startup()
        {
            _thread = new BackgroundWorker();
            _thread.DoWork += ThreadFunc;
            _thread.RunWorkerAsync();
        }

        public void Stop()
        {
            
        }

        private void ThreadFunc(object obj, DoWorkEventArgs doWorkEventArgs)
        {
            while (true)
            {
                var curMessageCount = messages;
                Thread.Sleep(5000);
                if (messages - curMessageCount > 0)
                    _logger.LogInformation($"counter: events processed per second {(messages - curMessageCount)/5.00}");
                else
                    _logger.LogInformation($"counter: events processed per second 0");
            }
        }
    }
}
