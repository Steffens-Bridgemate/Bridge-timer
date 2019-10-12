using BridgeTimer.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BridgeTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Viewmodel viewmodel;
       
        public MainWindow(IOptions< AppSettings> settings,ITimeProvision timeProvider)
        {
            InitializeComponent();
            var time = timeProvider.GetCurrentTime();
            viewmodel = new Viewmodel(new[] { new SoundThresholdNotificator() },settings.Value);
            viewmodel.SettingsRequested += EditSettings;
            viewmodel.CloseRequested +=(s,e)=> this.Close();
            this.DataContext = viewmodel;
            viewmodel.SettingsCommand.Execute(null);
        }

        private void EditSettings(object? sender, Viewmodel.SettingsRequestedEventArgs e)
        {
            var window = new SettingsWindow();
            window.DataContext = sender as Viewmodel;
            window.ShowDialog();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewmodel == null) return;
            viewmodel.ShowControlPanel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (viewmodel == null) return;
            viewmodel.ShowControlPanel();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (viewmodel == null) return;
            if (e.Key != Key.Escape) return;

            if (viewmodel.HideControlPanel)
                viewmodel.ShowControlPanel();
            else
                return;
        }
    }
}
