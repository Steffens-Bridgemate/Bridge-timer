using BridgeTimer.Services;
using BridgeTimer.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BridgeTimer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        public IConfiguration? Configuration { get; private set; }

        public const string RoundStartedSoundFile = "roundstarted.wav";
        public const string RoundEndedSoundFile = "roundended.wav";
        public const string WarningSoundFile = "warning.wav";
        public const string SoundsFolder = "Sounds";
        public static readonly string SettingsFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.settings";

        public const string BaseLogoFile = "Bridgemate_logo_black.png";
        public const string RoundStartedLogoFile = "roundstarted.png";
        public const string RoundEndedLogoFile = "roundended.png";
        public const string WarningLogoFile = "warning.png";

        public static string GetFullAppDataPath(string? fileName=null)
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name ??
                          throw new NullReferenceException($"No name for the executing assembly found.");

            var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                              appName);

            if (fileName == null)
                return settingsFolder;

            return Path.Combine(settingsFolder, fileName);

        }

        protected override void OnStartup(StartupEventArgs e)
        {
         
            var settingsFolder = App.GetFullAppDataPath() ?? throw new NullReferenceException("No path for the settings file was defined.");

            if (!Directory.Exists(settingsFolder))
                Directory.CreateDirectory(settingsFolder);

            var settingsFile = Path.Combine(settingsFolder,SettingsFilename);
            if(!File.Exists(settingsFile))
            {
                var settings =new AppSettingsContainer( AppSettings.Default());
                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings,Formatting.Indented));
            }

            var settingLines = File.ReadAllText(settingsFile);
            var container = JsonConvert.DeserializeObject<AppSettingsContainer>(settingLines);
            var appSettings = container.AppSettings;
            var colorChanger = new ColorChanger();

            var logoFileName = Path.Combine(settingsFolder, RoundStartedLogoFile);
            if (!File.Exists(logoFileName))
            {
                colorChanger.ChangeLogoColor(ColorChanger.Convert( appSettings.PlayingTimeForeground),
                                             ColorChanger.Convert( appSettings.PlayingTimeBackground), 
                                             logoFileName);
            }

            logoFileName = Path.Combine(settingsFolder, WarningLogoFile);
            if (!File.Exists(logoFileName))
            {
                colorChanger.ChangeLogoColor(ColorChanger.Convert(appSettings.WarningTimeForeground),
                                             ColorChanger.Convert(appSettings.WarningTimeBackground),
                                             logoFileName);
            }

            logoFileName = Path.Combine(settingsFolder, RoundEndedLogoFile);
            if (!File.Exists(logoFileName))
            {
                colorChanger.ChangeLogoColor(ColorChanger.Convert(appSettings.ChangeTimeForeground),
                                             ColorChanger.Convert(appSettings.ChangeTimeBackground),
                                             logoFileName);
            }




            foreach (var soundfile in new[] {RoundStartedSoundFile,WarningSoundFile,RoundEndedSoundFile })
            {
                if (!File.Exists(Path.Combine(settingsFolder, soundfile)))
                    File.Copy(Path.Combine(SoundsFolder,soundfile), App.GetFullAppDataPath(soundfile));
            }

            var builder = new ConfigurationBuilder()
               .SetBasePath(settingsFolder)
               .AddJsonFile(SettingsFilename, optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Create a service collection and configure our dependencies
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Build the our IServiceProvider and set our static reference to it
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration?.GetSection(nameof(AppSettings)));
            services.AddScoped<ITimeProvision, TimeProvider>();

            services.AddTransient(typeof(MainWindow));
        }
    }

}
