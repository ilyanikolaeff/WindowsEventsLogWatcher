using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEvtLogWatcher
{
    public static class Extensions
    {
        public static void ClearQueue<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.Count() > 0)
            {
                var tmpResult = queue.TryDequeue(out T tempElement);
            }
        }
    }
}
