using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Windows.Media;

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
            ExtraBreaks = new List<ExtraBreak>();
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
                changeTime = Math.Max(1, value);
                UpdateExtraBreaks();
                
            }
        }

        private int numberOfRounds;
        public int NumberOfRounds
        {
            get => numberOfRounds;
            set
            {
                numberOfRounds = value;
                UpdateExtraBreaks();
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
        public List<ExtraBreak> ExtraBreaks { get; set; }

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
        private void UpdateExtraBreaks()
        {
            ExtraBreaks = new List<ExtraBreak>();
            for (var i = 1; i <= NumberOfRounds; i++)
            {
                ExtraBreaks.Add(new ExtraBreak() { RoundNumber = i, BreakTime = changeTime, Description = "" });
            }
        }
        #endregion

        public class ExtraBreak
        {
            public ExtraBreak()
            {
                Description = string.Empty;
            }

            public int RoundNumber { get; set; }
            public int BreakTime { get; set; }

            public string Description { get; set; }
        }

    }
}
