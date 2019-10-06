using System;
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
            public CurrentTimeArgs(int hours, int minutes, int seconds,int currentRound,
                                   ThresholdReached threshold = ThresholdReached.NotSet)
            {
                this.Hours = hours;
                this.Minutes = minutes;
                this.Seconds = seconds;
                this.CurrentRound = currentRound;
                this.Threshold = threshold;
            }

            public int Hours { get; }

            public int Minutes { get; }
            public int Seconds { get; }
            public int CurrentRound { get; }

            public ThresholdReached Threshold { get; }
        }

        public event EventHandler<CurrentTimeArgs>? CurrentTime;

        private Timer timer;
        private TimeSpan totalTime;
        private TimeSpan changeTime;
        private bool warningGiven;
        private bool endOfRoundGiven;
        private bool startOfRoundGiven;
        public CountDownTimer(int playTimeHours, int playTimeMinutes, int warningMoment, int changeTime, int numberOfRounds)
        {
            Init(playTimeHours, playTimeMinutes, warningMoment, changeTime,numberOfRounds);

            timer = new Timer(1000);
            timer.Elapsed += OnTimerTick;
            InitializeTimeSpans(ThresholdReached.NotSet);
            RunningState = State.Stopped;
        }

        private void Init(int playTimeHours, int playTimeMinutes, int warningMoment, int changeTime, int numberOfRounds)
        {
            this.TotalPlayTime = playTimeHours * 60 + playTimeMinutes;
            this.WarningMoment = warningMoment;
            this.ChangeTime = changeTime;
            this.NumberOfRounds = numberOfRounds;
            this.CurrentRound = NumberOfRounds == 0 ? 0 : 1;
        }

        public void Reinit()
        {
            InitializeTimeSpans();
        }
        public void Reinit(int playTimeHours, int playTimeMinutes, int warningMoment, int changeTime,int numberOfRounds)
        {

            Init(playTimeHours, playTimeMinutes, warningMoment, changeTime, numberOfRounds);
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

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Hours, 
                                                          timeToPublish.Minutes, 
                                                          timeToPublish.Seconds,
                                                          CurrentRound,
                                                          threshold));
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
            

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Hours,
                                                          timeToPublish.Minutes,
                                                          timeToPublish.Seconds,
                                                          CurrentRound,
                                                          threshold));
        }

        private void InitializeTimeSpans(ThresholdReached threshold=ThresholdReached.RoundStarted)
        {
            timer.Stop();
            startOfRoundGiven = false;
            endOfRoundGiven = false;
            warningGiven = false;
            totalTime = TimeSpan.FromMinutes(this.TotalPlayTime);
            this.changeTime = TimeSpan.FromMinutes(this.ChangeTime);
            CurrentTime?.Invoke(this, new CurrentTimeArgs(totalTime.Hours, 
                                                          totalTime.Minutes, 
                                                          totalTime.Seconds,
                                                          CurrentRound,
                                                          threshold));
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
        public int NumberOfRounds { get; private set; }
        public int CurrentRound { get; private set; }

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
            var hoursInUse = totalTime.AddMinute().Hours >0;

            if (totalTime.AddMinute().Minutes <= 0 && !hoursInUse)
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
            if (totalTime.AddMinute().Minutes <= WarningMoment && !warningGiven && !hoursInUse)
            {
                threshold = ThresholdReached.EndOfRoundWarning;
                warningGiven = true;
            }
            else if(totalTime.AddMinute().Minutes<=0 && ! endOfRoundGiven && !hoursInUse)
            {
                threshold = ThresholdReached.RoundEnded;
                endOfRoundGiven = true;
                if (CurrentRound > 0)
                    CurrentRound += 1;
            }
            else if(totalTime.Minutes<=this.TotalPlayTime && ! startOfRoundGiven)
            {
                threshold = ThresholdReached.RoundStarted;
                startOfRoundGiven = true;
            }

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Hours,
                                                          timeToPublish.Minutes,
                                                          timeToPublish.Seconds,
                                                          CurrentRound,
                                                          threshold));

        }
    }
}
