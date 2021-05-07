using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepetierMqtt.Util
{
    public class PeriodicTask
    {
        private Task UnderlyingTask { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private Func<Task> Action { get; set; }

        public uint Interval { get; private set; }

        public PeriodicTask(Action task, uint interval = 5000)
        {
            Interval = Math.Max(interval, 1000);
            Action = new Func<Task>(async () => await Task.Run(task));
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void Run()
        {
            Action = new Func<Task>(async () => await Task.Run(Action));
            UnderlyingTask = Run(Action, CancellationTokenSource.Token, Interval);
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
            UnderlyingTask.Wait();
            if (UnderlyingTask.Status >= TaskStatus.RanToCompletion)
            {
                UnderlyingTask.Dispose();
            }
        }

        public static void ChangeInterval(PeriodicTask periodicTask, uint interval)
        {
            if (interval >= 1000)
            {
                periodicTask.Interval = interval;
            }

        }

        /// <summary>
        /// Credit to: https://stackoverflow.com/questions/4890915/is-there-a-task-based-replacement-for-system-threading-timer/23814733#23814733
        /// </summary>
        public static async Task Run(Func<Task> action, CancellationToken cancellationToken, uint miliseconds = 5000)
        {
            var period = TimeSpan.FromMilliseconds(miliseconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    await action();
                }
            }
        }

        /// <summary>
        /// Credit to: https://stackoverflow.com/questions/4890915/is-there-a-task-based-replacement-for-system-threading-timer/23814733#23814733
        /// </summary>
        public static Task Run(Func<Task> action, uint miliseconds = 5000)
        {
            return Run(action, CancellationToken.None, miliseconds);
        }


        /// <summary>
        /// Credit to: https://stackoverflow.com/questions/4890915/is-there-a-task-based-replacement-for-system-threading-timer/23814733#23814733
        /// </summary>
        public static async Task Run(Action action, CancellationToken cancellationToken, uint miliseconds = 5000)
        {
            var period = TimeSpan.FromMilliseconds(miliseconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Credit to: https://stackoverflow.com/questions/4890915/is-there-a-task-based-replacement-for-system-threading-timer/23814733#23814733
        /// </summary>
        public static Task Run(Action action, uint miliseconds = 5000)
        {
            return Run(action, CancellationToken.None, miliseconds);
        }
    }
}
