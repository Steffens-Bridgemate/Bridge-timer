﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BridgeTimer
{
    public class CountDownTimer
    {
        public enum ThresholdReached
        {
            NotSet = 0,
            RoundStarted,
            EndOfRoundWarning,
            RoundEnded
        }

        public enum State
        {
            NotSet=0,
            Started,
            Paused,
            Stopped
        }
        public class CurrentTimeArgs : EventArgs
        {
            public CurrentTimeArgs(int minutes, int seconds,
                ThresholdReached threshold = ThresholdReached.NotSet)
            {
                this.Minutes = minutes;
                this.Seconds = seconds;
                this.Threshold = threshold;
            }
            public int Minutes { get; }
            public int Seconds { get; }

            public ThresholdReached Threshold { get; }
        }

        public event EventHandler<CurrentTimeArgs> CurrentTime;

        private Timer timer;
        private TimeSpan totalTime;
        private TimeSpan changeTime;
        private bool warningGiven;
        private bool endOfRoundGiven;
        private bool startOfRoundGiven;
        public CountDownTimer(int totalPlayTime, int warningMoment, int changeTime)
        {
            this.TotalPlayTime = totalPlayTime;          
            this.WarningMoment = warningMoment;
            this.ChangeTime = changeTime;
            this.RunningState = State.Stopped;

            timer = new Timer(1000);
            timer.Elapsed += OnTimerTick;
            InitializeTimeSpans();
        }

        public void Reinit()
        {
            InitializeTimeSpans();
        }
        public void Reinit(int totalPlayTime, int warningMoment, int changeTime)
        {
            this.TotalPlayTime = totalPlayTime;
            this.WarningMoment = warningMoment;
            this.ChangeTime = changeTime;
            Reinit();
            
        }

        public void IncreasePlaytime(TimeSpan duration)
        {
            TimeSpan timeToPublish;
            var threshold = ThresholdReached.NotSet;

            if (totalTime.TotalMilliseconds > 0)
            {
                totalTime= totalTime.Add(duration);
                timeToPublish = totalTime;
                if (totalTime.TotalMinutes > WarningMoment && totalTime.Subtract(duration).TotalMinutes <= WarningMoment)
                {
                    threshold = ThresholdReached.RoundStarted;
                    warningGiven = false;
                }
                   
                else
                    threshold = ThresholdReached.NotSet;
            }
            else if (changeTime.TotalMilliseconds > 0)
            {
                changeTime= changeTime.Add(duration);
                timeToPublish = changeTime;
            }
            else
                return;

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Minutes, timeToPublish.Seconds, threshold));
        }

        public void DecreasePlaytime(TimeSpan duration)
        {
            TimeSpan timeToPublish;
            var threshold = ThresholdReached.NotSet;

            if (totalTime.TotalMilliseconds>0)
            {
                if(duration>totalTime) duration=totalTime;

                totalTime = totalTime.Subtract(duration);
                timeToPublish = totalTime;
                if (totalTime.TotalMinutes <= WarningMoment && totalTime.Add(duration).TotalMinutes > WarningMoment)
                {
                    threshold = ThresholdReached.EndOfRoundWarning;
                }
                else
                    threshold = ThresholdReached.NotSet;
            }
            else 
            {
                if (duration > changeTime) duration = changeTime;
                changeTime = changeTime.Subtract(duration);
                timeToPublish = changeTime;
            }
            

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Minutes, timeToPublish.Seconds, threshold));
        }

        private void InitializeTimeSpans()
        {
            timer.Stop();
            startOfRoundGiven = false;
            endOfRoundGiven = false;
            warningGiven = false;
            totalTime = TimeSpan.FromMinutes(this.TotalPlayTime);
            this.changeTime = TimeSpan.FromMinutes(this.ChangeTime);
            CurrentTime?.Invoke(this, new CurrentTimeArgs(totalTime.Minutes, totalTime.Seconds,ThresholdReached.RoundStarted));
        }

        /// <summary>
        /// Total time in minutes
        /// </summary>
        public int TotalPlayTime { get; private set; }

        /// <summary>
        /// The number of minutes before the end of the total time when a warning should be given.
        /// </summary>
        public int WarningMoment { get; private set; }
        public int ChangeTime { get; private set; }

        public State RunningState { get; private set; }

        public void ToggleRunningState()
        {
            switch(RunningState)
            {
                case State.Stopped:
                    Start();
                    break;
                case State.Started:
                    Pause();
                    break;
                case State.Paused:
                    Resume();
                    break;
                default:
                    throw new InvalidOperationException($"Invalid state: '{RunningState}'");
            }
        }

        public void Pause()
        {
            timer.Stop();
            RunningState = State.Paused;
        }

        public void Resume()
        {
            timer.Start();
            RunningState = State.Started;
        }

        public void Start()
        {
            InitializeTimeSpans();
            timer.Start();
            RunningState = State.Started;

        }

        public void Stop()
        {
            timer.Stop();
            RunningState = State.Stopped;
            InitializeTimeSpans();
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            totalTime= totalTime.SubtractSecond();
            TimeSpan timeToPublish;

            if (totalTime.AddMinute().Minutes <= 0)
            {
                changeTime = changeTime.SubtractSecond();
                if (changeTime.AddMinute().Minutes <= 0)
                {
                    InitializeTimeSpans();
                    timer.Start();
                    timeToPublish = totalTime;
                }
                else
                    timeToPublish = changeTime;
            }
            else
            {
                timeToPublish = totalTime;
            }
                

            ThresholdReached threshold = ThresholdReached.NotSet;
            if (totalTime.AddMinute().Minutes <= WarningMoment && !warningGiven)
            {
                threshold = ThresholdReached.EndOfRoundWarning;
                warningGiven = true;
            }
            else if(totalTime.AddMinute().Minutes<=0 && ! endOfRoundGiven)
            {
                threshold = ThresholdReached.RoundEnded;
                endOfRoundGiven = true;
            }
            else if(totalTime.Minutes<=this.TotalPlayTime && ! startOfRoundGiven)
            {
                threshold = ThresholdReached.RoundStarted;
                startOfRoundGiven = true;
            }

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Minutes,timeToPublish.Seconds, threshold));

        }
    }
}
