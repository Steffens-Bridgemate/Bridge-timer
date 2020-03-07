using BridgeTimer.DomainTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            RoundEnded,
            EventEnded

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

        private IntervalCountingTimer timer;
        private TimeSpan totalTime;
        private TimeSpan customChangeTime;
        private bool warningGiven;
        private bool endOfRoundGiven;
        private bool startOfRoundGiven;
        public CountDownTimer(int playTimeHours,
                              int playTimeMinutes,
                              int warningMoment,
                              List<(int roundNumber,int changeTime)> changeTimes,
                              int defaultChangeTime,
                              int numberOfRounds)
        {
            ChangeTimes = new List<(int roundNumber, int changeTime)>();
            Init(playTimeHours, playTimeMinutes, warningMoment,changeTimes, defaultChangeTime,numberOfRounds);
#if(DEBUG)
            timer = new IntervalCountingTimer(100);
#else
            timer = new IntervalCountingTimer(1000);
#endif
            timer.Elapsed += OnTimerTick;
            InitializeTimeSpans(ThresholdReached.NotSet);
            RunningState = State.Stopped;
        }

        private void Init(int playTimeHours,
                          int playTimeMinutes,
                          int warningMoment,
                          List<(int roundNumber,int changeTime)> changeTimes,
                          int defaultChangeTime,
                          int numberOfRounds)
        {
            this.TotalPlayTime = playTimeHours * 60 + playTimeMinutes;
            this.WarningMoment = warningMoment;
            this.ChangeTimes = changeTimes.ToList();
            this.DefaultChangeTime = defaultChangeTime;
            this.NumberOfRounds = numberOfRounds;
            this.CurrentRound = NumberOfRounds == 0 ? 0 : 1;
        }

        public void Reinit()
        {
            InitializeTimeSpans();
        }
        public void Reinit(int playTimeHours,
                           int playTimeMinutes,
                           int warningMoment,
                           List<(int roundNumber,int changeTime)> changeTimes,
                           int defaultChangeTime,
                           int numberOfRounds)
        {

            Init(playTimeHours, playTimeMinutes, warningMoment,changeTimes, defaultChangeTime, numberOfRounds);
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
            else if (customChangeTime.TotalMilliseconds > 0)
            {
                customChangeTime= customChangeTime.Add(duration);
                timeToPublish = customChangeTime;
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
                if (duration > customChangeTime) duration = customChangeTime;
                customChangeTime = customChangeTime.Subtract(duration);
                timeToPublish = customChangeTime;
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
            this.customChangeTime = TimeSpan.FromMinutes(ChangeTimes.Where(ct=>ct.roundNumber==CurrentRound)
                                                                    .Select(ct=>ct.changeTime)
                                                                    .DefaultIfEmpty( this.DefaultChangeTime)
                                                                    .Single());

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
        public int DefaultChangeTime { get; private set; }
        public List<(int roundNumber,int changeTime)> ChangeTimes { get; private set; }
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

        /// <summary>
        /// For debugging.
        /// </summary>
        internal void JustStopTimer()
        {
            timer.Stop();
        }

        public void Stop()
        {
            timer.Stop();
            RunningState = State.Stopped;
            InitializeTimeSpans();
            CurrentRound = NumberOfRounds == 0 ? 0 : 1;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            timer.Pause();
            var interval = timer.GetInterval();
            Debug.Print(interval.ToString());
            totalTime= totalTime.SubstractMilliSeconds(interval);
            TimeSpan timeToPublish;
            var hoursInUse = totalTime.AddMinute().Hours >0;
            var eventEnded = CurrentRound > NumberOfRounds;
          
            ThresholdReached threshold = ThresholdReached.NotSet;
            if (totalTime.AddMinute().Minutes <= 0 && !hoursInUse)
            {
                customChangeTime = customChangeTime.SubstractMilliSeconds(interval);
                if (customChangeTime.AddMinute().Minutes <= 0)
                {
                    InitializeTimeSpans(eventEnded? ThresholdReached.EventEnded:ThresholdReached.RoundStarted);
                    if (!eventEnded)
                        timer.Start();
                    else
                    {
                        threshold = ThresholdReached.EventEnded;
                        RunningState = State.Stopped;
                        timer.Stop();
                        CurrentRound = NumberOfRounds == 0 ? 0 : 1;
                    }
                       

                    timeToPublish = totalTime;
                }
                else
                    timeToPublish = customChangeTime;
            }
            else
            {
                timeToPublish = totalTime;
            }
                
            if (totalTime.AddMinute().Minutes <= WarningMoment && !warningGiven && !hoursInUse & !eventEnded)
            {
                threshold = ThresholdReached.EndOfRoundWarning;
                warningGiven = true;
            }
            else if(totalTime.AddMinute().Minutes<=0 && ! endOfRoundGiven && !hoursInUse && ! eventEnded)
            {
                threshold = ThresholdReached.RoundEnded;
                endOfRoundGiven = true;
                if (CurrentRound > 0)
                    CurrentRound += 1;
            }
            else if(totalTime.Minutes<=this.TotalPlayTime && ! startOfRoundGiven && !eventEnded)
            {
                threshold = ThresholdReached.RoundStarted;
                startOfRoundGiven = true;
            }

            CurrentTime?.Invoke(this, new CurrentTimeArgs(timeToPublish.Hours,
                                                          timeToPublish.Minutes,
                                                          timeToPublish.Seconds,
                                                          CurrentRound,
                                                          threshold));

            timer.Resume();

        }
    }
}
