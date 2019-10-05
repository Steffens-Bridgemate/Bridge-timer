using BridgeTimer.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;

namespace BridgeTimer
{
    public class Viewmodel : NotifyingObject
    {
        public class SettingsRequestedEventArgs : EventArgs
        { }

        public interface IThresholdNotification
        {
            void Notify(CountDownTimer.ThresholdReached threshold);
            bool IsMuted { get; set; }
        }

        public event EventHandler<SettingsRequestedEventArgs>? SettingsRequested;
        public event EventHandler? CloseRequested;

        private CountDownTimer timer;
        private Timer controlPanelTimer;

        private List<IThresholdNotification> notificators;
        private AppSettings _settings;

        public Viewmodel(IEnumerable<IThresholdNotification> thresholdNotificators, AppSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _isStopped = true;
            notificators = new List<IThresholdNotification>();

            timer = new CountDownTimer(_settings.TotalTime, _settings.WarningTime, _settings.ChangeTime);
            timer.CurrentTime += OnCurrentTime;
            timer.Reinit();

            controlPanelTimer = new Timer(7500);
            controlPanelTimer.Elapsed += ControlPanelTimer_Elapsed;
            controlPanelTimer.AutoReset=false;

            ToggleSoundCommand = new RelayCommand<object>(ToggleSound);
            StartOrPauseCommand = new RelayCommand<object>(ToggleTimer);
            StopOrCloseCommand = new RelayCommand<object>(StopOrClose,
                                    (x) =>timer.RunningState!= CountDownTimer.State.Started);
            IncreasePlaytimeCommand = new RelayCommand<string>(IncreasePlaytime,s=>timer.RunningState!= CountDownTimer.State.Stopped);
            DecreasePlaytimeCommand = new RelayCommand<string>(DecreasePlaytime, s => timer.RunningState != CountDownTimer.State.Stopped);
            SettingsCommand = new RelayCommand<object>(EditSettings, obj => timer.RunningState == CountDownTimer.State.Stopped);
            ConfirmSettingsCommand = new RelayCommand<object>(HandleNewSettings);
            RestoreTimingDefaultsCommand = new RelayCommand<object>(RestoreTimingDefaults);
            RestoreColorDefaultsCommand = new RelayCommand<object>(RestoreColorDefaults);

            TimeLeft = "0:00";
            notificators = thresholdNotificators.ToList();
            Minutes = new ObservableCollection<int>(Enumerable.Range(1, 99));
            SelectedMinutes = settings.TotalTime;
            WarningMinutes = new ObservableCollection<int>(Enumerable.Range(1, 30));
            SelectedWarningMinutes = settings.WarningTime;
            ChangeMinutes = new ObservableCollection<int>(Enumerable.Range(1, 30));
            SelectedChangeMinutes = settings.ChangeTime;
            HideMessage = true;
          
        }

        private void ControlPanelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DetermineControlPanelState();
        }

        private void OnCurrentTime(object? sender, CountDownTimer.CurrentTimeArgs e)
        {
            TimeLeft = $"{e.Minutes}:{e.Seconds}";
            MinutesLeft = $"{string.Format("{0:00}", e.Minutes)}";
            SecondsLeft = $"{string.Format("{0:00}", e.Seconds)}";
            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));

            if (e.Threshold != CountDownTimer.ThresholdReached.NotSet)
            {
                var doNotNotify = CurrentStage == CountDownTimer.ThresholdReached.EndOfRoundWarning &&
                                  e.Threshold == CountDownTimer.ThresholdReached.RoundStarted;
                CurrentStage = e.Threshold;
                HideMessage = (e.Threshold != CountDownTimer.ThresholdReached.RoundEnded);

                if (timer.RunningState == CountDownTimer.State.Stopped || doNotNotify   ) return;

                foreach (var notificator in notificators)
                {
                    notificator.Notify(e.Threshold);
                }
            }
        }

        #region Public interface
        public void DetermineControlPanelState()
        {
            if (timer.RunningState == CountDownTimer.State.Stopped || timer.RunningState==CountDownTimer.State.Paused)
                HideControlPanel = false;
            else
                HideControlPanel = true;
        }

        public void ShowControlPanel()
        {
            HideControlPanel=false;
            controlPanelTimer.Start();
        }

        #endregion

        #region Commands
    
        public RelayCommand<string> IncreasePlaytimeCommand { get; }

        private void  IncreasePlaytime(string seconds)
        {
            if(string.IsNullOrWhiteSpace(seconds)) return;

            timer.IncreasePlaytime(TimeSpan.FromSeconds(int.Parse( seconds)));
            ShowControlPanel();
        }

        public RelayCommand<string> DecreasePlaytimeCommand { get; }

        private void DecreasePlaytime(string seconds)
        {
            if (string.IsNullOrWhiteSpace(seconds)) return;

            timer.DecreasePlaytime(TimeSpan.FromSeconds(int.Parse( seconds)));
            ShowControlPanel();
        }

        public RelayCommand<object> ToggleSoundCommand { get; }

        private void ToggleSound(object obj)
        {
            IsMuted = !IsMuted;
            ShowControlPanel();
        }

        public RelayCommand<object> StartOrPauseCommand { get; }
        private void ToggleTimer(object obj)
        {
            timer.ToggleRunningState();

            _isStopped = false;

            DetermineControlPanelState();

            OnPropertyChanged(nameof(StartOrPauseCaption));
            OnPropertyChanged(nameof(StopOrCloseCaption));
            ShowControlPanel();
        }

        private bool _isStopped;
        public RelayCommand<object> StopOrCloseCommand { get; }

        private void StopOrClose(object obj)
        {
            if (_isStopped)
            {
                CloseRequested?.Invoke(this, new EventArgs());
                return;
            }

            timer.Stop();
            _isStopped=true;
            HideControlPanel = false;
            OnPropertyChanged(nameof(StartOrPauseCaption));
            OnPropertyChanged(nameof(StopOrCloseCaption));
            ShowControlPanel();
        }

        public RelayCommand<object> SettingsCommand { get; }
        private void EditSettings(object obj)
        {
            SettingsRequested?.Invoke(this, new SettingsRequestedEventArgs());
            ShowControlPanel();
        }
        public RelayCommand<object> ConfirmSettingsCommand { get; }

        private void HandleNewSettings(object parameter)
        {
            timer.Reinit(SelectedMinutes, SelectedWarningMinutes, SelectedChangeMinutes);
            OnPropertyChanged(nameof(CurrentStage));
        }

        public RelayCommand<object> RestoreTimingDefaultsCommand { get; }

        private void RestoreTimingDefaults(object obj)
        {
            _settings.RestoreTimingDefaults();
            _settings.Save();
            OnPropertyChanged(nameof(SelectedMinutes));
            OnPropertyChanged(nameof(SelectedWarningMinutes));
            OnPropertyChanged(nameof(SelectedChangeMinutes));
        }

        public RelayCommand<object> RestoreColorDefaultsCommand { get; }

        private void RestoreColorDefaults(object obj)
        {
            _settings.RestoreColorDefaults();
            _settings.Save();
            OnPropertyChanged(nameof(PlayingTimeBackground));
            OnPropertyChanged(nameof(PlayingTimeForeground));
            OnPropertyChanged(nameof(WarningTimeBackground));
            OnPropertyChanged(nameof(WarningTimeForeground));
            OnPropertyChanged(nameof(ChangeTimeBackground));
            OnPropertyChanged(nameof(ChangeTimeForeground));
            OnPropertyChanged(nameof(CurrentStage));
        }
        #endregion

        #region Settings

        public Color PlayingTimeForeground
        {
            get 
            {
                return _settings.PlayingTimeForeground;
            }
            set
            {
                _settings.PlayingTimeForeground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }

        public Color ChangeTimeForeground
        {
            get
            {
                return _settings.ChangeTimeForeground;
            }
            set
            {
                _settings.ChangeTimeForeground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }

        public Color WarningTimeForeground
        {
            get
            {
                return _settings.WarningTimeForeground;
            }
            set
            {
                _settings.WarningTimeForeground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }
        public Color PlayingTimeBackground
        {
            get
            {
                return _settings.PlayingTimeBackground;
            }
            set
            {
                _settings.PlayingTimeBackground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }

        public Color ChangeTimeBackground
        {
            get
            {
                return _settings.ChangeTimeBackground;
            }
            set
            {
                _settings.ChangeTimeBackground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }

        public Color WarningTimeBackground
        {
            get
            {
                return _settings.WarningTimeBackground;
            }
            set
            {
                _settings.WarningTimeBackground = value;
                _settings.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentStage));
            }
        }



        public ObservableCollection<int> Minutes { get; set; }
      
        public int SelectedMinutes
        {
            get => _settings.TotalTime;
            set
            {
                if (value == _settings.TotalTime) return;
                _settings.TotalTime = value;
                _settings.Save();
                OnPropertyChanged();
            }
        }
        public ObservableCollection<int> WarningMinutes { get; set; }
        public int SelectedWarningMinutes
        {
            get => _settings.WarningTime;
            set
            {
                if (value == _settings.WarningTime) return;
                _settings.WarningTime = value;
                _settings.Save();
                OnPropertyChanged();
            }
        }
        public ObservableCollection<int> ChangeMinutes { get; set; }
        public int SelectedChangeMinutes
        {
            get => _settings.ChangeTime;
            set
            {
                if (value == _settings.ChangeTime) return;
                _settings.ChangeTime = value;
                _settings.Save();
                OnPropertyChanged();
            }
        }


        #endregion

        private bool _hideControlPanel;

        public bool HideControlPanel
        {
            get { return _hideControlPanel; }
            set 
            {
                if (value == _hideControlPanel) return;

                _hideControlPanel = value;
                OnPropertyChanged();
            }
        }

        private bool _isMuted;

        public bool IsMuted
        {
            get { return _isMuted; }
            set
            {
                if (value == _isMuted) return;
                notificators.ForEach(n => n.IsMuted = value);
               
                _isMuted = value;
                OnPropertyChanged();
            }
        }


        private bool _hideMessage;

        public bool HideMessage
        {
            get { return _hideMessage; }
            set
            {
                if (value == _hideMessage) return;

                _hideMessage = value;
                OnPropertyChanged();
            }
        }

        public string StartOrPauseCaption
        {
            get => timer.RunningState == CountDownTimer.State.Started ? Properties.Resources.Caption_Pause : Properties.Resources.Caption_Start;
        }

        public string StopOrCloseCaption
        {
            get => _isStopped ? Properties.Resources.Caption_Close : Properties.Resources.Caption_Stop;
        }

        private string? timeLeft;
        public string? TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = value;
                OnPropertyChanged();
            }
        }

        public string? MinutesLeft { get; set; }
        public string? SecondsLeft { get; set; }

        private CountDownTimer.ThresholdReached _currentStage;
        public CountDownTimer.ThresholdReached CurrentStage
        {
            get => _currentStage;
            set
            {
                if (value == _currentStage) return;
                _currentStage = value;
                OnPropertyChanged();
            }
        }

        
    }
}
