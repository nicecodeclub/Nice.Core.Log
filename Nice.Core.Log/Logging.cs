using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nice.Core.Log
{
    public abstract class Logging
    {
        private bool isActive = false;
        private readonly ManualResetEventSlim mre = null;
        private readonly Queue<ILog> logs = null;
        public Logging()
        {
            isActive = true;
            mre = new ManualResetEventSlim(false);
            logs = new Queue<ILog>();
            Task.Run(() => Work());
        }

        private void Work()
        {
            while (isActive)
            {
                if (logs.Count > 0)
                    OnWrite();
                else
                {
                    //等待写入信号
                    mre.Reset();
                    mre.Wait();
                }
            }
        }
        protected void Write(ILog log)
        {
#if DEBUG
            Console.WriteLine(log.ToString());
#endif
            if (!isActive) return;
            lock (logs)
            {
                logs.Enqueue(log);
                mre.Set();
            }
        }
        protected void OnWrite()
        {
            ILog log = null;

            lock (logs)
            {
                if (logs.Count > 0)
                    log = logs.Dequeue();
            }
            if (log != null)
            {
                OnWrite(log);
            }
        }

        public abstract void OnWrite(ILog log);

        public void Close()
        {
            isActive = false;
            lock (logs)
            {
                if (logs != null && logs.Count > 0)
                {
                    foreach (ILog log in logs)
                    {
                        Write(log);
                    }
                }
            }
        }
    }
}
