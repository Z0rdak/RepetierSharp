using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepetierSharp.Util
{
    internal static class CyclicCallHelper
    {

        public static Timer CreateCyclicCall(Action call, uint interval = 5000, int Timeout = Timeout.Infinite)
        {
            Timer timer = null;
            async void timerCallback(object state)
            {
                try
                {
                    await Task.Run(call);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                finally
                {
                    timer.Change(interval, Timeout);
                }
            }
            timer = new Timer(timerCallback, null, interval, Timeout);
            return timer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduledCalls"></param>
        /// <param name="mappingName"></param>
        public static void SuspendCyclicCall(ref Dictionary<string, Timer> scheduledCalls, string mappingName)
        {
            if (scheduledCalls.ContainsKey(mappingName))
            {
                scheduledCalls[mappingName].Dispose();
                scheduledCalls[mappingName] = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduledCalls"></param>
        /// <param name="mappingName"></param>
        /// <param name="interval"></param>
        public static void ChangeInterval(ref Dictionary<string, Timer> scheduledCalls, string mappingName, uint interval)
        {
            if (scheduledCalls.ContainsKey(mappingName))
            {
                scheduledCalls[mappingName].Change(interval, Timeout.Infinite);
            }
        }

    }
}
