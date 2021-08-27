using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Linq;

namespace BridgeTimer.Settings
{
    public class AppSettingsContainer
    {

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public AppSettingsContainer(AppSettings settings)
        {
            AppSettings = settings;
        }

        public AppSettings AppSettings { get; set; }
    }

    public class AppSettings
    {
        #region Static methods

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static AppSettings Default()
        {
            return new AppSettings()
            {
                PlayTimeHours = DefaultTimings.hours,
                PlayTimeMinutes = DefaultTimings.minutes,
                WarningTime = DefaultTimings.warn,
                ChangeTime = DefaultTimings.change,
                NumberOfRounds = DefaultNumberOfRounds,
                PlayingTimeBackground = DefaultBackgrounds.total,
                WarningTimeBackground = DefaultBackgrounds.warn,
                ChangeTimeBackground = DefaultBackgrounds.change,
                PlayingTimeForeground = DefaultForegrounds.total,
                WarningTimeForeground = DefaultForegrounds.warn,
                ChangeTimeForeground = DefaultForegrounds.change
            };
        }

        public static (int hours, int minutes, int warn, int change) DefaultTimings => (0, 30, 5, 2);

        public static int DefaultNumberOfRounds = 6;
        private int warningTime;
        private int playTimeMinutes;
        private int changeTime;
        private int playTimeHours;

        public static (Color total, Color warn, Color change) DefaultBackgrounds => (Colors.DarkGreen,
                                                                                    (Color)ColorConverter.ConvertFromString("#EB9605"),
                                                                                     Colors.DarkRed);
        public static (Color total, Color warn, Color change) DefaultForegrounds => (Colors.White,
                                                                                     Colors.White,
                                                                                     Colors.White);
        #endregion

        public AppSettings()
        {
            CustomBreaks = new List<CustomBreak>();
        }

        #region Settings

        public int PlayTimeHours 
        { 
            get => playTimeHours;
            set
            {
                playTimeHours = value;
                PlayTimeMinutes = playTimeMinutes;
                WarningTime = warningTime;
            }
        }

        public int PlayTimeMinutes
        {
            get => playTimeMinutes;
            set
            {
                if (PlayTimeHours <= 0)
                    playTimeMinutes = Math.Max(2, value);
                else
                    playTimeMinutes = value;
                WarningTime = warningTime;
            }
        }


        public int WarningTime
        {
            get => warningTime;
            set
            {
                warningTime = Math.Min(PlayTimeHours * 60 + PlayTimeMinutes - 1, value);
            }
        }

        public int ChangeTime
        {
            get => changeTime;
            set
            {
                if (value == changeTime) return;
                var initializing = changeTime == 0;
                changeTime = Math.Max(1, value);
                if(!initializing) UpdateCustomBreaks();
                
            }
        }

        private int numberOfRounds;
        public int NumberOfRounds
        {
            get => numberOfRounds;
            set
            {
                if (value == numberOfRounds) return;
                var initializing = numberOfRounds == 0;
                numberOfRounds = value;
                if(!initializing) UpdateCustomBreaks();
            }
        }

        public Color WarningTimeForeground { get; set; }
        public Color PlayingTimeForeground { get; set; }
        public Color ChangeTimeForeground { get; set; }
        public Color WarningTimeBackground { get; set; }
        public Color PlayingTimeBackground { get; set; }
        public Color ChangeTimeBackground { get; set; }
        public bool IsMuted { get; set; }
        public string? CustomChangeMessageForRound { get; set; }
        public string? CustomChangeMessage { get; set; }
        public string? CustomEndOfEventMessage { get; set; }
        public List<CustomBreak> CustomBreaks { get; set; }
        public bool StartMaximized { get; set; }

        #endregion

        #region public methods

        public void RestoreTimingDefaults()
        {
            PlayTimeHours = DefaultTimings.hours;
            PlayTimeMinutes = DefaultTimings.minutes;
            WarningTime = DefaultTimings.warn;
            ChangeTime = DefaultTimings.change;
            NumberOfRounds = DefaultNumberOfRounds;
        }

        public void RestoreColorDefaults()
        {
            PlayingTimeBackground = DefaultBackgrounds.total;
            PlayingTimeForeground = DefaultForegrounds.total;
            WarningTimeBackground = DefaultBackgrounds.warn;
            WarningTimeForeground = DefaultForegrounds.warn;
            ChangeTimeBackground = DefaultBackgrounds.change;
            ChangeTimeForeground = DefaultForegrounds.change;
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(App.GetFullAppDataPath(App.SettingsFilename), 
                                  JsonConvert.SerializeObject(new AppSettingsContainer(this)));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
            
        }

        #endregion

        #region Private Methods
        private void UpdateCustomBreaks()
        {
            CustomBreaks = new List<CustomBreak>();
            for (var i = 1; i <= numberOfRounds-1; i++)
            {
                CustomBreaks.Add(new CustomBreak() { RoundNumber = i, 
                                                     BreakTime = changeTime, 
                                                     Description = string.IsNullOrEmpty( CustomChangeMessage) ?
                                                                        string.Format( Properties.Resources.Message_TakeSeatsForRound,i+1):
                                                                        CustomChangeMessage});
            }
            CustomBreaks.Add(new CustomBreak()
            {
                RoundNumber = numberOfRounds,
                BreakTime = changeTime,
                Description = Properties.Resources.Message_EventEnded
            });
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (!CustomBreaks.Any())
            {
                UpdateCustomBreaks();
                Save(); 
            }
        }
        #endregion

        public class CustomBreak
        {
            public CustomBreak()
            {
                Description = string.Empty;
            }

            public int RoundNumber { get; set; }
            public int BreakTime { get; set; }

            public string Description { get; set; }
        }

    }
}
