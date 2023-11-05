using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.BatchTG;

namespace TG.Client.Cache
{
    public class BatchTaskManager
    {
        private static BatchTaskManager server = new BatchTaskManager();

        public static BatchTaskManager Instance { get { return server; } }

        private BatchTaskManager()
        {
        }

        private ConcurrentQueue<BatchCollectTask> queue = new ConcurrentQueue<BatchCollectTask>();

        public void Clear()
        {
            while (queue.Count > 0)
            {
                try
                {
                    BatchCollectTask local = default(BatchCollectTask);
                    queue.TryDequeue(out local);
                }
                catch (Exception e)
                {
                }
            }
        }

        public void Enqueu(BatchCollectTask batchCollectTask)
        {
            queue.Enqueue(batchCollectTask);
        }

        public BatchCollectTask Dequeue()
        {
            BatchCollectTask tem = null;
            queue.TryDequeue(out tem);


            return tem;
        }
    }
}
