using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BridgeTimer.DomainTypes
{
    /// <summary>
    /// This class will track the absolute time it is running apart from raising an event after the specified interval.
    /// In this way it can be used to drive clocks. The absolute running time is measured using the Stopwatch class. 
    /// </summary>
    class IntervalCountingTimer: System.Timers.Timer
    {
        private long lastElapsedMilliseconds;
        private long currentElapsedMilliseconds;
        private Stopwatch stopwatch;

        public IntervalCountingTimer():base()
        {
            stopwatch = new Stopwatch();
            
        }

        public IntervalCountingTimer(double interval):base(interval)
        {
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Resets the interval for raising the Elapsed event and resest the tracking of the absolute time the timer is running.
        /// </summary>
        public new void Start()
        {
            stopwatch.Restart();
            lastElapsedMilliseconds = 0;
            base.Start();
        }

        /// <summary>
        /// Halts raising more Elapsed events, but does not stop tracking the absolute time the timer is running.
        /// </summary>
        public void Pause()
        {
            base.Stop();
        }

        /// <summary>
        /// Resumes raising the Elapsed event after the specified interval.
        /// </summary>
        public void Resume()
        {
            base.Start();
        }

        /// <summary>
        /// Stops the raising of the Elapsed event and halts tracking of the absolute time that the timer is running.
        /// </summary>
        public new void Stop()
        {
            base.Stop();
            stopwatch.Stop();
            currentElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        }

        public long GetInterval()
        {
            currentElapsedMilliseconds =stopwatch.ElapsedMilliseconds;
            var interval = currentElapsedMilliseconds - lastElapsedMilliseconds;
            lastElapsedMilliseconds = currentElapsedMilliseconds;
            return interval;
        }
      
    }
}
