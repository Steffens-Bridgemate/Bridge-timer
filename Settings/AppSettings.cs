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
        public AppSettingsContainer(AppSettings settings)
        {
            AppSettings = settings;
        }

        public AppSettings AppSettings { get; set; }
    }

    public class AppSettings
    {
        public static AppSettings Default()
        {
            return new AppSettings() { TotalTime = DefaultTimings.total,
                                       WarningTime = DefaultTimings.warn, 
                                       ChangeTime=DefaultTimings.change,
                                       PlayingTimeBackground=DefaultBackgrounds.total,
                                       WarningTimeBackground= DefaultBackgrounds.warn,
                                       ChangeTimeBackground=DefaultBackgrounds.change,
                                       PlayingTimeForeground=DefaultForegrounds.total,
                                       WarningTimeForeground=DefaultForegrounds.warn,
                                       ChangeTimeForeground=DefaultForegrounds.change};
        }

        public static (int total, int warn, int change) DefaultTimings => (30, 5, 2);
        public static (Color total, Color warn, Color change) DefaultBackgrounds => (Colors.DarkGreen,
                                                                                    (Color)ColorConverter.ConvertFromString("#EB9605"),
                                                                                     Colors.DarkRed);
        public static (Color total, Color warn, Color change) DefaultForegrounds => (Colors.White,
                                                                                     Colors.White,
                                                                                     Colors.White);

        public int TotalTime { get; set; }

        public int WarningTime { get; set; }

        public int ChangeTime { get; set; }

        public Color WarningTimeForeground { get; set; }
        public Color PlayingTimeForeground { get; set; }
        public Color ChangeTimeForeground { get; set; }
        public Color WarningTimeBackground { get; set; }
        public Color PlayingTimeBackground { get; set; }
        public Color ChangeTimeBackground { get; set; }
        public static string GetFullPath()
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
         
            var settingsFileName = $"{appName}.settings";

            return Path.Combine(settingsFolder,settingsFileName);

        }

        public void RestoreTimingDefaults()
        {
            TotalTime = DefaultTimings.total;
            WarningTime = DefaultTimings.warn;
            ChangeTime = DefaultTimings.change;
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
            File.WriteAllText(GetFullPath(), JsonConvert.SerializeObject(new AppSettingsContainer(this)));
        }
    }
}
