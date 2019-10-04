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
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
         
            var fullPath = AppSettings.GetFullPath();

            var settingsFolder = Path.GetDirectoryName(fullPath);
            var settingsFileName = Path.GetFileName(fullPath);

            if (!Directory.Exists(settingsFolder))
                Directory.CreateDirectory(settingsFolder);

            var settingsFile = Path.Combine(settingsFolder,settingsFileName);
            if(!File.Exists(settingsFile))
            {
                var settings =new AppSettingsContainer( AppSettings.Default());
                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings));
            }

            var builder = new ConfigurationBuilder()
               .SetBasePath(settingsFolder)
               .AddJsonFile(settingsFileName, optional: false, reloadOnChange: true);

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
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddScoped<ITimeProvision, TimeProvider>();

            services.AddTransient(typeof(MainWindow));
        }
    }

}
