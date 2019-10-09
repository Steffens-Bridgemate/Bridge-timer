using BridgeTimer.Extensions;
using BridgeTimer.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
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

        public const int MaxChangeMessageLenght = 45;

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

            timer = new CountDownTimer(_settings.PlayTimeHours,
                                       _settings.PlayTimeMinutes,
                                       _settings.WarningTime,
                                       _settings.ChangeTime,
                                       _settings.NumberOfRounds);
            timer.CurrentTime += OnCurrentTime;
            timer.Reinit();

            controlPanelTimer = new Timer(6000);
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
            RestoreTextDefaultsCommand = new RelayCommand<object>(RestoreTextDefaults);

            TimeLeft = "0:00";

            notificators = thresholdNotificators.ToList();
            notificators.ForEach(n => n.IsMuted = _settings.IsMuted);

            Hours = new ObservableCollection<int>(Enumerable.Range(0, 10));
            SelectedHours = _settings.PlayTimeHours;
            Minutes = new ObservableCollection<int>(Enumerable.Range(0, 60));
            SelectedMinutes = settings.PlayTimeMinutes;
            WarningMinutes = new ObservableCollection<int>(Enumerable.Range(1, 30));
            SelectedWarningMinutes = settings.WarningTime;
            ChangeMinutes = new ObservableCollection<int>(Enumerable.Range(1, 30));
            SelectedChangeMinutes = settings.ChangeTime;
            NumbersOfRounds = new ObservableCollection<int>(Enumerable.Range(0, 21));
            SelectedNumberOfRounds = settings.NumberOfRounds;

            ChangeMessage = "Hello World!";
            HideMessage = true;
          
        }

        private void ControlPanelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DetermineControlPanelState();
        }

        private void OnCurrentTime(object? sender, CountDownTimer.CurrentTimeArgs e)
        {
           
            TimeLeft = $"{e.Minutes}:{e.Seconds}";
            var moreThan60Minutes = e.Hours > 0;

            if(moreThan60Minutes)
            {
                AreSecondsHidden = true;
                var numberOfHours = e.Hours;
                
                string minutes;
                if(e.Seconds > 0)
                {
                    var numberOfMinutes = e.Minutes;
                    if (numberOfMinutes == 59)
                    {
                        numberOfMinutes = 0;
                        numberOfHours += 1;
                    }
                    else
                        numberOfMinutes += 1;
                    minutes = numberOfMinutes.ToString("00");
                }
                else
                    minutes = e.Minutes.ToString("00");

                var hours = numberOfHours.ToString("00");
                FirstDigit =char.MinValue;
                SecondDigit =hours[1];
                ThirdDigit = minutes[0];
                FourthDigit = minutes[1];
            }
            else
            {
                AreSecondsHidden = false;
                var minutes = e.Minutes.ToString("00");
                var seconds = e.Seconds.ToString("00");
                FirstDigit = minutes[0];
                SecondDigit = minutes[1];
                ThirdDigit = seconds[0];
                FourthDigit = seconds[1];
            }

            if (e.Threshold == CountDownTimer.ThresholdReached.EventEnded)
            {
                DetermineControlPanelState();
                CurrentStage = CountDownTimer.ThresholdReached.RoundStarted;
                _isStopped = true;
                OnPropertyChanged(nameof(StartOrPauseCaption));
                OnPropertyChanged(nameof(StopOrCloseCaption));
                RoundDescription = $"{Properties.Resources.Label_Round} {e.CurrentRound}";
                App.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
                return;
            }

            if (e.Threshold != CountDownTimer.ThresholdReached.NotSet)
            {
                var doNotNotify = CurrentStage == CountDownTimer.ThresholdReached.EndOfRoundWarning &&
                                  e.Threshold == CountDownTimer.ThresholdReached.RoundStarted;
                CurrentStage = e.Threshold;
                
                HideMessage = (e.Threshold != CountDownTimer.ThresholdReached.RoundEnded);
                RoundDescription = timer.NumberOfRounds>0  && 
                                   e.CurrentRound<=timer.NumberOfRounds && 
                                   e.Threshold!= CountDownTimer.ThresholdReached.RoundEnded ? 
                                        $"{Properties.Resources.Label_Round} {e.CurrentRound}":
                                        string.Empty;

                if (e.CurrentRound <= 0)
                    ChangeMessage = CustomChangeText;
                else
                {
                    if (timer.CurrentRound > timer.NumberOfRounds)
                        ChangeMessage =CustomTextAfterLastRound;
                    else
                        try
                        {
                            ChangeMessage = string.Format(CustomChangeTextForRound, e.CurrentRound);
                        }
                        catch (Exception)
                        {
                            ChangeMessage = CustomChangeText;
                        }
                      
                }
                    

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
            controlPanelTimer.Stop();
            controlPanelTimer.Start();
        }

        #endregion

        #region Commands

        public RelayCommand<object?> DoNothingCommand
        {
            get
            {
                return new RelayCommand<object?>((x) => x = null);
            }
        }
    
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
            CurrentStage = CountDownTimer.ThresholdReached.NotSet;
            SettingsRequested?.Invoke(this, new SettingsRequestedEventArgs());
            ShowControlPanel();
        }
        public RelayCommand<object> ConfirmSettingsCommand { get; }

        private void HandleNewSettings(object parameter)
        {
            var changer = new ColorChanger();
           
            changer.ChangeLogoColor(ColorChanger.Convert( PlayingTimeForeground), 
                                    ColorChanger.Convert(PlayingTimeBackground),
                                    App.RoundStartedLogoFile);
            changer.ChangeLogoColor(ColorChanger.Convert(WarningTimeForeground),
                                    ColorChanger.Convert(WarningTimeBackground),
                                    App.WarningLogoFile);
            changer.ChangeLogoColor(ColorChanger.Convert(ChangeTimeForeground),
                                    ColorChanger.Convert(ChangeTimeBackground),
                                    App.RoundEndedLogoFile);

            timer.Reinit(SelectedHours, SelectedMinutes, SelectedWarningMinutes, SelectedChangeMinutes, SelectedNumberOfRounds);
            CurrentStage= CountDownTimer.ThresholdReached.RoundStarted;
            OnAllPropertiesChanged();
        }

        public RelayCommand<object> RestoreTimingDefaultsCommand { get; }

        private void RestoreTimingDefaults(object obj)
        {
            _settings.RestoreTimingDefaults();
            _settings.Save();
            OnPropertyChanged(nameof(SelectedHours));
            OnPropertyChanged(nameof(SelectedMinutes));
            OnPropertyChanged(nameof(SelectedWarningMinutes));
            OnPropertyChanged(nameof(SelectedChangeMinutes));
            OnPropertyChanged(nameof(SelectedNumberOfRounds));
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

        public RelayCommand<object> RestoreTextDefaultsCommand { get; }

        private void RestoreTextDefaults(object obj)
        {
            CustomChangeText = string.Empty;
            CustomChangeTextForRound = string.Empty;
            CustomTextAfterLastRound = string.Empty;

            OnPropertyChanged(nameof(CustomChangeText));
            OnPropertyChanged(nameof(CustomChangeTextForRound));
            OnPropertyChanged(nameof(CustomTextAfterLastRound));
        }

        #endregion

        #region Settings

        private string? _logoPath;

        public string? LogoPath
        {
            get
            {
                switch(CurrentStage)
                {
                    case CountDownTimer.ThresholdReached.EndOfRoundWarning:
                        return WarningLogoPath;
                    case CountDownTimer.ThresholdReached.RoundStarted:
                        return RoundStartedLogoPath;
                    case CountDownTimer.ThresholdReached.RoundEnded:
                        return RoundEndedLogoPath;
                    default:
                        return null; ;
                }
            }
          
        }



        public string? RoundStartedLogoPath
        {
            get
            {

                return App.GetFullAppDataPath(App.RoundStartedLogoFile);
                
            }
        }
        public string RoundEndedLogoPath
        {
            get => App.GetFullAppDataPath(App.RoundEndedLogoFile);
        }
        public string WarningLogoPath
        {
            get => App.GetFullAppDataPath(App.WarningLogoFile);
        }

        public string CustomTextAfterLastRound
        {
            get
            {
                if (string.IsNullOrEmpty(_settings.CustomEndOfEventMessage))
                    return Properties.Resources.Message_EventEnded;
                else
                    return _settings.CustomEndOfEventMessage;
            }
            set
            {
                _settings.CustomEndOfEventMessage = value.PadLeftAndRight(MaxChangeMessageLenght);
                _settings.Save();
                OnPropertyChanged();
            }
        }

        public string CustomChangeText
        {
            get
            {
                if (string.IsNullOrEmpty(_settings.CustomChangeMessage))
                    return Properties.Resources.Message_TakeYourSeats;
                else
                    return _settings.CustomChangeMessage;
            }
            set
            {
                _settings.CustomChangeMessage = value.PadLeftAndRight(MaxChangeMessageLenght);
                _settings.Save();
                OnPropertyChanged();
            }
        }

        public string CustomChangeTextForRound
        {
            get 
            {
                if (string.IsNullOrEmpty(_settings.CustomChangeMessageForRound))
                    return Properties.Resources.Message_TakeSeatsForRound;
                else
                    return _settings.CustomChangeMessageForRound;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _settings.CustomChangeMessageForRound= string.Empty;
                    return;
                }
                if (!value.Contains("{0}"))
                    value += " {0}";
                value = Regex.Replace(value, @"{(?!0})|(?<!{0)}", string.Empty);
                _settings.CustomChangeMessageForRound = value.PadLeftAndRight(MaxChangeMessageLenght);
                _settings.Save();
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<int> Hours { get; set; }

        public int SelectedHours
        {
            get => _settings.PlayTimeHours;
            set
            {
                if (value == _settings.PlayTimeHours) return;
                _settings.PlayTimeHours = value;
                _settings.Save();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Minutes { get; set; }
      
        public int SelectedMinutes
        {
            get => _settings.PlayTimeMinutes;
            set
            {
                if (value == _settings.PlayTimeMinutes) return;
                _settings.PlayTimeMinutes = value;
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

        public ObservableCollection<int> NumbersOfRounds { get; set; }

        public int SelectedNumberOfRounds
        {
            get => _settings.NumberOfRounds;
            set
            {
                if (value == _settings.NumberOfRounds) return;
                _settings.NumberOfRounds = value;
                _settings.Save();
                OnPropertyChanged();
            }
        }

        #endregion

        private string? _roundDescription;

        public string? RoundDescription
        {
            get { return _roundDescription; }
            set 
            { 
                _roundDescription = value;
                OnPropertyChanged();
            }
        }

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

        public bool IsMuted
        {
            get { return _settings.IsMuted; }
            set
            {
                if (value == _settings.IsMuted) return;
                notificators.ForEach(n => n.IsMuted = value);
               
                _settings.IsMuted = value;
                _settings.Save();
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

        private char _firstDigit;

        public char FirstDigit
        {
            get { return _firstDigit; }
            set
            {
                _firstDigit = value;
                OnPropertyChanged();
            }
        }

        private char _secondDigit;

        public char SecondDigit
        {
            get { return _secondDigit; }
            set
            {
                _secondDigit = value;
                OnPropertyChanged();
            }
        }

        private char _thirdDigit;

        public char ThirdDigit
        {
            get { return _thirdDigit; }
            set
            {
                _thirdDigit = value;
                OnPropertyChanged();
            }
        }

        private char _fourthDigit;

        public char FourthDigit
        {
            get { return _fourthDigit; }
            set
            {
                _fourthDigit = value;
                OnPropertyChanged();
            }
        }

        private bool _areSecondsHidden;

        public bool AreSecondsHidden
        {
            get { return _areSecondsHidden && timer.RunningState== CountDownTimer.State.Started; }
            set
            {

                _areSecondsHidden = value;
                OnPropertyChanged();
   
            }
        }

        private string? _changeMessage;

        public string? ChangeMessage
        {
            get { return _changeMessage; }
            set
            {
                _changeMessage = value;
                OnPropertyChanged();
            }
        }




        private CountDownTimer.ThresholdReached _currentStage;
        public CountDownTimer.ThresholdReached CurrentStage
        {
            get => _currentStage;
            set
            {
                if (value == _currentStage) return;
                _currentStage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LogoPath));
            }
        }
    }
}
